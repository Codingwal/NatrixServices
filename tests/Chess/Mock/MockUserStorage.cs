using System.Collections.Concurrent;
using NatrixServices.Chess.Application.Interfaces;
using NatrixServices.Chess.Core.Entities;

public class MockUserStorage : IUserStorage
{
    private ConcurrentDictionary<string, UserData> Users { get; } = new();

    public async Task AddUserAsync(UserData user)
    {
        Users[user.Username] = user;
    }

    public async Task<IEnumerable<UserData>> GetAllUsersAsync()
    {
        return Users.Values;
    }

    public async Task<UserData?> GetUserAsync(string username)
    {
        Users.TryGetValue(username, out var userData);
        return userData;
    }

    public async Task UpdateUserAsync(UserData user)
    {
        Users[user.Username] = user;
    }

    public void LogUsers()
    {
        foreach (var (username, userData) in Users)
        {
            // Customize this log output based on the actual properties inside your UserData class
            Console.WriteLine($"\"{username}\": {userData.SeasonsWon}, {userData.TournamentsWon}, {userData.Stats.GamesWon}");
        }
    }
}