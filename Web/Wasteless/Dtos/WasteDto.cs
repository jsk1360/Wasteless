using System;
using Wasteless.Models;

namespace Wasteless.Dtos
{
    public class WasteDto
    {
        public WasteDto(string location, string city, Menu? menu, Waste waste)
        {
            Id = waste.WasteSID;
            LocationId = waste.LocationId;
            Location = location;
            City = city;
            Menu = menu?.ToDto() ?? MenuHelpers.MenuNotFound();
            Date = waste.Date;
            ForecastMealTotal = waste.ForecastMealTotal;
            ForecastLineWasteKg = waste.ForecastLineWasteKg.HasValue
                ? Math.Round(waste.ForecastLineWasteKg.Value, 2)
                : null;
            ForecastProductionWasteKg = waste.ForecastProductionWasteKg.HasValue
                ? Math.Round(waste.ForecastProductionWasteKg.Value, 2)
                : null;
            ForecastPlateWasteKg = waste.ForecastPlateWasteKg.HasValue
                ? Math.Round(waste.ForecastPlateWasteKg.Value, 2)
                : null;

            ForecastWasteTotalKg = waste.ForecastWasteTotalKg.HasValue
                ? Math.Round(waste.ForecastWasteTotalKg.Value, 2)
                : null;

            MealTotal = waste.MealTotal;
            LineWasteKg = waste.LineWasteKg;
            ProductionWasteKg = waste.ProductionWasteKg;
            PlateWasteKg = waste.PlateWasteKg;
            MealCountReserved = waste.MealCountReserved;
            SpecialMealCount = waste.SpecialMealCount;
        }

        public int Id { get; set; }
        public int LocationId { get; set; }
        public string Location { get; set; }
        public string City { get; set; }
        public DateTime Date { get; set; }
        public MenuDto Menu { get; set; }

        public double? ForecastMealTotal { get; set; }
        public double? ForecastLineWasteKg { get; set; }
        public double? ForecastProductionWasteKg { get; set; }
        public double? ForecastPlateWasteKg { get; set; }
        public double? ForecastWasteTotalKg { get; set; }
        public int? MealTotal { get; set; }
        public int? SpecialMealCount { get; set; }

        public double? LineWasteKg { get; set; }
        public double? ProductionWasteKg { get; set; }
        public double? PlateWasteKg { get; set; }

        public int? MealCountReserved { get; set; }
    }

    public record MenuDto(string Name, int? LocationId = null);

    public static class DtoExtensions
    {
        public static MenuDto ToDto(this Menu menu)
        {
            return new(menu.Name, menu.LocationId);
        }
    }

    public static class MenuHelpers
    {
        public static MenuDto MenuNotFound()
        {
            return new("Menua ei löydy");
        }
    }
}