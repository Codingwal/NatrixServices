using ARSoft.Tools.Net.Dns;

namespace NatrixServices.DnsBlocker;

public class DnsBlockerService(IServiceProvider ServiceProvider, ILogger<DnsBlockerService> Logger) : BackgroundService
{
    private readonly DnsServer server = new();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Logger.LogInformation("Starting DNS server");
        server.QueryReceived += OnQueryReceived;
        server.Start();

        await Task.Delay(Timeout.Infinite, stoppingToken);

        Logger.LogInformation("Stopping DNS server");
        server.QueryReceived -= OnQueryReceived;
        server.Stop();
    }

    private async Task OnQueryReceived(object sender, QueryReceivedEventArgs e)
    {
        try
        {
            if (e.Query is not DnsMessage query) return;

            // using IServiceScope scope = ServiceProvider.CreateScope();
            var configContext = ServiceProvider.GetRequiredService<ConfigContext>();
            var dataContext = ServiceProvider.GetRequiredService<DataContext>();

            DnsBlocker dnsBlocker = new(Logger, configContext, dataContext);
            e.Response = await dnsBlocker.ProcessDnsQuery(query, null, null);

            Logger.LogInformation("Finished handling dns request");
        }
        catch (Exception exception)
        {
            Logger.LogError($"DnsBlocker error: {exception.Message}");
        }
    }
}

public class DnsBlocker(ILogger Logger, ConfigContext ConfigContext, DataContext DataContext)
{
    public async Task<DnsMessage?> ProcessDnsQuery(DnsMessage query, UserId? userId, DeviceId? deviceId)
    {
        if (query.Questions.Count == 0) return null;

        // Get the DataContext
        GlobalConfig globalConfig = await ConfigContext.GetGlobalData();

        // Return if the dns server should do nothing at all
        if (!globalConfig.EnableDnsServer)
        {
            Logger.LogInformation($"Ignoring request because globalConfig.EnableDnsServer is false");
            return null;
        }

        // Get the domain which ip is requested
        DnsQuestion question = query.Questions[0];
        string domainName = question.Name.ToString();

        Logger.LogInformation($"Handling request to {domainName}");

        // Block or forward the DNS request
        DnsMessage response = query.CreateResponseInstance();
        bool blockDomain = await BlockDomain(domainName, userId, deviceId, ConfigContext);
        if (blockDomain)
        {
            Logger.LogInformation($"Blocking \"{domainName}\"");
            response.ReturnCode = ReturnCode.NxDomain;
        }
        else
        {
            Logger.LogInformation($"Forwarding \"{domainName}\"");
            DnsMessage? upstreamResponse = await ForwardQuestion(question);

            if (upstreamResponse != null)
            {
                response.AnswerRecords.AddRange(upstreamResponse.AnswerRecords);
                response.ReturnCode = upstreamResponse.ReturnCode;
            }
            else
                Logger.LogError("Error: upstreamResponse is null");
        }

        DnsRequest requestData = new()
        {
            Time = DateTimeOffset.UtcNow,
            Domain = domainName,
            Blocked = blockDomain,
            UserId = userId,
            DeviceId = deviceId
        };

        await UpdateData(requestData);

        return response;
    }
    private static async Task<bool> BlockDomain(string domainName, UserId? userId, DeviceId? deviceId, ConfigContext configContext)
    {
        GlobalConfig globalConfig = await configContext.GetGlobalData();

        if (!globalConfig.EnableBlocking)
            return false;

        List<string> domainsToBlock = [];
        if (userId == null)
            domainsToBlock = ["jamf", "test.de"];
        else
        {
            UserConfig? userConfig = await configContext.GetUserData(userId);
            if (userConfig == null) return false;

            if (!BlockingEnabled(userConfig, deviceId, globalConfig))
                return false;

            domainsToBlock = GetDomainsToBlock(userConfig, globalConfig);
        }

        bool blockDomain = domainsToBlock.Any(x => domainName.Contains(x, StringComparison.OrdinalIgnoreCase));

        return blockDomain;
    }
    private static bool BlockingEnabled(UserConfig userConfig, DeviceId? deviceId, GlobalConfig globalConfig)
    {
        if (!globalConfig.EnableBlocking)
            return false;

        if (deviceId == null)
            return userConfig.EnableBlocking;

        if (!userConfig.Devices.TryGetValue(deviceId, out DeviceConfig? deviceConfig))
            return false;

        return deviceConfig.EnableBlocking;
    }
    private static List<string> GetDomainsToBlock(UserConfig userConfig, GlobalConfig globalConfig)
    {
        List<string> domainsToBlock = [];
        foreach ((_, FilterReference filterRef) in userConfig.Filters)
        {
            if (!globalConfig.Filters.TryGetValue(filterRef.Id, out FilterConfig? filterConfig))
                continue;

            domainsToBlock.AddRange(filterConfig.DomainsToBlock);
        }
        return domainsToBlock;
    }

    private async Task UpdateData(DnsRequest requestData)
    {
        GlobalData globalData = await DataContext.GetGlobalData();
        globalData.DnsRequestCount++;
        globalData.LastRequest = requestData;

        if (requestData.UserId != null)
        {
            UserData userData = await DataContext.GetUserData(requestData.UserId) ?? new();
            userData.DnsRequestCount++;
            userData.LastRequest = requestData with { }; // with { } creates a shallow copy
        }

        await DataContext.SaveChangesAsync();
    }

    private static async Task<DnsMessage?> ForwardQuestion(DnsQuestion question)
    {
        var upstreamResponse = await DnsClient.Default.ResolveAsync(question.Name, question.RecordType, question.RecordClass);
        return upstreamResponse;
    }
}