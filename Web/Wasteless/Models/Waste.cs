using System;
using System.Globalization;

namespace Wasteless.Models
{
    public class Waste
    {
        public int WasteSID { get; set; }
        public int DateId { get; set; }
        public int MenuItemId { get; set; }

        public DateTime Date => GetDate();

        public double? ProductionWasteKg { get; set; }
        public double? LineWasteKg { get; set; }
        public double? PlateWasteKg { get; set; }
        public double? WasteTotalKg { get; set; }
        public int LocationId { get; set; }
        public int? MealCount { get; set; }
        public int? SpecialMealCount { get; set; }
        public int? MealTotal { get; set; }
        public int? ForecastMealTotal { get; set; }
        public double? ForecastSpecialMealCount { get; set; }
        public double? ForecastMealCount { get; set; }
        public double? ForecastProductionWasteKg { get; set; }
        public double? ForecastLineWasteKg { get; set; }
        public double? ForecastPlateWasteKg { get; set; }
        public double? ForecastWasteTotalKg { get; set; }
        public DateTime ModifiedTime { get; set; }
        public string WeatherInfo { get; set; } = string.Empty;
        public int? MealCountReserved { get; set; }
        public double? ProducedKg { get; set; }
        
        public string? Comment { get; set; }

        private DateTime GetDate()
        {
            if (DateTime.TryParseExact(DateId.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var result))
                return result;

            throw new Exception("Invalid DateSID");
        }
    }

    public class WasteLimit
    {
        public int LocationId { get; set; }
        public double Limit { get; set; }
    }
}