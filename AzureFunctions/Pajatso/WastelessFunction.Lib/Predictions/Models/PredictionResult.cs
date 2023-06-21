using System.Collections.Generic;

namespace WastelessFunction.Functions.Predictions.Models;

public class PredictionResult
{
    public Schema Schema { get; set; }
    public List<Datum> Data { get; set; }
}