using RepoDb;
using Wasteless.Models;

namespace Wasteless.Infrastructure
{
    public static class MapperDefinitions
    {
        public static void Setup()
        {
            FluentMapper.Entity<Waste>().Table("[DW].[FacWastelessByItemNew]")
                .Column(x => x.DateId, "[DateSID]")
                .Column(x => x.MenuItemId, "[MenuItemSID]")
                .Column(x => x.LocationId, "[LocationSID]")
                .Column(x => x.ForecastMealTotal, "[Forecast_MealTotal]")
                .Column(x => x.ForecastSpecialMealCount, "[Forecast_SpecialMealCount]")
                .Column(x => x.ForecastMealCount, "[Forecast_MealCount]")
                .Column(x => x.ForecastProductionWasteKg, "[Forecast_ProductionWasteKg]")
                .Column(x => x.ForecastLineWasteKg, "[Forecast_LineWasteKg]")
                .Column(x => x.ForecastPlateWasteKg, "[Forecast_PlateWasteKg]")
                .Column(x => x.ForecastWasteTotalKg, "[Forecast_WasteTotalKg]");

            FluentMapper.Entity<FactMenuItem>().Table("[DW].[FacMenuItemNew]")
                .Identity(x => x.MenuSID)
                .Column(x => x.DateId, "[DateSID]")
                .Column(x => x.MenuItemId, "[MenuItemSID]")
                .Column(x => x.LocationId, "[LocationSID]")
                .Column(x => x.Menu, "[Menu]");

            FluentMapper.Entity<MenuItem>().Table("[DW].[DimMenuItemNew]")
                .Identity(x => x.Id);
            
            FluentMapper.Entity<Location>().Table("[DW].[DimLocation]")
                .Primary(x => x.Id)
                .Column(x => x.Id, "[LocationSID]")
                .Column(x => x.Name, "[LocationName]");
        }
    }
}