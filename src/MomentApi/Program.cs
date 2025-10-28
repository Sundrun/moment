using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistence;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration((_, config) =>
    {
        config.AddEnvironmentVariables();
    })
    .ConfigureServices((_, services) =>
    {
        services.AddPersistenceServices();
    })
    .ConfigureLogging(logging => logging.AddConsole())
    .Build();

await host.RunAsync();

public partial class Program { }
