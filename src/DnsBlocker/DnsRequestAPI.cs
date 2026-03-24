// using ARSoft.Tools.Net.Dns;
// using Microsoft.AspNetCore.Mvc;

// namespace NatrixServices.DnsBlocker;

// [Route("dns")]
// [ApiController]
// public class DnsRequestAPI(DataContext DataContext) : ControllerBase
// {
//     private const string DNS_CONTENT_TYPE = "application/dns-message";
//     private const int NOT_ALLOWED = 405;

//     [HttpPost("{userId}/{deviceId}")]
//     public async Task<IActionResult> PostDnsQuery(UserId userId, DeviceId deviceId)
//     {
//         if (Request.ContentType != DNS_CONTENT_TYPE)
//             return StatusCode(NOT_ALLOWED);

//         using MemoryStream stream = new();
//         await Request.Body.CopyToAsync(stream);
//         byte[] query = stream.ToArray();

//         return await HandleDnsRequest(query);
//     }

//     private async Task<IActionResult> HandleDnsRequest(byte[] queryBytes)
//     {
//         DnsMessage query = DnsMessage.Parse(queryBytes);
//     }
// }
