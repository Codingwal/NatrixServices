using NatrixServices.Chess.Core.Entities;
using NatrixServices.Chess.Core.Services;

namespace NatrixServices.Chess.Core.MatchGenerator;

public class MatchGenerator : IMatchGenerator
{
    public IEnumerable<ChessGame> GenerateGames(ChessEvent chessEvent)
    {
        throw new NotImplementedException();
        // if (chessEvent.EventType != EventType.Season)
        //     throw new NotImplementedException();

        // var userMatchups = GetCombinations(chessEvent.Players);

        // TimeSpan timePerMatch = chessEvent.TotalEventDuration / userMatchups.Count();

        // if (timePerMatch <  )

        //     foreach (var (player1, player2) in userMatchups)
        //     {
        //         ChessGame chessGame = new(GameId.Generate(), "TODO", chessEvent.IsPublic,
        //             chessEvent.TimePerPlayer, Fen.DefaultFen, chessEvent.EventId);

        //         chessGame.JoinGame(player1);
        //         chessGame.JoinGame(player2);

        //         yield return chessGame;
        //     }
    }

    private static List<Matchup> GetMatchups(List<string> players)
    {
        var combinations = GetCombinations(players);

        List<Matchup> matches = [];
        Dictionary<string, int> gamesRested = players.ToDictionary(p => p, p => 0);

        while (combinations.Count != 0)
        {
            int highestSum = int.MaxValue;
            Matchup bestMatch = default!;
            foreach (var match in combinations)
            {
                int sum = gamesRested[match.Player1] + gamesRested[match.Player2];

                if (sum > highestSum)
                {
                    highestSum = sum;
                    bestMatch = match;
                }
            }

            matches.Add(bestMatch);
            combinations.Remove(bestMatch);

            foreach (var player in players)
            {
                if (player == bestMatch.Player1 || player == bestMatch.Player2)
                    gamesRested[player] = 0;
                else
                    gamesRested[player]++;
            }
        }

        return matches;
    }

    private static List<Matchup> GetCombinations(List<string> players)
    {
        List<Matchup> combinations = [];

        for (int i = 0; i < players.Count; i++)
        {
            for (int j = i + 1; j < players.Count; j++)
            {
                combinations.Add(new(players[i], players[j]));
            }
        }

        return combinations;
    }

    private record Matchup(string Player1, string Player2);
}