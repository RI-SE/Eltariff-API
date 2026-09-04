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
        string filePath = Path.Combine(_hostEnvironment.WebRootPath, "swagger/specification", "gridtariffapi-bundle.json");
        JsonNode json = JsonDataLoader.LoadApiSpecification(filePath);
        string? apiName = json["info"]?["title"]?.ToString();
        string? apiVersion = json["info"]?["version"]?.ToString();

        var additionalProperties = new Dictionary<string, object>
        {
            { "metadata1", "Value1" },
            { "metadata2", "Value2" }
        };

        var info = new InfoResponse
        {
            Name = apiName!,
            ApiVersion = apiVersion!,
            ImplementationVersion = "1.2.3",
            TariffDataLastUpdated = DateTimeOffset.Parse("2026-04-16T09:30:00+01:00"),
            Operator = "The Grid Company AB",
            TimeZone = "Europe/Stockholm",
            IdentityProviderUrl = "https://idp.gridcompany.se/oath2/token",
            AdditionalProperties = additionalProperties
        };

        await Task.CompletedTask;
        return Ok(info);
    }

    public override async Task<ActionResult<PricesResponse>> GetPrices(
        [FromQuery] Guid? componentId,
        [FromQuery] string? product,
        [FromQuery] DateOnly? date,
        [FromQuery] DateTimeOffset? fromIncluding,
        [FromQuery] DateTimeOffset? toExcluding,
        [FromQuery] string? resolution)
    {
        if (componentId == null || componentId == Guid.Empty)
        {
            if (product == null || product == string.Empty)
            {
                return BadRequest("Must specify either componentId or product.");
            }
            else if (!product.Equals("ProductCode3"))
            {
                return NotFound($"Found no price list related to product {product}");
            }
        }
        else if (!componentId.Equals(Guid.Parse("e33307b6-77b2-4d7d-b33f-908d2cc9ebbb")))
        {
            return NotFound($"Found no price list related to componentId {componentId}");
        }

        if (resolution != null && resolution != "PT1H" && resolution != "PT15M")
        {
            return BadRequest($"Invalid resolution '{resolution}'. Supported values are PT1H and PT15M.");
        }

        bool quarterHourly = resolution == "PT15M";

        DateTime now = DateTime.Now;
        DateTime today = now.Date;
        DateTimeOffset from = fromIncluding ?? today;
        DateTimeOffset to = toExcluding ?? from.AddDays(7);
        if (date is DateOnly d)
        {
            from = d.ToDateTime(TimeOnly.MinValue);
            to = from.AddDays(1);
        }

        var timespan = to - from;
        int numberOfHours = timespan.Days * 24 + timespan.Hours;
        DateTime tomorrow = today.AddDays(1);

        List<PriceListEntry> actual = [];
        List<PriceListEntry> preview = [];
        for (int i = 0; i < numberOfHours; i++)
        {
            DateTimeOffset start = from.AddHours(i);
            PriceListEntry price = new()
            {
                Created = from.Date.AddHours(-12),
                Start = start,
                End = start.AddHours(1),
                PriceExVat = i * 0.1m,
                PriceIncVat = i * 0.1m * 1.25m,
            };

            if (start.Date <= tomorrow.Date && now.Hour >= 12)
            {
                actual.Add(price);
            }
            else
            {
                preview.Add(price);
            }
        }

        if (quarterHourly)
        {
            actual = SplitIntoQuarterHours(actual);
            preview = SplitIntoQuarterHours(preview);
        }

        PricesResponse response = new()
        {
            Currency = "SEK",
            Unit = "kW",
            Actual = actual,
            Preview = preview
        };

        await Task.CompletedTask;
        return Ok(response);
    }

    private static List<PriceListEntry> SplitIntoQuarterHours(List<PriceListEntry> hourlyPrices)
    {
        List<PriceListEntry> quarterHourlyPrices = new(hourlyPrices.Count * 4);
        foreach (PriceListEntry hourlyPrice in hourlyPrices)
        {
            for (int quarter = 0; quarter < 4; quarter++)
            {
                DateTimeOffset start = hourlyPrice.Start.AddMinutes(quarter * 15);
                quarterHourlyPrices.Add(new PriceListEntry
                {
                    Created = hourlyPrice.Created,
                    Start = start,
                    End = start.AddMinutes(15),
                    PriceExVat = hourlyPrice.PriceExVat,
                    PriceIncVat = hourlyPrice.PriceIncVat,
                });
            }
        }

        return quarterHourlyPrices;
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
