using Microsoft.AspNetCore.Mvc;
using GeneratedController;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ExampleController;

public class GridTariffController : GeneratedControllerBase
{
    public override async Task<ActionResult<InfoResponse>> GetInfo()
    {
        JsonNode json = await JsonDataLoader.LoadApiSpecification("gridtariffapi-wip.json");
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
            LastUpdated = DateTime.Parse("2025-03-25"),
            IdentityProviderUrl = "https://idp.gridcompany.se/oath2/token",
            AdditionalProperties = additionalProperties
        };
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
        var tariffsResponse = await JsonDataLoader.LoadResponseDataAsync<TariffsResponse>("tariffs.json");
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
        return NotFound($"Tariff with id {id} was not found.");
    }

    public override async Task<ActionResult<TariffsResponse>> GetTariffs()
    {
        return await JsonDataLoader.LoadResponseDataAsync<TariffsResponse>("tariffs.json");
    }

    public override async Task<ActionResult<TariffsSearchResponse>> SearchTariffs([BindRequired, FromBody] TariffsSearchRequest body)
    {
        await Task.Delay(0);
        return StatusCode(StatusCodes.Status501NotImplemented, $"POST /tariffs/search is not implemented.");
    }
}