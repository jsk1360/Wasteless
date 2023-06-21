namespace WastelessFunction.Lib.Predictions;

public class Predictions
{
    private readonly IPredictionsApi _predictionsApi;
    private readonly PredictionsUpdater _predictionsUpdater;

    public Predictions(IPredictionsApi predictionsApi, PredictionsUpdater predictionsUpdater)
    {
        _predictionsApi = predictionsApi;
        _predictionsUpdater = predictionsUpdater;
    }

    public async Task Update(DateOnly date)
    {
        var currentDate = new DateOnly(2021, 11, 1);
        var maxDate = new DateOnly(2022, 2, 1);
        var data = await _predictionsApi.GetPrediction(currentDate);
        while (data is not null && currentDate < maxDate)
        {
            foreach (var datum in data.Data)
            {
                await _predictionsUpdater.UpdatePredictions(datum);
            }

            Console.WriteLine($"Päivämäärä: {currentDate}");
            Console.WriteLine($"Tuloksia: {data.Data.Count}");
            currentDate = currentDate.AddDays(14);
            data = await _predictionsApi.GetPrediction(currentDate);
        }
    }
}