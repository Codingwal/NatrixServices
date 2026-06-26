using Microsoft.AspNetCore.Mvc;
using NatrixServices.Betting.Application.Commands;
using NatrixServices.Betting.Core.Entities;
using NatrixServices.Betting.Presentation.DTOs;
using NatrixServices.Shared.Application;
using NatrixServices.Shared.Core;
using NatrixServices.Shared.Infrastructure;

namespace NatrixServices.Betting.Presentation.Controllers;

[ApiController]
[Route("api/betting/bets")]
public class BettingController(ICommandDispatcher dispatcher) : ControllerBase
{
    [HttpPost]
    [AuthAsUser]
    public async Task<IActionResult> PostBetAsync([FromBody] BetDTO bet)
    {
        if (!MatchId.TryParse(bet.MatchId, out var matchId))
            return Result.Failure(ErrorType.BadRequest, $"Invalid MatchId \"{bet.MatchId}\".").ToActionResult();

        var command = new PlaceBetCommand(HttpContext.GetUsername(), matchId, new(bet.Player1, bet.Player2), bet.Stake);

        return await dispatcher.ExecuteCommandAsync(command)
            .ToActionResult();
    }
}