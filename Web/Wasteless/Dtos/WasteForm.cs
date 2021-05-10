using System;

namespace Wasteless.Dtos
{
    public class WasteForm
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int LocationId { get; set; }
        public int? MealTotal { get; set; }
        public int? MealCountReserved { get; set; }
        public double? LineWasteKg { get; set; }
        public double? PlateWasteKg { get; set; }
        public double? ProductionWasteKg { get; set; }
        public int? SpecialMealCount { get; set; }
        public string Menu { get; set; }
        public string? Comment { get; set; }
    }
}