namespace NatrixServices.Users.Presentation.DTOs;

public record CreateUserRequest(string Username, string PasswordHash);