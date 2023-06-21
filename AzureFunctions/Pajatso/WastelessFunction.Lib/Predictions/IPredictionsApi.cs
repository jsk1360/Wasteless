using Refit;
using WastelessFunction.Functions.Predictions.Models;

namespace WastelessFunction.Lib.Predictions;

public interface IPredictionsApi
{
    [Get("/preditem")]
    Task<PredictionResult?> GetPrediction(DateOnly? date = null);
}