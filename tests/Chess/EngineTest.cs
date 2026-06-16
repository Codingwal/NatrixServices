using NatrixServices.Chess.Core.Engine;
using NatrixServices.Chess.Core.Entities;
using NatrixServices.Chess.Presentation.DTOs;

namespace NatrixServices.Test.Chess;

public class EngineTest
{
    [Theory]
    [InlineData(Fen.DefaultFen, "e2", "e4", true)] // Pawn double move
    [InlineData(Fen.DefaultFen, "e2", "e3", true)] // Pawn single move
    [InlineData(Fen.DefaultFen, "e2", "e5", false)] // Pawn triple move
    [InlineData(Fen.DefaultFen, "a1", "a2", false)] // Rook captures its own pawn
    [InlineData(Fen.DefaultFen, "a7", "a6", false)] // Black pawn moves
    [InlineData(Fen.DefaultFen, "b1", "a3", true)] // Knight jumps
    [InlineData(Fen.DefaultFen, "a1", "b3", false)] // Rook jumps like knight
    public void TestMoves(string fen, string from, string to, bool expectedResult)
    {
        Assert.True(Fen.FenToBoard(fen).TryGetValue(out var board, out var error),
            $"Failed to parse fen \"{fen}\". Error: {error}");

        ChessEngine engine = new();

        MoveDTO moveDTO = new(from, to);
        Assert.True(moveDTO.ToMove().TryGetValue(out var move, out error),
            $"Failed to convert moveDTO to move. Error: {error}");

        bool legal = engine.IsMoveLegal(board, move);

        Assert.Equal(legal, expectedResult);
    }
}
