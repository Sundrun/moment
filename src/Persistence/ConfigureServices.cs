using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Operations.Commands.CreateUser;

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

            var context = new MomentContext(optionsBuilder.Options);

            var isLocal = config.GetValue<bool>("Application:IsDevelopment");
            if (isLocal)
            {
                context.Database.EnsureCreated();
            }

            return context;
        });
        
        services.AddTransient<ICreateUser, CreateUser>();
    }
}