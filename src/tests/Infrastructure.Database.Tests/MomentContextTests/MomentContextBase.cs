using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

namespace Infrastructure.Database.Tests.MomentContextTests;

[Collection("RunInSerialOrderToAvoidTestContainerConflicts")]
public abstract class MomentContextBase : IAsyncLifetime
{
    private MsSqlContainer _msSqlContainer = null!;
    protected Database.MomentContext _dbContext = null!;
    
    public async Task InitializeAsync()
    {
        _msSqlContainer = new MsSqlBuilder()
            .WithPortBinding(1433)
            .Build();

        await _msSqlContainer.StartAsync();
            
        var connectionString = _msSqlContainer.GetConnectionString();
        
        var optionsBuilder = new DbContextOptionsBuilder<Database.MomentContext>()
            .UseSqlServer(connectionString);
        
        _dbContext = new Database.MomentContext(optionsBuilder.Options);
        await _dbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _msSqlContainer.DisposeAsync();
    }
}