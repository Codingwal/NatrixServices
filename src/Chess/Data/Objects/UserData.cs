using Microsoft.EntityFrameworkCore;
using NatrixServices.Chess.API; // TODO: Remove this dependency

namespace NatrixServices.Chess.Data;

public class UserData : UserDataBase
{
    public bool TitleHolder { get; set; } = false;
    public int SeasonsWon { get; set; } = 0;
    public int TournamentsWon { get; set; } = 0;
    public UserStatsDTO Stats { get; set; } = new();
}