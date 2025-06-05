#nullable enable

using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;

public class OpenApiTools()
{
    public static string GetVersion(string openApiFilePath)
    {
        try
        {
            var openApiStream = File.ReadAllText(openApiFilePath);
            var document = JsonNode.Parse(openApiStream);
            string? apiVersion = document?["info"]?["version"]?.ToString();

            ArgumentException.ThrowIfNullOrEmpty(apiVersion, "info.version");

            return apiVersion;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: Failed to read OpenAPI version from file {openApiFilePath}: {ex.Message}");
            throw;
        }
    }

    public static void EditFile(string filePath)
    {
        string jsonText = File.ReadAllText(filePath);
        if (JsonNode.Parse(jsonText) is JsonObject root)
        {
            string version = GetVersion(filePath);
            if (filePath.EndsWith("-wip.json") && root.TryGetPropertyValue("info", out var info) && info is JsonObject infoObj)
            {
                File.Move(filePath, filePath.Replace("-wip", $"-v123-wip"));
            }
        }
        else
        {
            Console.WriteLine($"Skipping {filePath} - Not a valid JSON object.");
        }
    }
}
