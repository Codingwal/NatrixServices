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
        if (e.Query is not DnsMessage query) return;

        using IServiceScope scope = ServiceProvider.CreateScope();
        var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        DnsBlocker dnsBlocker = new(Logger, dataContext);

        try
        {
            e.Response = await dnsBlocker.ProcessDnsQuery(query, null, null);
        }
        catch (Exception exception)
        {
            Logger.LogError($"DnsBlocker error: {exception.Message}");
        }
    }
}

public class DnsBlocker(ILogger Logger, DataContext DataContext)
{
    public async Task<DnsMessage?> ProcessDnsQuery(DnsMessage query, string? username, DeviceId? deviceId)
    {
        if (query.Questions.Count == 0) return null;

        // Get the DataContext
        var globalData = await DataContext.GetGlobalData();

        // Return if the dns server should do nothing at all
        if (!globalData.EnableDnsServer)
        {
            Logger.LogInformation($"Ignoring request because globalConfig.EnableDnsServer is false");
            return null;
        }

        // Get the domain which ip is requested
        DnsQuestion question = query.Questions[0];
        string domainName = question.Name.ToString();

        // Block or forward the DNS request
        DnsMessage response = query.CreateResponseInstance();
        bool blockDomain = await BlockDomain(domainName, username, deviceId, DataContext);
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

        DnsRequestDTO requestData = new()
        {
            Time = DateTimeOffset.UtcNow,
            Domain = domainName,
            Blocked = blockDomain,
            Username = username,
            DeviceId = deviceId
        };

        await UpdateData(requestData);

        return response;
    }
    private static async Task<bool> BlockDomain(string domainName, string? username, DeviceId? deviceId, DataContext dataContext)
    {
        var globalData = await dataContext.GetGlobalData();

        if (!globalData.EnableBlocking)
            return false;

        List<string> domainsToBlock = [];
        if (username == null)
            domainsToBlock = ["jamf", "test.de"];
        else
        {
            var userData = await dataContext.GetUserData(username);
            if (userData == null) return false;

            if (!BlockingEnabled(userData, deviceId))
                return false;

            domainsToBlock = GetDomainsToBlock(userData, globalData);
        }

        bool blockDomain = domainsToBlock.Any(x => domainName.Contains(x, StringComparison.OrdinalIgnoreCase));

        return blockDomain;
    }
    private static bool BlockingEnabled(UserData userData, DeviceId? deviceId)
    {
        if (deviceId == null)
            return userData.EnableBlocking;

        if (!userData.Devices.TryGetValue(deviceId, out var deviceConfig))
            return false;

        return deviceConfig.EnableBlocking;
    }
    private static List<string> GetDomainsToBlock(UserData userData, GlobalData globalData)
    {
        List<string> domainsToBlock = [];
        foreach (FilterReferenceDTO filterRef in userData.Filters.Values)
        {
            if (!globalData.Filters.TryGetValue(filterRef.Id, out var filterConfig))
                continue;

            domainsToBlock.AddRange(filterConfig.DomainsToBlock);
        }
        return domainsToBlock;
    }

    private async Task UpdateData(DnsRequestDTO requestData)
    {
        GlobalData globalData = await DataContext.GetGlobalData();
        globalData.DnsRequestCount++;
        globalData.LastRequest = requestData;

        if (requestData.Username != null)
        {
            UserData userData = await DataContext.GetUserData(requestData.Username) ?? new();
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