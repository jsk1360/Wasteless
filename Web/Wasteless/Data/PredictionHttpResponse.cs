using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Wasteless.Data
{
    public abstract class PredictionHttpResponse
    {
        public class PredictionResponse
        {
            [JsonPropertyName("schema")] public Schema Schema { get; set; }

            [JsonPropertyName("data")] public List<Prediction> Data { get; set; }
        }

        public class Prediction
        {
            [JsonPropertyName("LocationSID")] public long LocationSid { get; set; }

            [JsonPropertyName("DateSID")] public long DateSid { get; set; }

            [JsonPropertyName("PredictedMealCount")]
            public double PredictedMealCount { get; set; }

            [JsonPropertyName("PredictedWasteTotalKg")]
            public double PredictedWasteTotalKg { get; set; }

            [JsonPropertyName("PredictedLineWasteKg")]
            public double PredictedLineWasteKg { get; set; }

            [JsonPropertyName("PredictedPlateWasteKg")]
            public double PredictedPlateWasteKg { get; set; }
        }


        public class Field
        {
            [JsonPropertyName("name")] public string Name { get; set; }

            [JsonPropertyName("type")] public string Type { get; set; }
        }

        public class Schema
        {
            [JsonPropertyName("fields")] public List<Field> Fields { get; set; }

            [JsonPropertyName("pandas_version")] public string PandasVersion { get; set; }
        }
    }
}