namespace NatrixServices.Users.Core.Entities;

public class UserData(string username, string passwordHash)
{
    public string Username { get; set; } = username;

    public string PasswordHash { get; set; } = passwordHash;
    public string? LinkedAccount { get; set; } = null;
}