using System;
using System.Globalization;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace WastelessFunction.Data
{
    public class WasteDB

    {
        public int WasteSID { get; set; }
        public int DateSID { get; set; }               
        public int LocationSID { get; set; }
        public int? Forecast_MealTotal { get; set; }
        public double? Forecast_SpecialMealCount { get; set; }
        public double? Forecast_MealCount { get; set; }
        public double? Forecast_ProductionWasteKg { get; set; }
        public double? Forecast_LineWasteKg { get; set; }
        public double? Forecast_PlateWasteKg { get; set; }
        public double? Forecast_WasteTotalKg { get; set; }
        
               
    }

}
