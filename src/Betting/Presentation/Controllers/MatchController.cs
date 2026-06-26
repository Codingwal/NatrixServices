using Microsoft.AspNetCore.Mvc;
using NatrixServices.Betting.Application.Commands;
using NatrixServices.Betting.Core.Entities;
using NatrixServices.Betting.Presentation.DTOs;
using NatrixServices.Shared.Application;
using NatrixServices.Shared.Core;

namespace NatrixServices.Betting.Presentation.Controllers;

[ApiController]
[Route("api/betting/matches")]
public class MatchController(ICommandDispatcher dispatcher) : ControllerBase
{
    [HttpPost]
    [AuthAsAdmin]
    public async Task<IActionResult> CreateMatchAsync([FromBody] CreateMatchRequest request)
    {
        return await request.ToCommand()
            .Map(dispatcher.ExecuteCommandAsync<CreateMatchCommand, MatchId>)
            .Map(matchId => new { matchId })
            .ToActionResult();
    }

    [HttpPost]
    [AuthAsAdmin]
    [Route("{matchId}/result")]
    public async Task<IActionResult> SetMatchResultAsync(MatchId matchId, [FromBody] MatchResultDTO result)
    {
        return await result.ToMatchResult()
            .Map<SetMatchResultCommand>(res => new SetMatchResultCommand(matchId, res))
            .Then(dispatcher.ExecuteCommandAsync)
            .ToActionResult();
    }

    [HttpGet]
    [NoAuth]
    public async Task<IActionResult> GetMatchesAsync([FromQuery] string? status, [FromQuery] string? Event)
    {
        return await StatusDTO.ToOpenFilter(status)
            .Map<GetMatchesCommand>(open => new GetMatchesCommand(open, Event))
            .Map(dispatcher.ExecuteCommandAsync<GetMatchesCommand, IEnumerable<Match>>)
            .ToActionResult();
    }
}