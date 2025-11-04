using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MomentApi.DependencyInjection;

namespace MomentApi.Extensions;

public static class HostBuilderConfigureMomentApiExtension
{
    public static IHostBuilder ConfigureMomentApi(this IHostBuilder hostBuilder)
    {
        return hostBuilder.ConfigureFunctionsWorkerDefaults()
            .ConfigureAppConfiguration((_, config) =>
            {
                config.AddEnvironmentVariables();
            })
            .ConfigureServices((context, services) =>
            {
                services.ConfigureAuthenticationServices(context.Configuration);
                services.ConfigurePersistenceServices(context.Configuration);
            })
            .ConfigureLogging(logging => logging.AddConsole());
    }
}