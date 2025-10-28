using Functions.CreateUser;
using Microsoft.Extensions.DependencyInjection;

namespace Persistence;

public static class ConfigureServices
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services)
    {
        services.AddSingleton<ICreateUser, CreateUser>();
        return services;
    }
}