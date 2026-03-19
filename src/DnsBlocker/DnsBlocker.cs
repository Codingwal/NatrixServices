using System.Net;
using ARSoft.Tools.Net.Dns;
using Microsoft.EntityFrameworkCore;

namespace NatrixServices;

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
        DnsMessage? query = e.Query as DnsMessage;

        // Check if there really is a question
        if (query == null || query.Questions.Count == 0)
            return;

        IPAddress ipAddr = e.RemoteEndpoint.Address.MapToIPv4();

        // Get the DataContext
        using IServiceScope scope = serviceProvider.CreateScope();
        DataContext dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        // Check if the ip address is registered in the config of a natrix user
        DnsBlockerConfig? config = await dataContext.DnsBlockerConfigs.FirstOrDefaultAsync(x => x.IPAddress == ipAddr.ToString());
        if (config == null)
        {
            logger.LogInformation("Ignoring request from {ipAddr} as it is not listed in any users list", ipAddr);
            return;
        }

        // Get the domain which ip is requested
        DnsQuestion question = query.Questions[0];
        string domainName = question.Name.ToString();

        // Block or forward the DNS request
        DnsMessage response = query.CreateResponseInstance();
        if (BlockDomain(domainName, config))
        {
            logger.LogInformation("Blocking \"{domainName}\"", domainName);
            response.ReturnCode = ReturnCode.NxDomain;
        }
        else
        {
            logger.LogInformation("Forwarding \"{domainName}\"", domainName);
            DnsMessage? upstreamResponse = await ForwardQuestion(question);

            if (upstreamResponse != null)
            {
                response.AnswerRecords.AddRange(upstreamResponse.AnswerRecords);
                response.ReturnCode = upstreamResponse.ReturnCode;
            }
            else
                logger.LogError("Error: upstreamResponse is null");
        }

        e.Response = response;
    }
    private static bool BlockDomain(string domainName, DnsBlockerConfig config)
    {
        if (!config.Block)
            return false;

        foreach (string domainToBlock in config.DomainsToBlock.Domains)
        {
            if (domainName.Contains(domainToBlock, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }
    private static async Task<DnsMessage?> ForwardQuestion(DnsQuestion question)
    {
        var upstreamResponse = await DnsClient.Default.ResolveAsync(question.Name, question.RecordType, question.RecordClass);
        return upstreamResponse;
    }
}