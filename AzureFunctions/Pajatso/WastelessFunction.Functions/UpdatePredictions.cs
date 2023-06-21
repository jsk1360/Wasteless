using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using WastelessFunction.Lib.Predictions;

namespace WastelessFunction.Functions;

public class UpdatePredictions
{
    private readonly IPredictionsApi _predictionsApi;
    private readonly PredictionsUpdater _predictionsUpdater;

    public UpdatePredictions(IPredictionsApi predictionsApi, PredictionsUpdater predictionsUpdater)
    {
        _predictionsApi = predictionsApi;
        _predictionsUpdater = predictionsUpdater;
    }

    [FunctionName("UpdatePredictionsHttp")]
    public async Task<HttpResponseMessage> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
        HttpRequest req, ILogger log)
    {
        var tomorrow = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var data = await _predictionsApi.GetPrediction(tomorrow);

        var options = new JsonSerializerOptions
        {
            Converters =
            {
                new JsonStringEnumConverter()
            }
        };

        var jsonToReturn = JsonSerializer.Serialize(data, options);

        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(jsonToReturn, Encoding.UTF8, "application/json")
        };
    }

    [FunctionName("UpdatePredictionsTimer")]
    public async Task RunAsync2([TimerTrigger("%schedule%")] TimerInfo myTimer, ILogger log)
    {
        log.LogInformation($"C# Timer trigger function executed at: {DateTime.UtcNow}");

        var tomorrow = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var data = await _predictionsApi.GetPrediction(tomorrow);

        if (data is null) return;

        foreach (var datum in data.Data)
        {
            await _predictionsUpdater.UpdatePredictions(datum);
        }
    }
}