using Microsoft.Extensions.Options;
using WastelessFunction.Lib.Biovaaka.Models;
using WastelessFunction.Lib.Biovaaka.Models.Measurement;
using WastelessFunction.Lib.Biovaaka.Models.Production;

namespace WastelessFunction.Lib.Biovaaka;

public class BiovaakaClient
{
    private readonly IBiovaakaClient _client;
    private readonly BiovaakaOptions _options;

    public BiovaakaClient(IOptions<BiovaakaOptions> options, IBiovaakaClient client)
    {
        _client = client;
        _options = options.Value;
    }

    public async Task<List<MeasurementResult>> GetMeasurements(DateOnly startDate,
        DateOnly endDate)
    {
        var request = new BiovaakaRequestParameters(_options.CustomerGuid, _options.SiteGuid, startDate, endDate);
        var measurements = await _client.GetMeasurements(request);
        return measurements.ToList();
    }

    public async Task<List<ProductionResult>> GetProduction(DateOnly startDate, DateOnly endDate)
    {
        var request = new BiovaakaRequestParameters(_options.CustomerGuid, _options.SiteGuid, startDate, endDate);
        var productions = await _client.GetProduction(request);
        return productions.ToList();
    }
}