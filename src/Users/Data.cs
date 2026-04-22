using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace NatrixServices.Users;

public class UserDataContext(DbContextOptions<UserDataContext> options) : DatabaseItemStorage<UserData, string>(options)
{
    public async Task<bool> UserExistsAsync(string username) => await ItemExistsAsync(username);
    public async Task<bool> AuthenticateAsync(string username, string passwordHash)
    {
        UserData? userData = await GetItemAsync(username);
        if (userData == null) return false;

        return userData.PasswordHash == passwordHash;
    }
}

public class UserData : IIdentifiable<string>
{
    [Key, Required, StringLength(8)]
    public string Username { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;
    public string LinkedAccount { get; set; } = string.Empty; // Not used yet

    public string Id { get => Username; set => Username = value; }
}