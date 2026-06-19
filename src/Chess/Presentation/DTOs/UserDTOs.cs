using NatrixServices.Chess.Core.Entities;

namespace NatrixServices.Chess.Presentation.DTOs;

public record UserDataDTO
{
    public string Username { get; set; }
    public string Title { get; set; }
    public string TitleDescription { get; set; }
    public int SeasonsWon { get; set; }
    public int TournamentsWon { get; set; }
    public UserDataDTO(UserData userData)
    {
        Username = userData.Username;
        Title = userData.TitleHolder ? "Title holder" : userData.Stats.GetTitle().Item1;
        TitleDescription = userData.TitleHolder ? "The reigning champion" : userData.Stats.GetTitle().Item2;
        SeasonsWon = userData.SeasonsWon;
        TournamentsWon = userData.TournamentsWon;
    }
}