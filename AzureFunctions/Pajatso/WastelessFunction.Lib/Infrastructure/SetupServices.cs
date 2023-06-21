using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Refit;
using WastelessFunction.Lib.Biovaaka;
using WastelessFunction.Lib.Predictions;

namespace WastelessFunction.Lib.Infrastructure;

public static class SetupServices
{
    public static void AddBiovaaka(IServiceCollection services)
    {
        services.AddOptions<BiovaakaAuthOptions>().Configure<IConfiguration>((settings, config) =>
        {
            config.GetSection("Biovaaka:Auth").Bind(settings);
        });
        services.AddOptions<BiovaakaOptions>().Configure<IConfiguration>((settings, config) =>
        {
            config.GetSection("Biovaaka:Api").Bind(settings);
        });

        services.AddTransient<BiovaakaAuthHeaderHandler>();
        services.AddTransient<BiovaakaClient>();
        services.AddTransient<BiovaakaUpdater>();
        services.AddRefitClient<IBiovaakaAuthClient>()
            .ConfigureHttpClient((servicesProvider, c) =>
            {
                var options = servicesProvider.GetRequiredService<IOptions<BiovaakaAuthOptions>>();
                c.BaseAddress =
                    new Uri($"https://login.microsoftonline.com/{options.Value.Tenant}");
            });
        
        services.AddRefitClient<IBiovaakaClient>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://api.biovaaka.cloud/api"))
            .AddHttpMessageHandler<BiovaakaAuthHeaderHandler>();
    }

    public static void AddPredictions(IServiceCollection services)
    {
        services.AddOptions<PredictionOptions>().Configure<IConfiguration>((settings, configuration) =>
        {
            configuration.GetSection("Predictions").Bind(settings);
        });
        services.AddOptions<ConnectionString>().Configure<IConfiguration>((settings, configuration) =>
        {
            configuration.GetSection("ConnectionStrings").Bind(settings);
        });

        services.AddTransient<PredictionsUpdater>();
        services.AddTransient<AuthHeaderHandler>();
        services.AddTransient<Predictions.Predictions>();
        services.AddRefitClient<IPredictionsApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://wasteless-test.azurewebsites.net/api"))
            .AddHttpMessageHandler<AuthHeaderHandler>();
    }
}