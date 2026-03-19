using System.Text.Json;

namespace NatrixServices;

public static class ConfigHandler
{
    private static JsonSerializerOptions jsonSerializerOptions = new() { WriteIndented = true };
    public static T LoadUserConfig<T>(NatrixUser user)
    {
        if (!TryLoadUserConfig(user, out T? userConfig))
            throw new Exception($"Config of type {typeof(T)} does not exist for user {user.UserId}");

        return userConfig!;
    }
    public static bool TryLoadUserConfig<T>(NatrixUser user, out T? userConfig)
    {
        UserConfig<T> config = LoadConfig<UserConfig<T>>(GetName<T>());

        return config.Data.TryGetValue(user.UserId, out userConfig);
    }

    public static Dictionary<NatrixUser, T> GetAllUserConfigs<T>()
    {
        UserConfig<T> config = LoadConfig<UserConfig<T>>(GetName<T>());

        return config.Data.ToDictionary(x => NatrixUser.GetUser(x.Key), x => x.Value);
    }

    public static void SetUserConfig<T>(NatrixUser user, T userConfig)
    {
        var config = File.Exists(GetName<T>()) ? LoadConfig<UserConfig<T>>(GetName<T>()) : UserConfig<T>.Default;

        config.Data[user.UserId] = userConfig;

        SaveConfig(config, GetName<T>());
    }

    // public static void InitConfig<T>()
    // {
    //     SaveConfig(UserConfig<T>.Default, GetName<T>());
    // }

    private static T? LoadConfig<T>(string configName)
    {
        string str = File.ReadAllText(configName);
        return JsonSerializer.Deserialize<T>(str);
    }

    private static void SaveConfig<T>(T config, string configName)
    {
        string str = JsonSerializer.Serialize(config, jsonSerializerOptions);
        File.WriteAllText(configName, str);
    }

    private static string GetName<T>()
    {
        return $"Config/{typeof(T).Name}.json";
    }

    private struct UserConfig<T>
    {
        public Dictionary<string, T> Data { get; set; }
        public static UserConfig<T> Default => new() { Data = [] };
    }
}

