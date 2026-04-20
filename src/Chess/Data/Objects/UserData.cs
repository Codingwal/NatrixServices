using System.ComponentModel.DataAnnotations;
using NatrixServices.Chess.API; // TODO: Remove this dependency

namespace NatrixServices.Chess.Data;

public class UserData : IIdentifiable<string>
{
    [Key, Required, StringLength(8)]
    public string Username { get; set; } = string.Empty;
    public bool TitleHolder { get; set; } = false;
    public int SeasonsWon { get; set; } = 0;
    public int TournamentsWon { get; set; } = 0;
    public UserStatsDTO Stats { get; set; } = new();

    public string Id { get => Username; set => Username = value; }
}
