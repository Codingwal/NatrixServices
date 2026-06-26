using Microsoft.AspNetCore.Mvc;
using NatrixServices.Betting.Application.Commands;
using NatrixServices.Betting.Core.Entities;
using NatrixServices.Betting.Presentation.DTOs;
using NatrixServices.Shared.Application;
using NatrixServices.Shared.Core;

namespace NatrixServices.Betting.Presentation.Controllers;

[ApiController]
[Route("api/betting/users")]
public class UserController(ICommandDispatcher dispatcher) : ControllerBase
{
    [HttpGet]
    [Route("{username}")]
    [AuthAsUser("username")]
    public async Task<IActionResult> GetUserDataAsync(string username)
    {
        return await dispatcher.ExecuteCommandAsync<GetUserCommand, UserData>(new(username))
            .Map(user => new UserDataDTO(user.Username, user.Balance))
            .ToActionResult();
    }

    [HttpGet]
    [Route("{username}/bets")]
    [AuthAsUser("username")]
    public async Task<IActionResult> GetBetsAsync(string username, [FromQuery] string? status)
    {
        return await StatusDTO.ToOpenFilter(status)
            .Map<GetBetsCommand>(open => new GetBetsCommand(username, MatchId: null, open))
            .Map(dispatcher.ExecuteCommandAsync<GetBetsCommand, IEnumerable<Bet>>)
            .Map(bets => new BetListDTO(bets))
            .ToActionResult();
    }
}