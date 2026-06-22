namespace NatrixServices.Shared.Application;

public abstract class AuthAttribute : Attribute;


[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class NoAuthAttribute : AuthAttribute;


[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class AuthAsUserAttribute(string? _usernameRouteParam = null) : AuthAttribute
{
    public string? UsernameRouteParam { get; } = _usernameRouteParam;
}


[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class AuthAsAdminAttribute : AuthAttribute;