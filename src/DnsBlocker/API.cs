using Microsoft.AspNetCore.Mvc;

namespace NatrixServices.DnsBlocker;

[Route("api/dnsblocker/config")]
public class ConfigAPI(ConfigContext configContext)
    : DataAPI<UserConfig, GlobalConfig>(configContext, AdminOnlySet: false)
{
}

[Route("api/dnsblocker/data")]
public class DataAPI(DataContext dataContext)
    : DataAPI<UserData, GlobalData>(dataContext, AdminOnlySet: true)
{

}