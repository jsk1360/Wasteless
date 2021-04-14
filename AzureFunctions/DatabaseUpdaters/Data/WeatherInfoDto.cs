using System.Text.Json.Serialization;

namespace WastelessFunction.Data
{
    public class WeatherResult
    {
        [JsonPropertyName("lat")]
        public double Lat { get; set; }

        [JsonPropertyName("lon")]
        public double Lon { get; set; }

        [JsonPropertyName("timezone")]
        public string Timezone { get; set; }

        [JsonPropertyName("timezone_offset")]
        public long TimezoneOffset { get; set; }

        [JsonPropertyName("daily")]
        public Daily[] Daily { get; set; }
    }

    public class Daily
    {
        [JsonPropertyName("dt")]
        public long Dt { get; set; }

        [JsonPropertyName("sunrise")]
        public long Sunrise { get; set; }

        [JsonPropertyName("sunset")]
        public long Sunset { get; set; }

        [JsonPropertyName("temp")]
        public Temp Temp { get; set; }

        [JsonPropertyName("feels_like")]
        public FeelsLike FeelsLike { get; set; }

        [JsonPropertyName("pressure")]
        public long Pressure { get; set; }

        [JsonPropertyName("humidity")]
        public long Humidity { get; set; }

        [JsonPropertyName("dew_point")]
        public double DewPoint { get; set; }

        [JsonPropertyName("wind_speed")]
        public double WindSpeed { get; set; }

        [JsonPropertyName("wind_deg")]
        public long WindDeg { get; set; }

        [JsonPropertyName("weather")]
        public WeatherElement[] Weather { get; set; }

        [JsonPropertyName("clouds")]
        public int Clouds { get; set; }

        [JsonPropertyName("pop")]
        public double Pop { get; set; }

        [JsonPropertyName("uvi")]
        public double Uvi { get; set; }

        [JsonPropertyName("snow")]
        public double? Snow { get; set; }
    }

    public class FeelsLike
    {
        [JsonPropertyName("day")]
        public double Day { get; set; }

        [JsonPropertyName("night")]
        public double Night { get; set; }

        [JsonPropertyName("eve")]
        public double Eve { get; set; }

        [JsonPropertyName("morn")]
        public double Morn { get; set; }
    }

    public class Temp
    {
        [JsonPropertyName("day")]
        public double Day { get; set; }

        [JsonPropertyName("min")]
        public double Min { get; set; }

        [JsonPropertyName("max")]
        public double Max { get; set; }

        [JsonPropertyName("night")]
        public double Night { get; set; }

        [JsonPropertyName("eve")]
        public double Eve { get; set; }

        [JsonPropertyName("morn")]
        public double Morn { get; set; }
    }

    public class WeatherElement
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("main")]
        public string Main { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("icon")]
        public string Icon { get; set; }
    }
}
