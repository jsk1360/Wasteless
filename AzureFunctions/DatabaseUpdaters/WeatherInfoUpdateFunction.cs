using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using WastelessFunction.Data;

namespace WastelessFunction
{
    public class WeatherInfoUpdateFunction
    {
        [FunctionName("WeatherInfoUpdate")]
        public static async Task RunAsync([TimerTrigger("%weatherschedule%")] TimerInfo myTimer, ILogger log)
        {
            var apiKey = Environment.GetEnvironmentVariable($"WeatherApiKey");
            var apiUrl = Environment.GetEnvironmentVariable($"WeatherApiUrl");
            var connectionString = Environment.GetEnvironmentVariable($"WasteDB");

            if (string.IsNullOrEmpty(apiKey))
                throw new Exception("ApiKey not set");

            if (string.IsNullOrEmpty(apiUrl))
                throw new Exception("ApiUrl not set");

            if (string.IsNullOrEmpty(connectionString))
                throw new Exception("ConnectionString not set");

            var locations = new List<Location>();

            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(apiUrl)
            };

            var weathers = new List<Weather>();
            foreach (var location in locations)
            {
                try
                {
                    var response = await httpClient.GetAsync(
                        $"?lat={location.Latitude}&lon={location.Longitude}&exclude=current,minutely,hourly,alerts&appid={apiKey}&units=metric&lang=fi");

                    response.EnsureSuccessStatusCode();

                    await using var responseStream = await response.Content.ReadAsStreamAsync();
                    var result = await JsonSerializer.DeserializeAsync<WeatherResult>(responseStream);

                    weathers.AddRange(result.Daily.Select(day => new Weather
                    {
                        LocationSid = location.LocationSid,
                        DateSid = Convert.ToInt32(DateTimeOffset.FromUnixTimeSeconds(day.Dt).Date.ToString("yyyyMMdd")),
                        Cloudiness = Convert.ToInt32(Math.Round(day.Clouds / 12.5, MidpointRounding.ToEven)),
                        Temperature = day.Temp.Day,
                        WindSpeed = day.WindSpeed
                    }));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            await connection.ExecuteAsync(
                @" MERGE dw.FacWeatherInfo AS TARGET
                    USING (SELECT @DateSid,
                                  @Cloudiness,
                                  @Temperature,
                                  @WindSpeed,
                                  @LocationSid,
                                  GETDATE() AT TIME ZONE 'UTC' AT TIME ZONE 'E. Europe Standard Time') AS SOURCE (DateSID,
                                                                                                                  Cloudiness,
                                                                                                                  Temperature,
                                                                                                                  WindSpeed,
                                                                                                                  LocationSID,
                                                                                                                  ModifiedTime)
                    ON (TARGET.DateSID = SOURCE.DateSID and TARGET.LocationSID = SOURCE.LocationSID)
                    WHEN MATCHED AND
                        (TARGET.Cloudiness  <> SOURCE.Cloudiness OR
                         TARGET.Temperature <> SOURCE.Temperature OR
                         TARGET.WindSpeed   <> SOURCE.WindSpeed)
                        THEN
                        UPDATE
                        SET TARGET.Cloudiness   = SOURCE.Cloudiness,
                            TARGET.Temperature  = SOURCE.Temperature,
                            TARGET.WindSpeed    = SOURCE.WindSpeed,
                            TARGET.ModifiedTime = SOURCE.ModifiedTime
                    WHEN NOT MATCHED THEN
                        insert (datesid, cloudiness, temperature, windspeed, locationsid, modifiedtime)
                        values (SOURCE.DateSID, SOURCE.Cloudiness, SOURCE.Temperature, SOURCE.WindSpeed, SOURCE.LocationSID,
                                null);",
                weathers);
        }
    }
}
