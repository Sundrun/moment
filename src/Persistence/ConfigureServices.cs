using Functions.CreateUser;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Persistence;

public static class ConfigureServices
{
    public static void AddPersistenceServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddTransient<MomentContext>(_ =>
        {
            var connectionString = config.GetConnectionString("MomentContext");
            
            var optionsBuilder = new DbContextOptionsBuilder<MomentContext>()
                .UseSqlServer(connectionString);

            return new MomentContext(optionsBuilder.Options);
        });
        
        services.AddTransient<ICreateUser, CreateUser>();
    }
}