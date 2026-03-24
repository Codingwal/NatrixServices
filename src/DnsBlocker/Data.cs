using Microsoft.EntityFrameworkCore;

namespace NatrixServices.DnsBlocker;

public class DataContext(DbContextOptions<DataContext> options) : DataContext<UserData, GlobalData>(options) { }

public class UserData : UserDataBase
{
    public int DnsRequestCount { get; set; } = 0;
    public LastRequest LastRequest { get; set; } = new();
}

public class GlobalData
{
    public int DnsRequestCount { get; set; } = 0;
    public LastRequest LastRequest { get; set; } = new();
}

public class LastRequest
{
    public DateTimeOffset Time { get; set; } = default;
    public string Domain { get; set; } = string.Empty;
    public bool Blocked { get; set; } = false;
    public UserId UserId { get; set; } = UserId.Empty;
    public string DeviceId { get; set; } = string.Empty;
}