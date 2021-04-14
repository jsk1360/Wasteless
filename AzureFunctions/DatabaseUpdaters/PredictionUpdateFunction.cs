using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WastelessFunction.Service;
using System.Threading.Tasks;

namespace WastelessFunction
{
    public class PredictionUpdateFunction
    {
        [FunctionName("PredictionUpdate")]
        //0 30 3 * * 1-5 at 3:30 AM every weekday
        //0-6 every day
        public async Task Run([TimerTrigger("%schedule%")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"Timer trigger PredictionUpdate-function started at: {DateTime.Now}");

            try
            {
                string url = Environment.GetEnvironmentVariable($"PredictionHttpUrlBase");
                string dbConnection = Environment.GetEnvironmentVariable($"WasteDB");

                var httpClient = new HttpClient
                { 
                    BaseAddress = new Uri(url)
                };
                
                var predictionHttpService = new PredictionHttpService(httpClient,log);

                               
                var wasteService = new WasteService(dbConnection, predictionHttpService, log);
                //new DateTime(2020, 12, 14)); 
                var waste = await wasteService.GetWeekWaste(DateTime.Today.AddDays(1));

                await wasteService.MergeWaste(waste);

                log.LogInformation($"Timer trigger PredictionUpdate-function executed at: {DateTime.Now}");

            }
            catch (Exception ex)
            {
                log.LogError(ex, "Timer trigger PredictionUpdate-function terminated unexpectedly");
            }
        }


    }

}

