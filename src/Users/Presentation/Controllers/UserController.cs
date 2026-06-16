using Microsoft.AspNetCore.Mvc;
using NatrixServices.Shared.Application;
using NatrixServices.Shared.Core;
using NatrixServices.Users.Application.Commands;
using NatrixServices.Users.Core.Entities;
using NatrixServices.Users.Presentation.DTOs;

namespace NatrixServices.Users.Presentation.Controllers;

[ApiController]
[Route("api/users")]
public class UserController(ICommandDispatcher dispatcher) : ControllerBase
{
    [HttpPost]
    [NoAuth]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        CreateUserCommand command = new(request.Username, request.PasswordHash);

        var result = await dispatcher.ExecuteCommandAsync(command);
        if (result.IsFailure) return result.Error.ToActionResult();

        return Created($"api/users/{request.Username}", new { username = request.Username });
    }

    [HttpGet("{username}")]
    [AuthAsUser("username")]
    public async Task<IActionResult> GetUserInfo(string username)
    {
        GetUserCommand command = new(username);

        return await dispatcher.ExecuteCommandAsync<GetUserCommand, UserData>(command)
            .Map(user => new { user.Username })
            .ToActionResult();
    }
}