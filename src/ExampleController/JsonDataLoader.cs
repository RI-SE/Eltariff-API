using System.Text.Json;
using System.Text.Json.Nodes;

namespace ExampleController;

public static class JsonDataLoader
{
    private static readonly JsonSerializerOptions OPTIONS = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static T LoadResponseData<T>(string responseFile) where T : class
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "Data", responseFile);
        var json = File.ReadAllText(filePath);
        var items = JsonSerializer.Deserialize<T>(json, OPTIONS);
        if (items != null)
        {
            return items;
        }
        throw new JsonException($"Failed reading json file {filePath}");
    }

    public static JsonNode LoadApiSpecification(string filePath)
    {
        var content = File.ReadAllText(filePath);
        JsonNode? document = JsonNode.Parse(content);
        if (document != null)
        {
            return document;
        }
        throw new ArgumentNullException(nameof(document));
    }
}
