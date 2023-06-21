using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WastelessFunction.Lib.Biovaaka;
using WastelessFunction.Lib.Infrastructure;

var config = SetupConfiguration();

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(b => b.AddConfiguration(config))
    .ConfigureServices(SetupServices.AddBiovaaka)
    .ConfigureServices(SetupServices.AddPredictions)
    .Build();


IConfiguration SetupConfiguration()
{
    return new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .AddEnvironmentVariables()
        .AddCommandLine(args)
        .Build();
}

// var predictions = host.Services.GetRequiredService<Predictions>();
// var today = DateOnly.FromDateTime(DateTime.Today);
// await predictions.Update(today);

// var biovaakaClient = host.Services.GetRequiredService<BiovaakaUpdater>();
//
// await biovaakaClient.Update(new DateOnly(2021, 1, 12));