using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using WastelessFunction.Functions;
using WastelessFunction.Functions.MenuApi;
using WastelessFunction.Lib.Infrastructure;

[assembly: FunctionsStartup(typeof(Startup))]

namespace WastelessFunction.Functions;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        SetupServices.AddBiovaaka(builder.Services);
        SetupServices.AddPredictions(builder.Services);
        builder.Services.AddTransient<MenuService>();
    }
}