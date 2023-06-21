using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using WastelessFunction.Functions.Predictions.Models;

namespace WastelessFunction.Lib.Predictions;

public class PredictionsUpdater
{
    private readonly string _connectionString;

    public PredictionsUpdater(IOptions<ConnectionString> options)
    {
        _connectionString = options.Value.WasteDb;
    }

    public async Task UpdatePredictions(Datum datum)
    {
        await MergeWaste(datum);
    }

    private async Task MergeWaste(Datum waste)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        const string sql = @"
            MERGE INTO DW.FacWastelessByItemNew AS TARGET
            USING (VALUES (@MenuItemSID, @DateSID, @LocationSID, @Forecast_MealTotal, @Forecast_ProductionWasteKg,
                           @Forecast_LineWasteKg,
                           @Forecast_PlateWasteKg,
                           GETDATE() AT TIME ZONE 'UTC' AT TIME ZONE 'E. Europe Standard Time')
            ) AS SOURCE (MenuItemSID, DateSID, LocationSID, Forecast_MealTotal, Forecast_ProductionWasteKg, Forecast_LineWasteKg,
                         Forecast_PlateWasteKg,
                         ModifiedTime)
            ON (Target.MenuItemSID = Source.MenuItemSID and
                Target.DateSID = Source.DateSID and
                Target.LocationSID = Source.LocationSID)
            WHEN MATCHED THEN
                UPDATE
                SET TARGET.Forecast_ProductionWasteKg = SOURCE.Forecast_ProductionWasteKg,
                    TARGET.Forecast_LineWasteKg       = SOURCE.Forecast_LineWasteKg,
                    TARGET.Forecast_PlateWasteKg      = SOURCE.Forecast_PlateWasteKg,
                    TARGET.Forecast_MealTotal         = SOURCE.Forecast_MealTotal,
                    TARGET.ModifiedTime               = SOURCE.ModifiedTime
            WHEN NOT MATCHED THEN
                INSERT (MenuItemSID, DateSID, LocationSID, Forecast_MealTotal, Forecast_ProductionWasteKg, Forecast_LineWasteKg,
                        Forecast_PlateWasteKg,
                        ModifiedTime)
                VALUES (SOURCE.MenuItemSID, SOURCE.DateSID, SOURCE.LocationSID, SOURCE.Forecast_MealTotal,
                        SOURCE.Forecast_ProductionWasteKg,
                        SOURCE.Forecast_LineWasteKg,
                        SOURCE.Forecast_PlateWasteKg,
                        SOURCE.ModifiedTime);
            ";

        await connection.ExecuteAsync(sql, new
        {
            MenuItemSID = waste.MenuItemSid,
            DateSID = waste.DateSid,
            LocationSID = waste.LocationSid,
            Forecast_ProductionWasteKg = waste.PredictedProductionWasteKg,
            Forecast_LineWasteKg = waste.PredictedLineWasteKg,
            Forecast_PlateWasteKg = waste.PredictedPlateWasteKg,
            Forecast_MealTotal = waste.PredictedMealTotal
        });
    }
}