using Microsoft.Extensions.DependencyInjection;
using Operations.Queries.ValidateToken;

namespace Infrastructure.Authentication;

public static class ConfigureServices
{
    public static void AddAuthenticationServices(this IServiceCollection services)
    {
        services.AddTransient<IValidateToken, ValidateToken>();
    }
}