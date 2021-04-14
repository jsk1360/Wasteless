using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using WastelessFunction.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Dapper;
using Microsoft.Extensions.Logging;
//using RepoDb;

namespace WastelessFunction.Service
{
    public class WasteService
    {
        private readonly PredictionHttpService _predictionHttpService;
        private readonly string _connectionString;
        private readonly ILogger _log;
        public WasteService(string connectionString, PredictionHttpService predictionHttpService, ILogger log)
        {
            _predictionHttpService = predictionHttpService;
            _connectionString = connectionString;
            _log = log;
        }
        public async Task<IEnumerable<WasteDB>> GetWeekWaste(DateTime date)
        {
            //next date
            var startDateSid = Convert.ToInt32(date.ToString("yyyyMMdd"));
                     
            //get predictions from http
            var predictions = await _predictionHttpService.GetPredictions(startDateSid);

            //get min and max date
            startDateSid = predictions.Min<WasteJSON>(d => d.DateSid);
            var endDateSid = predictions.Max<WasteJSON>(d => d.DateSid);
                        

            await using var connection = new SqlConnection(_connectionString);

            _log.LogInformation($"Timer trigger ForecastUpdate-function start to open connection to Wasteless-database at: {DateTime.Now}");
            await connection.OpenAsync();
            _log.LogInformation($"Timer trigger ForecastUpdate-function opened connection to Wasteless-database at: {DateTime.Now}");

            //get rows from database from dateperiod
            _log.LogInformation($"Timer trigger ForecastUpdate-function start to get data from Wasteless-database at: {DateTime.Now}");
            var waste = await connection.QueryAsync<WasteDB>("SELECT * FROM DW.FacWasteless WHERE DateSID >= @startDate AND DateSID <= @endDate;", 
                        new { startDate = startDateSid, endDate = endDateSid });
            _log.LogInformation($"Timer trigger ForecastUpdate-function data got from Wasteless-database at: {DateTime.Now}");

            await connection.CloseAsync();

            //add all data to waste list
            var wasteAll= waste.SetPredictions(predictions);
                        
            var wastes = new List<WasteDB>();

            foreach (var wasteDBto in wasteAll)
            {
                wastes.Add(wasteDBto);
            }

            return wastes;
        }
        public async Task MergeWaste(IEnumerable<WasteDB> wastes)
        {
            await using var connection = new SqlConnection(_connectionString);

            //merge data to DW.FacWasteless-table
            const string sql = @"MERGE INTO DW.FacWasteless AS TARGET
                                USING (VALUES
                                    (@WasteSID, @DateSID, @LocationSID, @Forecast_SpecialMealCount, @Forecast_MealTotal,
                                                @Forecast_ProductionWasteKg, @Forecast_LineWasteKg, @Forecast_PlateWasteKg,
                                                GETDATE() AT TIME ZONE 'UTC' AT TIME ZONE 'E. Europe Standard Time')
                                    ) AS SOURCE (WasteSID, DateSID, LocationSID, Forecast_SpecialMealCount, Forecast_MealTotal,
                                                Forecast_ProductionWasteKg, Forecast_LineWasteKg, Forecast_PlateWasteKg, ModifiedTime)
                                    ON (Target.WasteSID = Source.WasteSID)
                                WHEN MATCHED THEN
                                        UPDATE SET
                                        TARGET.Forecast_SpecialMealCount = SOURCE.Forecast_SpecialMealCount,
                                        TARGET.Forecast_MealTotal = SOURCE.Forecast_MealTotal,
                                        TARGET.Forecast_ProductionWasteKg = SOURCE.Forecast_ProductionWasteKg,
                                        TARGET.Forecast_LineWasteKg = SOURCE.Forecast_LineWasteKg,
                                        TARGET.Forecast_PlateWasteKg = SOURCE.Forecast_PlateWasteKg,
                                        TARGET.ModifiedTime = SOURCE.ModifiedTime
                                WHEN NOT MATCHED THEN
                                        INSERT (DateSID, LocationSID, Forecast_SpecialMealCount, Forecast_MealTotal,
                                                Forecast_ProductionWasteKg, Forecast_LineWasteKg, Forecast_PlateWasteKg, ModifiedTime)
                                        VALUES (SOURCE.DateSID, SOURCE.LocationSID, SOURCE.Forecast_SpecialMealCount, SOURCE.Forecast_MealTotal,
                                                SOURCE.Forecast_ProductionWasteKg, SOURCE.Forecast_LineWasteKg, SOURCE.Forecast_PlateWasteKg, SOURCE.ModifiedTime);";

            _log.LogInformation($"Timer trigger ForecastUpdate-function start to open connection to Wasteless-database at: {DateTime.Now}");
            await connection.OpenAsync();
            _log.LogInformation($"Timer trigger ForecastUpdate-function opened connection to Wasteless-database at: {DateTime.Now}");

            //loop all wastes and execute merge-sql
            _log.LogInformation($"Timer trigger ForecastUpdate-function start to add data to FacWasteless-table at: {DateTime.Now}");
            foreach (var waste in wastes)
            {
                await connection.ExecuteAsync(sql, new {
                    WasteSID = waste.WasteSID,
                    DateSID = waste.DateSID,
                    LocationSID = waste.LocationSID,
                    Forecast_SpecialMealCount = waste.Forecast_SpecialMealCount,
                    Forecast_MealTotal = waste.Forecast_MealTotal,
                    Forecast_ProductionWasteKg = waste.Forecast_ProductionWasteKg,
                    Forecast_LineWasteKg = waste.Forecast_LineWasteKg,
                    Forecast_PlateWasteKg = waste.Forecast_PlateWasteKg,
                });
            }
            _log.LogInformation($"Timer trigger ForecastUpdate-function executed to add data to FacWasteless-table at: {DateTime.Now}");

            await connection.CloseAsync();
        }
    }
   
}
