using Functions.ValidateToken;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Authentication;

public static class ConfigureServices
{
    public static void AddAuthenticationServices(this IServiceCollection services)
    {
        services.AddTransient<IValidateToken, ValidateToken>();
    }
}