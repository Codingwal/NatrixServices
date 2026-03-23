using System.Net;
using ARSoft.Tools.Net.Dns;
using Microsoft.EntityFrameworkCore;

namespace NatrixServices.DnsBlocker;

public class DnsBlocker(IServiceProvider serviceProvider, ILogger<DnsBlocker> logger) : BackgroundService
{
    private readonly DnsServer server = new();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Starting DNS server");
        server.QueryReceived += OnQueryReceived;
        server.Start();

        await Task.Delay(Timeout.Infinite, stoppingToken);

        logger.LogInformation("Stopping DNS server");
        server.QueryReceived -= OnQueryReceived;
        server.Stop();
    }

    private async Task OnQueryReceived(object sender, QueryReceivedEventArgs e)
    {
        try
        {
            await HandleQuery(sender, e);
            logger.LogInformation("Finished handling dns request");
        }
        catch (Exception exception)
        {
            logger.LogError($"DnsBlocker error: {exception.Message}");
        }
    }
    private async Task HandleQuery(object sender, QueryReceivedEventArgs e)
    {
        DnsMessage? query = e.Query as DnsMessage;

        // Check if there really is a question
        if (query == null || query.Questions.Count == 0)
            return;


        IPAddress ipAddr = e.RemoteEndpoint.Address.MapToIPv4();
        string ipAddrStr = ipAddr.ToString();

        logger.LogInformation($"Received request from {ipAddr}");

        // Get the DataContext
        using IServiceScope scope = serviceProvider.CreateScope();
        var configContext = scope.ServiceProvider.GetRequiredService<ConfigContext>();
        GlobalConfig globalConfig = await configContext.GetGlobalData();

        // Return if the dns server should do nothing at all
        if (!globalConfig.EnableDnsServer)
        {
            logger.LogInformation($"Ignoring request because globalConfig.EnableDnsServer is false");
            return;
        }

        // Check if the ip address is registered in the config of a user
        List<UserConfig> users = await configContext.UserData.ToListAsync();
        UserConfig? userConfig = users.FirstOrDefault(x => x.IPAddresses.Contains(ipAddrStr));
        if (userConfig == null)
        {
            logger.LogInformation($"Ignoring request from {ipAddr} as it is not listed in any users list");
            return;
        }

        // Get the domain which ip is requested
        DnsQuestion question = query.Questions[0];
        string domainName = question.Name.ToString();

        logger.LogInformation($"Handling request to {domainName} by {ipAddr}");

        // Block or forward the DNS request
        DnsMessage response = query.CreateResponseInstance();
        if (BlockDomain(domainName, userConfig, globalConfig))
        {
            logger.LogInformation($"Blocking \"{domainName}\"");
            response.ReturnCode = ReturnCode.NxDomain;
        }
        else
        {
            logger.LogInformation($"Forwarding \"{domainName}\"");
            DnsMessage? upstreamResponse = await ForwardQuestion(question);

            if (upstreamResponse != null)
            {
                response.AnswerRecords.AddRange(upstreamResponse.AnswerRecords);
                response.ReturnCode = upstreamResponse.ReturnCode;
            }
            else
                logger.LogError("Error: upstreamResponse is null");
        }

        await UpdateData(scope, userConfig.UserId);

        e.Response = response;
    }
    private static bool BlockDomain(string domainName, UserConfig userConfig, GlobalConfig globalConfig)
    {
        // Is blocking enabled locally and globally?
        if (!globalConfig.EnableBlocking || !userConfig.EnableBlocking)
            return false;

        // Check if the domain should be blocked
        bool blockDomain = userConfig.DomainsToBlock.Any(x => domainName.Contains(x, StringComparison.OrdinalIgnoreCase));

        return blockDomain;
    }

    private static async Task UpdateData(IServiceScope scope, UserId userId)
    {
        var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        // Update user data
        UserData userData = await dataContext.GetUserData(userId) ?? new();
        userData.DnsRequestCount++;
        userData.LastRequestTime = DateTimeOffset.UtcNow;
        await dataContext.SetUserData(userId, userData);

        // Update global data
        GlobalData globalData = await dataContext.GetGlobalData();
        globalData.DnsRequestCount++;
        globalData.LastRequestTime = DateTimeOffset.UtcNow;
        await dataContext.SetGlobalData(globalData);
    }

    private static async Task<DnsMessage?> ForwardQuestion(DnsQuestion question)
    {
        var upstreamResponse = await DnsClient.Default.ResolveAsync(question.Name, question.RecordType, question.RecordClass);
        return upstreamResponse;
    }
}