using System;
using System.Collections.Generic;
using System.Linq;
using Wasteless.Models;

namespace Wasteless.Dtos
{
    public class WasteDto
    {
        public WasteDto(string location, string city, IReadOnlyCollection<FactMenuItem>? menuItems, IReadOnlyCollection<Waste> wastes, WasteLimit? limit = null)
        {
            var waste = wastes.First();
            
            Id = waste.WasteSID;
            LocationId = waste.LocationId;
            Location = location;
            City = city;
            Menu = menuItems?.FirstOrDefault()?.Menu;
            Date = waste.Date;
            ForecastMealTotal = waste.ForecastMealTotal;
            
            var menuItemWasteForecast = from i in menuItems
                join l in wastes on i.MenuItemId equals l.MenuItemId
                select new MenuItemWaste(new PajatsoMenuItem(i.MenuItemId, i.Name), LineWasteKg: l.ForecastLineWasteKg.Round(2), ProductionWasteKg: l.ForecastProductionWasteKg.Round(2), PlateWasteKg: l.ForecastPlateWasteKg.Round(2), l.ForecastWasteTotalKg.Round(2));

            ForecastMenuItemWaste = menuItemWasteForecast;
            ForecastPlateWasteKg = waste.ForecastPlateWasteKg.Round(2);
            ForecastWasteTotalKg = waste.ForecastWasteTotalKg.Round(2);
            MealTotal = waste.MealTotal;
            
            var menuItemWaste = from i in menuItems
                join l in wastes on i.MenuItemId equals l.MenuItemId
                select new MenuItemWaste(new PajatsoMenuItem(i.MenuItemId, i.Name), LineWasteKg: l.LineWasteKg.Round(2), ProducedKg: l.ProducedKg.Round(2));
            
            MenuItemWaste = menuItemWaste;
            PlateWasteKg = wastes.Sum(w => w.PlateWasteKg).Round(2);
            ProductionWasteKg = wastes.Sum(w => w.ProductionWasteKg).Round(2);
            MealCountReserved = waste.MealCountReserved;
            SpecialMealCount = waste.SpecialMealCount;
            Comment = waste.Comment;
            WasteLimit = limit?.Limit;
        }

        public int Id { get; set; }
        public int LocationId { get; set; }
        public string Location { get; set; }
        public string City { get; set; }
        public DateTime Date { get; set; }
        public string? Menu { get; set; }

        public double? ForecastMealTotal { get; set; }
        public double? ForecastPlateWasteKg { get; set; }
        public double? ForecastWasteTotalKg { get; set; }
        public IEnumerable<MenuItemWaste>? ForecastMenuItemWaste { get; set; }
        public int? MealTotal { get; set; }
        public int? SpecialMealCount { get; set; }

        public IEnumerable<MenuItemWaste>? MenuItemWaste { get; set; }
        public double? PlateWasteKg { get; set; }
        public double? ProductionWasteKg { get; set; }

        public int? MealCountReserved { get; set; }

        public double? WasteLimit { get; set; }
        public string? Comment { get; set; }
    }
    
    public record MenuItemWaste(PajatsoMenuItem Item, double? LineWasteKg = null, double? ProductionWasteKg = null, double? PlateWasteKg = null, double? TotalWasteKg = null, double? ProducedKg = null);

    public record PajatsoMenuQueryResult(string MenuName, int MenuItemId, string MenuItemName,
        PajatsoMenuItemType MenuItemType);

    public record PajatsoMenu(string Name, IEnumerable<PajatsoMenuItem> Items);

    public record PajatsoMenuItem(int Id, string Name);

    public enum PajatsoMenuItemType
    {
        Side,
        Main
    }

    public static class MenuHelpers
    {

        public static IEnumerable<PajatsoMenu> ConvertToMenu(this IEnumerable<PajatsoMenuQueryResult> results)
        {
            var grouped = results.GroupBy(r => new { r.MenuName });

            return grouped.Select(g =>
                new PajatsoMenu(g.Key.MenuName, g.Select(r => new PajatsoMenuItem(r.MenuItemId, r.MenuItemName))));
        }
    }

    public static class DoubleHelpers
    {
        public static double? Round(this double? value, int digits)
        {
            if (value.HasValue)
            {
                return Math.Round(value.Value, digits);
            }
            
            return null;
        }
    }
}