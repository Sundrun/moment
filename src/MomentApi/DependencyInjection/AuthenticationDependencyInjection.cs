using Infrastructure.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Operations.Queries.ValidateToken;

namespace MomentApi.DependencyInjection;

public static class AuthenticationDependencyInjection
{
    public static void ConfigureAuthenticationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddTransient<IValidateToken>(_ =>
        {
            var googleConfig = config.GetSection("Google");
            var clientId = googleConfig.GetValue<string>("ClientId")!;

            return new ValidateToken(clientId);
        });
    }
}