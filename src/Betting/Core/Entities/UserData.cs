namespace NatrixServices.Betting.Core.Entities;

public record UserData(string Username)
{
    public float Balance { get; set; } = 100;
}