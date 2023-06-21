using Refit;
using WastelessFunction.Lib.Biovaaka.Models;
using WastelessFunction.Lib.Biovaaka.Models.Measurement;
using WastelessFunction.Lib.Biovaaka.Models.Production;

namespace WastelessFunction.Lib.Biovaaka;

public interface IBiovaakaClient
{
    [Get("/measurements/customer/{request.customerGuid}/site/{request.siteGuid}")]
    Task<IEnumerable<MeasurementResult>> GetMeasurements(BiovaakaRequestParameters request);

    [Get("/production/customer/{request.customerGuid}/site/{request.siteGuid}")]
    Task<IEnumerable<ProductionResult>> GetProduction(BiovaakaRequestParameters request);
}