namespace SwaggerUI;

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
}
