using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Linq;

namespace WastelessFunction.Data
{
    public class WasteJSON
    {

        [JsonPropertyName("LocationSID")] public int LocationSid { get; set; }

        [JsonPropertyName("DateSID")] public int DateSid { get; set; }

        [JsonPropertyName("PredictedMealTotal")]
        public double PredictedMealTotal { get; set; }

        [JsonPropertyName("PredictedWasteTotalKg")]
        public double PredictedWasteTotalKg { get; set; }

        [JsonPropertyName("PredictedLineWasteKg")]
        public double PredictedLineWasteKg { get; set; }

        [JsonPropertyName("PredictedPlateWasteKg")]
        public double PredictedPlateWasteKg { get; set; }

       
    }
    public class Prediction
    {
        [JsonPropertyName("schema")] public Schema Schema { get; set; }

        [JsonPropertyName("data")] public List<WasteJSON> Data { get; set; }
    }
    public class Schema
    {
        [JsonPropertyName("fields")] public List<Field> Fields { get; set; }

        [JsonPropertyName("pandas_version")] public string PandasVersion { get; set; }
    }
    public class Field
    {
        [JsonPropertyName("name")] public string Name { get; set; }

        [JsonPropertyName("type")] public string Type { get; set; }
    }
    public static class WasteHelpers
    {
        public static IEnumerable<WasteDB> SetPredictions(this IEnumerable<WasteDB> wastes, IEnumerable<WasteJSON> predictions)
        {
            var tmpWastes = wastes.ToList();
            foreach (var prediction in predictions)
            {
                var waste = tmpWastes.Where(x =>
                    x.DateSID == prediction.DateSid && x.LocationSID == prediction.LocationSid).ToList();

                if (waste.Count == 0)
                {
                    var wasteNew = new WasteDB();
                    wasteNew.LocationSID = prediction.LocationSid;
                    wasteNew.DateSID = prediction.DateSid;
                    waste.Add(wasteNew);
                    tmpWastes.Add(wasteNew);
                }

                foreach (var wasteDto in waste)
                {
                    wasteDto.Forecast_MealTotal = (int)Math.Round(prediction.PredictedMealTotal,0);
                    wasteDto.Forecast_WasteTotalKg = prediction.PredictedWasteTotalKg;
                    wasteDto.Forecast_LineWasteKg = prediction.PredictedLineWasteKg;
                    wasteDto.Forecast_PlateWasteKg = prediction.PredictedPlateWasteKg;
                }
                
            }
            return tmpWastes;
        }
    }

}
