using System.Text.Json;

namespace CoffeeShop.Prowiders;

public static class JsonProvider
{
    private static readonly JsonSerializerOptions _options;

    static JsonProvider()
    {
        _options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
        };
    }
    public static string Serialize<T>(T obj) => JsonSerializer.Serialize(obj, _options);
    public static T Deserialize<T>(string jsonString) => JsonSerializer.Deserialize<T>(jsonString, _options);
}