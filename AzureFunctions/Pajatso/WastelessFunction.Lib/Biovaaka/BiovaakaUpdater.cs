using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using WastelessFunction.Lib.Biovaaka.Models.Measurement;

namespace WastelessFunction.Lib.Biovaaka;

public class BiovaakaUpdater
{
    private readonly BiovaakaClient _client;
    private readonly string _connectionString;

    public BiovaakaUpdater(BiovaakaClient client, IOptions<ConnectionString> connectionStringOption)
    {
        _client = client;
        _connectionString = connectionStringOption.Value.WasteDb;
    }

    public async Task Update(DateOnly date)
    {
        var dateSid = Convert.ToInt32(date.ToString("yyyyMMdd"));
        
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var measurements = await _client.GetMeasurements(date, date);
        var productions = await _client.GetProduction(date, date);

        var groupedMeasurements = measurements.Where(m => m.ItemId is not null)
            .GroupBy(m => new { m.ItemId, m.ItemName }).ToList();
        var groupedProductions = productions.Where(m => m.ItemId is not null)
            .GroupBy(p => new { p.ItemId, p.ItemName }).ToList();

        var menuItems = (await connection.QueryAsync<int>(
            @"select menu.MenuItemSID
              from dw.FacMenuItemNew menu
              join dw.DimMenuItemNew item on menu.MenuItemSID = item.Id
              where DateSID = @date and LocationSID = @locationId",
            new { date = dateSid, locationId = 1 })).ToList();

        var productionWaste = measurements.Where(g => g.ItemSourceName == "Valmistus").Sum(g => g.MeasuredWeight);
        var plateWaste = measurements.Where(g => g.ItemSourceName == "Lautanen").Sum(g => g.MeasuredWeight);

        foreach (var menuItem in menuItems)
        {
            var group = groupedMeasurements.FirstOrDefault(g => g.Key.ItemId == menuItem);
            var produced = groupedProductions.FirstOrDefault(g => g.Key.ItemId == menuItem);
            
            var lineWaste = group?.Where(g => g.ItemSourceName == "Linjasto").Sum(g => g.MeasuredWeight);
            var producedAmount = produced?.Sum(p => p.MeasuredWeight) / 1000f;
            var lineWasteAmount = lineWaste.HasValue ? Math.Round(lineWaste.Value / 1000f, 2) : 0;

            var updateRow = new UpdateRow
            {
                DateSID = dateSid,
                MenuItemSID = menuItem,
                LineWasteKg = lineWasteAmount,
                PlateWasteKg = Math.Round(plateWaste / menuItems.Count / 1000f, 2),
                ProducedKg = producedAmount.HasValue ? Math.Round(producedAmount.Value, 2) : 0f,
                ProductionWasteKg = Math.Round(productionWaste / menuItems.Count / 1000f, 2),
            };

            await connection.ExecuteAsync(UpdateSql, updateRow);
        }
    }

    private class UpdateRow
    {
        public int MenuItemSID { get; set; }
        public int DateSID { get; set; }
        public double LineWasteKg { get; set; }
        public double PlateWasteKg { get; set; }
        public double? ProductionWasteKg { get; set; }
        public double? ProducedKg { get; set; }
        public int LocationSID { get; set; } = 1;
    }

    private const string UpdateSql = @"
            MERGE INTO DW.FacWastelessByItemNew AS TARGET
            USING (VALUES (@MenuItemSID, @DateSID, @LocationSID, @LineWasteKg, @PlateWasteKg, @ProductionWasteKg, @ProducedKg,
                           GETDATE() AT TIME ZONE 'UTC' AT TIME ZONE 'E. Europe Standard Time')
            ) AS SOURCE (MenuItemSID, DateSID, LocationSID, LineWasteKg, PlateWasteKg, ProductionWasteKg, ProducedKg, ModifiedTime)
            ON (Target.MenuItemSID = Source.MenuItemSID and
                Target.DateSID = Source.DateSID and
                Target.LocationSID = Source.LocationSID)
            WHEN MATCHED THEN
                UPDATE
                SET TARGET.LineWasteKg  = SOURCE.LineWasteKg,
                    TARGET.PlateWasteKg = SOURCE.PlateWasteKg,
                    TARGET.ProductionWasteKg = SOURCE.ProductionWasteKg,
                    TARGET.ProducedKg = SOURCE.ProducedKg,
                    TARGET.ModifiedTime = SOURCE.ModifiedTime
            WHEN NOT MATCHED THEN
                INSERT (MenuItemSID, DateSID, LocationSID, LineWasteKg, PlateWasteKg, ProductionWasteKg, ProducedKg, ModifiedTime)
                VALUES (SOURCE.MenuItemSID, SOURCE.DateSID, SOURCE.LocationSID,
                        SOURCE.LineWasteKg,
                        SOURCE.PlateWasteKg,
                        SOURCE.ProductionWasteKg,
                        SOURCE.ProducedKg,
                        SOURCE.ModifiedTime);";
}