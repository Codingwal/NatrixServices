using System.ComponentModel.DataAnnotations;

namespace NatrixServices.DnsBlocker;

public class DeviceConfigDTO : IBlockingConfig
{
    [Required] public DeviceId Id { get; set; } = DeviceId.Empty;
    [Required] public bool EnableBlocking { get; set; } = false;
}
public class FilterReferenceDTO : IBlockingConfig
{
    [Required] public FilterId Id { get; set; } = FilterId.Empty;
    [Required] public bool EnableBlocking { get; set; } = true;
}
public class FilterConfigDTO
{
    [Required] public FilterId Id { get; set; } = FilterId.Empty;
    public string Description { get; set; } = string.Empty;
    [Required] public List<string> DomainsToBlock { get; set; } = [];
}
public record DnsRequestDTO
{
    public DateTimeOffset Time { get; set; } = default;
    public string Domain { get; set; } = string.Empty;
    public bool Blocked { get; set; } = false;
    public string? Username { get; set; } = null;
    public DeviceId? DeviceId { get; set; } = null;
}
