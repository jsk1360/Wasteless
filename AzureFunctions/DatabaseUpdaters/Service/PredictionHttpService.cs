using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WastelessFunction.Data;
using System.Net;
using Microsoft.Extensions.Logging;

namespace WastelessFunction.Service
{
    public class PredictionHttpService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _log;

        public PredictionHttpService(HttpClient httpClient, ILogger log)
        {
            _httpClient = httpClient;
            _log = log;
        }
        public async Task<IEnumerable<WasteJSON>> GetPredictions(int date)
        {
            
            _log.LogInformation($"Timer trigger PredictionUpdate-function started http request {_httpClient.BaseAddress + "pred"} at: {DateTime.Now}");
            
            var response = await _httpClient.GetAsync(_httpClient.BaseAddress + string.Format("pred?date={0}", date));

            response.EnsureSuccessStatusCode();
            
            await using var responseStream = await response.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<Prediction>(responseStream);

            _log.LogInformation($"Timer trigger PredictionUpdate-function Executed http request {_httpClient.BaseAddress + "pred"} at: {DateTime.Now}");
                
            return result.Data;
               

        }
    }
}
