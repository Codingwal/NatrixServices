using System.Net;
using ARSoft.Tools.Net.Dns;

namespace NatrixServices;

public class DnsBlocker
{
    private readonly DnsServer server;
    public DnsBlocker()
    {
        server = new DnsServer();
        server.QueryReceived += OnQueryReceived;
    }

    public void Start()
    {
        Console.WriteLine("Starting DNS server");
        server.Start();
    }

    public void Stop()
    {
        Console.WriteLine("Stopping DNS server");
        server.Stop();
    }

    private async Task OnQueryReceived(object sender, QueryReceivedEventArgs e)
    {
        DnsMessage? query = e.Query as DnsMessage;

        // Check if there really is a question
        if (query == null || query.Questions.Count == 0)
            return;

        IPAddress ipAddr = e.RemoteEndpoint.Address.MapToIPv4();

        // Check if the ip address is registered in the config of a natrix user
        if (!GetUserConfig(ipAddr, out DnsBlockerConfig config))
        {
            Console.WriteLine($"Ignoring request from {ipAddr} as it is not listed in any users list");
            return;
        }

        // Get the domain which ip is requested
        DnsQuestion question = query.Questions[0];
        string domainName = question.Name.ToString();

        // Block or forward the DNS request
        DnsMessage response = query.CreateResponseInstance();
        if (BlockDomain(domainName, config))
        {
            Console.WriteLine($"Blocking \"{domainName}\"");
            response.ReturnCode = ReturnCode.NxDomain;
        }
        else
        {
            Console.WriteLine($"Forwarding \"{domainName}\"");
            DnsMessage? upstreamResponse = await ForwardQuestion(question);

            if (upstreamResponse != null)
            {
                response.AnswerRecords.AddRange(upstreamResponse.AnswerRecords);
                response.ReturnCode = upstreamResponse.ReturnCode;
            }
            else
                Console.WriteLine("Error: upstreamResponse is null");
        }

        e.Response = response;
    }

    private static bool GetUserConfig(IPAddress ipAddr, out DnsBlockerConfig config)
    {
        var userConfigs = ConfigHandler.GetAllUserConfigs<DnsBlockerConfig>();

        string ipAddrStr = ipAddr.ToString();

        var (userId, userConfig) = userConfigs.FirstOrDefault(x => x.Value.IPAddresses.Contains(ipAddrStr));

        config = userConfig;
        return userId != null;
    }
    private static bool BlockDomain(string domainName, DnsBlockerConfig config)
    {
        if (!config.Block)
            return false;

        foreach (string domainToBlock in config.DomainsToBlock)
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