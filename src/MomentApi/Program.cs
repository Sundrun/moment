using Infrastructure.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MomentApi.DependencyInjection;
using Operations.Queries.ValidateToken;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration((_, config) =>
    {
        config.AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        services.ConfigureAuthenticationServices(context.Configuration);
        services.ConfigurePersistenceServices(context.Configuration);
        
        services.AddTransient<IValidateToken, ValidateToken>();
    })
    .ConfigureLogging(logging => logging.AddConsole())
    .Build();

await host.RunAsync();

public partial class Program { }
