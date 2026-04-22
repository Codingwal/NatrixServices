using NatrixServices.Chess.Data;

namespace NatrixServices.Chess.Management;

public class EventNotFoundException() : Exception("Event not found"), INotFoundException;
public class AlreadyEventParticipantException() : Exception("You are already a participant of this event"), IConflictException;

public struct EventBaseConfig
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required int MaxPlayerCount { get; set; }
    public required int TimePerPlayer { get; set; }
}

public interface IEventManager
{
    Task<List<EventData>> GetAllEventsAsync();

    Task<EventData> GetEventAsync(EventId eventId);

    Task CreateTournamentAsync(EventBaseConfig config);
    Task CreateSeasonAsync(EventBaseConfig config);
    Task JoinEventAsync(EventId eventId, string username);

    Task StartEventAsync(EventId eventId);
}

public class EventManager(IItemStorage<EventData, EventId> EventStorage, IGameManager GameManager) : IEventManager
{
    public async Task<List<EventData>> GetAllEventsAsync() => await EventStorage.GetAllItemsAsync();

    public async Task<EventData> GetEventAsync(EventId eventId) => await EventStorage.GetItemAsync(eventId) ?? throw new EventNotFoundException();

    public async Task CreateTournamentAsync(EventBaseConfig config)
    {
        EventId eventId = Utility.GenerateId();
        EventData eventData = new()
        {
            Id = eventId,
            Name = config.Name,
            Description = config.Description,
            MaxPlayerCount = config.MaxPlayerCount,
            TimePerPlayer = config.TimePerPlayer,
            Details = new TournamentDetails()
        };
        await EventStorage.AddItemAsync(eventData);
    }

    public async Task CreateSeasonAsync(EventBaseConfig config)
    {
        EventId eventId = Utility.GenerateId();
        EventData eventData = new()
        {
            Id = eventId,
            Name = config.Name,
            Description = config.Description,
            MaxPlayerCount = config.MaxPlayerCount,
            TimePerPlayer = config.TimePerPlayer,
            Details = new SeasonDetails()
        };
        await EventStorage.AddItemAsync(eventData);
    }

    public async Task JoinEventAsync(EventId eventId, string username)
    {
        EventData eventData = await GetEventAsync(eventId);

        if (eventData.Players.Contains(username))
            throw new AlreadyEventParticipantException();

        eventData.Players.Add(username);
        await EventStorage.UpdateItemAsync(eventData);
    }

    public async Task StartEventAsync(string eventId)
    {
        EventData eventData = await GetEventAsync(eventId);

        switch (eventData.Details)
        {
            case TournamentDetails tournamentDetails:
                await SetupTournamentAsync(eventData, tournamentDetails);
                break;
            case SeasonDetails seasonDetails:
                await SetupSeasonAsync(eventData, seasonDetails);
                break;
        }
    }

    private async Task SetupTournamentAsync(EventData eventData, TournamentDetails tournamentDetails)
    {
        int playerCount = eventData.Players.Count;
        int layerCount = (int)Math.Ceiling(Math.Log2(playerCount));

        var shuffledPlayers = ShufflePlayers(eventData.Players);

        // Setup layers and games
        for (int layer = 0; layer < layerCount; layer++)
        {
            int playersInLayer = playerCount / (int)Math.Pow(2, layer);
            int gamesInLayer = playersInLayer / 2;

            for (int i = 0; i < gamesInLayer; i++)
            {
                string indexStr = (i + 1) switch { 1 => "first", 2 => "second", 3 => "third", _ => $"${i + 1}th" };
                string layerStr = gamesInLayer switch { 1 => "final", 2 => "semi-final", 4 => "quarter-final", _ => $"round of {playersInLayer}" };

                GameId gameId = await GameManager.CreateGameAsync($"{eventData.Name} | {indexStr} {layerStr}", isPublic: true, eventData.TimePerPlayer);
                eventData.Games.Add(gameId);
            }
        }

        // Join first-layer games
        for (int i = 0; i < playerCount; i++)
        {
            GameId gameId = eventData.Games[i / 2];
            await GameManager.JoinGameAsync(shuffledPlayers[i], gameId);
        }
    }

    private async Task SetupSeasonAsync(EventData eventData, SeasonDetails seasonDetails)
    {
        for (int i = 0; i < eventData.Players.Count; i++)
        {
            for (int j = i + 1; j < eventData.Players.Count; j++)
            {
                string player1 = eventData.Players[i];
                string player2 = eventData.Players[j];

                GameId gameId = await GameManager.CreateGameAsync($"{eventData.Name} | Regular match", isPublic: true, eventData.TimePerPlayer);
                eventData.Games.Add(gameId);
                await GameManager.JoinGameAsync(player1, gameId);
                await GameManager.JoinGameAsync(player2, gameId);
            }
        }
    }

    private List<string> ShufflePlayers(List<string> players)
    {
        Random rnd = new();
        List<string> shuffledPlayers = players.OrderBy(_ => rnd.Next()).ToList();
        return shuffledPlayers;
    }
}