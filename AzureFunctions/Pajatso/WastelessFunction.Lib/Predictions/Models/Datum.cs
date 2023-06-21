namespace WastelessFunction.Functions.Predictions.Models;

public class Datum
{
    public int LocationSid { get; set; }
    public int DateSid { get; set; }
    public double MenuItemSid { get; set; }
    public string Dish { get; set; }
    public double PredictedWasteTotalKg { get; set; }

    public double PredictedMealTotal { get; set; }
    public double PredictedProductionWasteKg { get; set; }
    public double PredictedLineWasteKg { get; set; }
    public double PredictedPlateWasteKg { get; set; }
}