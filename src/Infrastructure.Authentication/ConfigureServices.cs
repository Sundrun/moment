using Functions;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Authentication;

public static class ConfigureServices
{
    public static IServiceCollection AddAuthenticationServices(this IServiceCollection services)
    {
        services.AddTransient<IValidateToken, ValidateToken>();
        return services;
    }
}