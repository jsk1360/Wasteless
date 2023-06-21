using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using WastelessFunction.Lib.Biovaaka;

namespace WastelessFunction.Functions;

public class BiovaakaFunctions
{
    private readonly ILogger<BiovaakaFunctions> _logger;
    private readonly BiovaakaUpdater _biovaakaUpdater;

    public BiovaakaFunctions(ILogger<BiovaakaFunctions> logger, BiovaakaUpdater biovaakaUpdater)
    {
        _logger = logger;
        _biovaakaUpdater = biovaakaUpdater;
    }

    [FunctionName("UpdateMeasurementsTimer")]
    public async Task RunAsync([TimerTrigger("%biovaakaschedule%")] TimerInfo myTimer, ILogger log)
    {
        log.LogInformation($"C# Timer trigger function executed at: {DateTime.UtcNow}");

        var today = DateOnly.FromDateTime(DateTime.Today);

        try
        {
            log.LogInformation($"Updating Biovaaka measurements for {today}");
            await _biovaakaUpdater.Update(today);
            log.LogInformation($"Successfully updated Biovaaka measurements for {today}");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Could not update Biovaaka data");
            throw;
        }
    }
}