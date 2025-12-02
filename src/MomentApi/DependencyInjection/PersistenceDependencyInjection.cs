using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Operations.Commands.CreateMoment;
using Operations.Commands.CreateUser;
using Operations.Queries.GetMoments;
using Persistence;

namespace MomentApi.DependencyInjection;

public static class PersistenceDependencyInjection
{
    public static void ConfigurePersistenceServices(this IServiceCollection services, IConfiguration config)
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
        
        services.AddTransient<ICreateMoment, CreateMoment>();
        services.AddTransient<ICreateUser, CreateUser>();
        services.AddTransient<IGetMoments, GetMoments>();
    }
}