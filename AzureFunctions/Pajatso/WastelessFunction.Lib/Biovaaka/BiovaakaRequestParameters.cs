using Refit;

namespace WastelessFunction.Lib.Biovaaka;

public class BiovaakaRequestParameters
{
    public BiovaakaRequestParameters(Guid customerGuid, Guid siteGuid, DateOnly startDate, DateOnly endDate)
    {
        CustomerGuid = customerGuid;
        SiteGuid = siteGuid;
        StartDate = startDate.ToString("yyyy-MM-dd");
        EndDate = endDate.ToString("yyyy-MM-dd");
    }

    [AliasAs("customerGuid")] public Guid CustomerGuid { get; }
    [AliasAs("siteGuid")] public Guid SiteGuid { get; }
    [AliasAs("startDate")] public string StartDate { get; }
    [AliasAs("endDate")] public string EndDate { get; }
}