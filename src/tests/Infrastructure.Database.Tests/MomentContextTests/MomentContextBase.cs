using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

namespace Infrastructure.Database.Tests.MomentContextTests;

[Collection("RunInSerialOrderToAvoidTestContainerConflicts")]
public abstract class MomentContextBase : IAsyncLifetime
{
    private MsSqlContainer _msSqlContainer = null!;
    protected MomentContext DbContext = null!;
    protected DateTimeOffset TestStartTime { get; set; }
    
    public async Task InitializeAsync()
    {
        TestStartTime = DateTimeOffset.UtcNow;
        
        _msSqlContainer = new MsSqlBuilder()
            .WithPortBinding(1433)
            .Build();

        await _msSqlContainer.StartAsync();
            
        var connectionString = _msSqlContainer.GetConnectionString();
        
        var optionsBuilder = new DbContextOptionsBuilder<MomentContext>()
            .UseSqlServer(connectionString);
        
        DbContext = new MomentContext(optionsBuilder.Options);
        await DbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _msSqlContainer.DisposeAsync();
    }
}