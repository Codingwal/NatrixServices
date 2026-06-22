using NatrixServices.Chess.Core.Entities;
using NatrixServices.Chess.Core.Services;

namespace NatrixServices.Chess.Core.MatchGenerator;

public class MatchGenerator2 : IMatchGenerator
{
    private const string Placeholder = "$NONE$";
    public IEnumerable<ChessGame> GenerateGames(ChessEvent chessEvent)
    {
        if (chessEvent.EventType != EventType.Season)
            throw new NotImplementedException();

        var userMatchups = GetMatchups(chessEvent.Players);

        TimeSpan timePerMatch = chessEvent.TotalEventDuration / userMatchups.Count();

        foreach (var match in userMatchups)
        {
            if (match.Player1 == Placeholder || match.Player2 == Placeholder)
                continue;

            ChessGame chessGame = new(GameId.Generate(), "TODO", chessEvent.IsPublic,
                chessEvent.TimePerPlayer, Fen.DefaultFen, chessEvent.EventId);

            chessGame.JoinGame(match.Player1);
            chessGame.JoinGame(match.Player2);

            yield return chessGame;
        }
    }

    public IEnumerable<Matchup> GetMatchups(List<string> players)
    {
        throw new NotImplementedException();

        if (players.Count % 2 != 0)
            players.Add(Placeholder);

        int playerCount = players.Count;
        int roundCount = playerCount - 1;
        int gamesPerRound = playerCount / 2;

        for (int round = 0; round < roundCount; round++)
        {
            for (int game = 0; game < gamesPerRound; game++)
            {


                // int p1 = ( + round) % (playerCount - 1);
                // yield return new(player1, player2);
            }
        }
    }

    public record Matchup(string Player1, string Player2);
}