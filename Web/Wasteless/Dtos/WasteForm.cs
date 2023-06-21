using System;
using System.Collections.Generic;

namespace Wasteless.Dtos
{
    public class WasteForm
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int LocationId { get; set; }
        public int? MealTotal { get; set; }
        public int? MealCountReserved { get; set; }
        public IEnumerable<MenuItemWaste>? MenuItemWaste { get; set; }
        public double? PlateWasteKg { get; set; }
        public int? SpecialMealCount { get; set; }
        public string? Comment { get; set; }
    }
}