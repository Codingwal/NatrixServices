using NatrixServices.Users.Core.Entities;

namespace NatrixServices.Users.Presentation.DTOs;

public record CreateUserRequest(string Username, string PasswordHash);
public record LinkAccountRequest(string Account);

public record UserDataDTO(string Username, string? LinkedAccount)
{
    public UserDataDTO(UserData data)
        : this(data.Username, data.LinkedAccount)
    {
    }
}