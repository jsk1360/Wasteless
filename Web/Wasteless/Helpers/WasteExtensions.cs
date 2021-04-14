using System.Collections.Generic;
using System.Linq;
using Wasteless.Data;
using Wasteless.Models;

namespace Wasteless.Helpers
{
    public static class WasteExtensions
    {
        public static void SetPredictions(this Waste wasteDto, PredictionHttpResponse.Prediction prediction)
        {
            wasteDto.ForecastMealCount = prediction.PredictedMealCount;
            wasteDto.ForecastWasteTotalKg = prediction.PredictedWasteTotalKg;
            wasteDto.ForecastLineWasteKg = prediction.PredictedLineWasteKg;
            wasteDto.ForecastPlateWasteKg = prediction.PredictedPlateWasteKg;
        }

        public static void SetPredictions(this IEnumerable<Waste> wastes,
            IEnumerable<PredictionHttpResponse.Prediction> predictions)
        {
            var tmpWastes = wastes.ToList();
            foreach (var prediction in predictions)
            {
                var waste = tmpWastes.Where(x =>
                    x.DateId == prediction.DateSid && x.LocationId == prediction.LocationSid).ToList();

                if (waste.Count == 0) continue;

                foreach (var wasteDto in waste) wasteDto.SetPredictions(prediction);
            }
        }
    }
}