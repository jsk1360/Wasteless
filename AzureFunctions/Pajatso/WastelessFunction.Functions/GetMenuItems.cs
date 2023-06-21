using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using WastelessFunction.Functions.MenuApi;
using JsonSerializer = System.Text.Json.JsonSerializer;


namespace WastelessFunction.Functions;

public class GetMenus
{
    private readonly MenuService _menuService;

    public GetMenus(MenuService menuService)
    {
        _menuService = menuService;
    }

    [FunctionName("GetMenus")]
    public async Task<HttpResponseMessage> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
        HttpRequest req, ILogger log)
    {
        log.LogInformation("C# HTTP trigger function processed a request.");

        var dateParsed = DateOnly.TryParse(req.Query["date"], out var date);

        if (!dateParsed)
        {
            return new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("Please pass a valid date in the query string")
            };
        }

        var location = req.Query["location"];

        if (string.IsNullOrEmpty(location))
        {
            return new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("Please pass a valid location in the query string")
            };
        }

        var results = await _menuService.GetMenuForDay(location, date);
        var options = new JsonSerializerOptions
        {
            Converters =
            {
                new JsonStringEnumConverter()
            }
        };
        var jsonToReturn = JsonSerializer.Serialize(results, options);

        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(jsonToReturn, Encoding.UTF8, "application/json")
        };
    }
}

public class GetMenuItems
{
    private readonly MenuService _menuService;

    public GetMenuItems(MenuService menuService)
    {
        _menuService = menuService;
    }

    [FunctionName("GetMenuItems")]
    public async Task<HttpResponseMessage> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
        HttpRequest req, ILogger log)
    {
        var location = req.Query["location"];

        if (string.IsNullOrEmpty(location))
        {
            return new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("Please pass a valid location in the query string")
            };
        }

        try
        {
            var results = await _menuService.GetAllMenuItems(location);
            var options = new JsonSerializerOptions
            {
                Converters =
                {
                    new JsonStringEnumConverter()
                }
            };

            var jsonToReturn = JsonSerializer.Serialize(results, options);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonToReturn, Encoding.UTF8, "application/json")
            };
        }
        catch (Exception e)
        {
            log.LogError(e, "Could not get menu items");
            throw;
        }
    }
}