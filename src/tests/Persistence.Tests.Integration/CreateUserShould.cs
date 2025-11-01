using AwesomeAssertions;
using Functions.ValidateToken;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

namespace Persistence.Tests.Integration;

public class CreateUserShould : IAsyncLifetime
{
    private CreateUser _dtu = null!;
    private MsSqlContainer _msSqlContainer = null!;
    private MomentContext _dbContext = null!;
    
    public async Task InitializeAsync()
    {
        _dtu = new CreateUser();
        
        _msSqlContainer = new MsSqlBuilder()
            .WithPortBinding(1433)
            .Build();

        await _msSqlContainer.StartAsync();
            
        var connectionString = _msSqlContainer.GetConnectionString();
        
        var optionsBuilder = new DbContextOptionsBuilder<MomentContext>()
            .UseSqlServer(connectionString);
        
        _dbContext = new MomentContext(optionsBuilder.Options);
        await _dbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _msSqlContainer.DisposeAsync();
    }
    
    [Fact]
    public async Task AddOwner()
    {
        // Act
        await _dtu.CreateAsync(new ValidToken(string.Empty));
        var result = await _dbContext.MomentOwners.FirstOrDefaultAsync();

        // Assert
        result.Should().NotBeNull();
    }
}