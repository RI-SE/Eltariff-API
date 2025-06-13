using Microsoft.AspNetCore.Mvc;
using GeneratedController;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Hosting;

namespace ExampleController;

public class GridTariffController(IWebHostEnvironment hostEnvironment) : GeneratedControllerBase
{
    private IWebHostEnvironment _hostEnvironment = hostEnvironment;

    public override async Task<ActionResult<InfoResponse>> GetInfo()
    {
        string filePath = Path.Combine(_hostEnvironment.WebRootPath, "swagger/specification", "gridtariffapi-wip.json");
        JsonNode json = JsonDataLoader.LoadApiSpecification(filePath);
        string? apiName = json["info"]?["title"]?.ToString();
        string? apiVersion = json["info"]?["version"]?.ToString();
        string implementationRevision = "r1";

        var additionalProperties = new Dictionary<string, object>
        {
            { "metadata1", "Value1" },
            { "metadata2", "Value2" }
        };

        var info = new InfoResponse
        {
            Name = apiName,
            ApiVersion = apiVersion,
            ImplementationVersion = $"{apiVersion}-{implementationRevision}",
            Operator = "The Grid Company AB",
            IdentityProviderUrl = "https://idp.gridcompany.se/oath2/token",
            LastUpdated = DateTime.Parse("2025-06-13"),
            AdditionalProperties = additionalProperties
        };

        await Task.CompletedTask;
        return Ok(info);
    }

    public override async Task<ActionResult<PricesResponse>> GetPrices(
        [BindRequired] Guid componentId,
        [FromQuery] DateTime? fromIncluding,
        [FromQuery] DateTime? toExcluding)
    {

        var today = DateTime.Today;
        List<PriceListEntry> prices = [];
        for (int i = 0; i < 24; i++)
        {
            prices.Add(new PriceListEntry
            {
                Timestamp = today.AddHours(i),
                PriceExVat = i * 0.1m,
                PriceIncVat = i * 0.1m * 1.25m,
                Currency = "SEK"
            });
        }

        PriceList priceList = new()
        {
            ComponentId = componentId,
            Resolution = "PT1H",
            Prices = prices
        };

        PricesResponse response = new()
        {
            PriceList = priceList
        };

        await Task.CompletedTask;
        return Ok(response);
    }

    public override async Task<ActionResult<TariffResponse>> GetTariffById([BindRequired] Guid id)
    {
        var tariffsResponse = JsonDataLoader.LoadResponseData<TariffsResponse>("tariffs.json");
        foreach (var tariff in tariffsResponse.Tariffs)
        {
            if (tariff.Id == id)
            {
                return new TariffResponse
                {
                    Tariff = tariff,
                };
            }
        }

        await Task.CompletedTask;
        return NotFound($"Tariff with id {id} was not found.");
    }

    public override async Task<ActionResult<TariffsResponse>> GetTariffs()
    {
        await Task.CompletedTask;
        return JsonDataLoader.LoadResponseData<TariffsResponse>("tariffs.json");
    }

    public override async Task<ActionResult<TariffsSearchResponse>> SearchTariffs([BindRequired, FromBody] TariffsSearchRequest body)
    {
        await Task.CompletedTask;
        return StatusCode(StatusCodes.Status501NotImplemented, $"POST /tariffs/search is not implemented.");
    }
}