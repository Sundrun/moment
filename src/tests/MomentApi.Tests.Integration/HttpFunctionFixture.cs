using Entities;
using Entities.Wrappers;
using Functions.Functions;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MomentApi.Extensions;
using Operations.Queries.ValidateToken;
using Testcontainers.MsSql;

namespace MomentApi.Tests.Integration;

[CollectionDefinition(nameof(MomentApiCollection))]
public class MomentApiCollection : ICollectionFixture<HttpFunctionFixture>
{
    // This class is a marker; no implementation needed
}

public class HttpFunctionFixture : IAsyncLifetime
{
    private MsSqlContainer _msSqlContainer = null!;
    private IHost _host = null!;
    public readonly GoogleIdentitySubject TestSubject = new("TestSubject");

    private class TestValidateToken : IValidateToken
    {
        public Task<IValidatedToken> ValidateTokenAsync(string token) => Task.FromResult<IValidatedToken>(new ValidToken(new GoogleIdentitySubject("TestSubject")));
    }
    
    public async Task InitializeAsync()
    {
        _msSqlContainer = new MsSqlBuilder()
            .WithPortBinding(1433)
            .Build();

        await _msSqlContainer.StartAsync();
        
        await InitializeDatabase();

        InitializeHostAsync();
    }

    private async Task InitializeDatabase()
    {
        var connectionString = _msSqlContainer.GetConnectionString();
        var optionsBuilder = new DbContextOptionsBuilder<MomentContext>()
            .UseSqlServer(connectionString);

        var context = new MomentContext(optionsBuilder.Options);
        await context.Database.EnsureCreatedAsync();
        
        await InitializeTestOwnerAsync(context);
    }

    private async Task InitializeTestOwnerAsync(MomentContext context)
    {
        var owner = new MomentOwner();
        await context.MomentOwners.AddAsync(owner);
        
        var identity = new GoogleIdentity
        {
            Subject = TestSubject
        };
        var identityOwner = new GoogleIdentityOwner
        {
            Owner = owner,
            GoogleIdentity = identity
        };
        await context.GoogleIdentities.AddAsync(identity);
        await context.GoogleIdentityOwners.AddAsync(identityOwner);
        await context.SaveChangesAsync();
    }

    private void InitializeHostAsync()
    {
        _host = new HostBuilder()
            .ConfigureAppConfiguration(configurationBuilder =>
            {
                var integrationTestConfig = new Dictionary<string, string>
                {
                    { "Application:IsDevelopment", "true" },
                    { "ConnectionStrings:MomentContext", _msSqlContainer.GetConnectionString() },
                };

                configurationBuilder.AddInMemoryCollection(integrationTestConfig!);
            })
            .ConfigureMomentApi()
            .ConfigureServices((_, services) =>
            {
                ReplaceValidateTokenService(services);

                services.AddTransient<HttpCreateMomentGoogleFunction>();
                services.AddTransient<HttpCreateUserGoogleFunction>();
            })
            .Build();
    }

    public async Task DisposeAsync()
    {
        await _msSqlContainer.DisposeAsync();
    }
    
    public T GetService<T>() where T : class, IHttpFunction
    {
        return _host.Services.GetRequiredService<T>();
    }
    
    private static void ReplaceValidateTokenService(IServiceCollection services)
    {
        var tokenValidators = services.Where(d => d.ServiceType == typeof(IValidateToken)).ToList();
        foreach (var tokenValidator in tokenValidators)
        {
            services.Remove(tokenValidator);
        }
                
        services.AddTransient<IValidateToken, TestValidateToken>();
    }
}