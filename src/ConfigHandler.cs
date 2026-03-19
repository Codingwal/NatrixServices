using System.Text.Json;

namespace NatrixServices;

public static class DataHandler
{
    private static JsonSerializerOptions jsonSerializerOptions = new() { WriteIndented = true };
    public static T LoadUserData<T>(NatrixUser user)
    {
        if (!TryLoadUserData(user, out T? userData))
            throw new Exception($"Data of type {typeof(T)} does not exist for user {user.UserId}");

        return userData!;
    }
    public static bool TryLoadUserData<T>(NatrixUser user, out T? userData)
    {
        UserData<T> Data = LoadData<UserData<T>>(GetName<T>());

        return Data.Data.TryGetValue(user.UserId, out userData);
    }

    public static Dictionary<NatrixUser, T> GetAllUserDatas<T>()
    {
        UserData<T> Data = LoadData<UserData<T>>(GetName<T>());

        return Data.Data.ToDictionary(x => UserManager.GetUser(x.Key), x => x.Value);
    }

    public static void SetUserData<T>(NatrixUser user, T userData)
    {
        var Data = File.Exists(GetName<T>()) ? LoadData<UserData<T>>(GetName<T>()) : UserData<T>.Default;

        Data.Data[user.UserId] = userData;

        SaveData(Data, GetName<T>());
    }

    // public static void InitData<T>()
    // {
    //     SaveData(UserData<T>.Default, GetName<T>());
    // }

    private static T? LoadData<T>(string DataName)
    {
        string str = File.ReadAllText(DataName);
        return JsonSerializer.Deserialize<T>(str);
    }

    private static void SaveData<T>(T Data, string DataName)
    {
        string str = JsonSerializer.Serialize(Data, jsonSerializerOptions);
        File.WriteAllText(DataName, str);
    }

    private static string GetName<T>()
    {
        return $"Data/{typeof(T).Name}.json";
    }

    private struct UserData<T>
    {
        public Dictionary<string, T> Data { get; set; }
        public static UserData<T> Default => new() { Data = [] };
    }
}

