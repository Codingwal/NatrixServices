using System.Reflection;
using ARSoft.Tools.Net.Dns;
using Microsoft.AspNetCore.Mvc;

namespace NatrixServices.DnsBlocker;

[Route("dns-query")]
[ApiController]
public class DnsRequestAPI(ILogger<DnsRequestAPI> Logger, ConfigContext ConfigContext, DataContext DataContext) : ControllerBase
{
    private const string DNS_CONTENT_TYPE = "application/dns-message";
    private const int NOT_ALLOWED = 405;

    [HttpPost]
    [HttpPost("{userId}")]
    [HttpPost("{userId}/{deviceId}")]
    public async Task<IActionResult> PostDnsQuery(UserId? userId = null, DeviceId? deviceId = null)
    {
        if (Request.ContentType != DNS_CONTENT_TYPE)
            return StatusCode(NOT_ALLOWED);

        using MemoryStream stream = new();
        await Request.Body.CopyToAsync(stream);
        byte[] query = stream.ToArray();

        return await ProcessDnsRequest(query, userId, deviceId);
    }

    [HttpGet()]
    [HttpGet("{userId}")]
    [HttpGet("{userId}/{deviceId}")]
    public async Task<IActionResult> GetDnsQuery([FromQuery] string dns, UserId? userId = null, DeviceId? deviceId = null)
    {
        if (string.IsNullOrEmpty(dns))
            return BadRequest();

        if (Request.ContentType != DNS_CONTENT_TYPE)
            return StatusCode(NOT_ALLOWED);

        string base64 = dns.Replace('-', '+').Replace('_', '/');

        if (dns.Length % 4 != 0)
            base64 += new string('=', 4 - dns.Length % 4);

        return await ProcessDnsRequest(Convert.FromBase64String(base64), userId, deviceId);
    }

    private async Task<IActionResult> ProcessDnsRequest(byte[] queryBytes, UserId? userId, DeviceId? deviceId)
    {
        DnsMessage query;
        try
        {
            query = DnsMessage.Parse(queryBytes);
        }
        catch
        {
            Logger.LogWarning("Returning because of failed parsing");
            return BadRequest();
        }

        DnsBlocker dnsBlocker = new(Logger, ConfigContext, DataContext);
        DnsMessage? response = await dnsBlocker.ProcessDnsQuery(query, userId, deviceId);

        if (response == null)
            return BadRequest();

        Logger.LogInformation("Encoding response");

        byte[] responseBytes = EncodeResponse(response);

        return File(responseBytes, DNS_CONTENT_TYPE);
    }

    private byte[] EncodeResponse(DnsMessage response)
    {
        MethodInfo? method = typeof(DnsMessageBase).GetMethod("Encode", BindingFlags.NonPublic | BindingFlags.Instance, []);

        if (method == null)
        {
            Logger.LogError("Failed to find method DnsMessageBase.Encode()");
            throw new();
        }

        DnsRawPackage? rawPackage = (DnsRawPackage?)method.Invoke(response, []);

        if (rawPackage == null)
        {
            Logger.LogError("RawPackage is null");
            throw new();
        }

        return rawPackage.ToArraySegment(false).ToArray();
    }

    private void LogIpAddresses(DnsMessage msg)
    {
        IEnumerable<string> ipAddresses = msg.AnswerRecords
            .Select(answer =>
            {
                if (answer is ARecord aRecord)
                    return aRecord.Address.ToString();
                else if (answer is AaaaRecord aaaaRecord)
                    return aaaaRecord.Address.ToString();
                return "";
            });

        foreach (string ipAddr in ipAddresses)
            Logger.LogInformation($"IP: \"{ipAddr}\"");
    }

}
