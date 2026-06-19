using Microsoft.AspNetCore.Mvc;
using NatrixServices.Chess.Application.Commands;
using NatrixServices.Chess.Core.Entities;
using NatrixServices.Chess.Presentation.DTOs;
using NatrixServices.Shared.Application;
using NatrixServices.Shared.Core;

namespace NatrixServices.Chess.Presentation.Controllers;

public class UserNotFoundException() : Exception("User not found");

[ApiController]
[Route("api/chess/users")]
public class UserController(ICommandDispatcher dispatcher) : ControllerBase
{
    [HttpGet("{username}")]
    [NoAuth]
    public async Task<IActionResult> GetUserData(string username)
    {
        GetUserCommand command = new(username);

        return await dispatcher.ExecuteCommandAsync<GetUserCommand, UserData>(command)
            .Map(userData => new UserDataDTO(userData))
            .ToActionResult();
    }

    [HttpGet("{username}/games")]
    [AuthAsUser("username")]
    public async Task<IActionResult> GetUserGames(string username, [FromQuery] string? status = null)
    {
        GameStatus? gameStatus;
        if (status == null)
            gameStatus = null;
        else
        {
            var res = GameStatusDTO.StrToStatus(status);
            if (res.IsFailure) return res.Error.ToActionResult();
            gameStatus = res.Value;
        }

        GetGamesCommand command = new(OnlyPublic: false, gameStatus, username);

        return await dispatcher.ExecuteCommandAsync<GetGamesCommand, IEnumerable<ChessGame>>(command)
            .Map(games => new GameListDTO(games))
            .ToActionResult();
    }

    [HttpGet("{username}/stats")]
    [NoAuth]
    public async Task<IActionResult> GetUserStats(string username)
    {
        GetUserCommand command = new(username);

        return await dispatcher.ExecuteCommandAsync<GetUserCommand, UserData>(command)
            .Map(userData => userData.Stats)
            .ToActionResult();
    }
}