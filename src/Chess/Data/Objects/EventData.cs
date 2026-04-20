global using EventId = string;
using System.Text.Json.Serialization;

namespace NatrixServices.Chess.Data;

public class EventData : IIdentifiable<EventId>
{
    public EventId Id { get; set; } = EventId.Empty;
    public required string Name { get; set; }
    public required string Description { get; set; }

    public required int MaxPlayerCount { get; set; }
    public List<string> Players { get; set; } = [];

    public List<GameId> Games { get; set; } = [];

    public required int TimePerPlayer { get; set; } // In minutes

    public required EventDetails Details { get; set; }
}


[JsonPolymorphic(TypeDiscriminatorPropertyName = "Type")]
[JsonDerivedType(typeof(TournamentDetails), "Tournament")]
[JsonDerivedType(typeof(SeasonDetails), "Season")]
public abstract class EventDetails { }

public class TournamentDetails : EventDetails
{
    public int LayerCount { get; set; }
}

public class SeasonDetails : EventDetails
{

}