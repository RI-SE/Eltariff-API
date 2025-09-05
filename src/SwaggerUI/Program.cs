using SwaggerUI;

string wwwrootSpecificationDir = Path.Combine("wwwroot", "swagger", "specification");
Dictionary<string, string> swaggerUISpecifications = [];

foreach (var filePath in Directory.GetFiles(wwwrootSpecificationDir))
{
    string version = OpenApiTools.GetVersion(filePath);
    swaggerUISpecifications[version] = Path.GetFileName(filePath);
}

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseStaticFiles();
app.UseSwagger();
app.UseSwaggerUI(o =>
{
    KeyValuePair<string, string>? wipSpecification = null;
    bool isLatest = true;
    foreach (var (version, uiSpecificationFileName) in swaggerUISpecifications.OrderBy(x => x.Key).Reverse())
    {
        if (uiSpecificationFileName.Contains("-wip"))
        {
            wipSpecification = new KeyValuePair<string, string>(version, uiSpecificationFileName);
        }
        else if (isLatest)
        {
            o.SwaggerEndpoint($"specification/{uiSpecificationFileName}", $"{version} (latest)");
            isLatest = false;
        }
        else
        {
            o.SwaggerEndpoint($"specification/{uiSpecificationFileName}", version);
        }
    }

    if (wipSpecification != null) {
        string wipVersion = wipSpecification.Value.Key;
        string wipSpecificationFileName = wipSpecification.Value.Value;
        o.SwaggerEndpoint($"specification/{wipSpecificationFileName}", $"{wipVersion} (wip)");
    }

    o.RoutePrefix = "swagger";
    o.DocumentTitle = "Grid Tariff API";
    o.InjectStylesheet("ui/custom.css");
    o.InjectJavascript("ui/custom.js");
});

app.MapControllers();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.Run();
