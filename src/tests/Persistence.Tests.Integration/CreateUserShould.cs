using AwesomeAssertions;
using Functions.CreateUser;
using Functions.ValidateToken;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

namespace Persistence.Tests.Integration;

public class CreateUserShould : IAsyncLifetime
{
    private CreateUser _dtu = null!;
    private MsSqlContainer _msSqlContainer = null!;
    private MomentContext _testContext = null!;
    
    public async Task InitializeAsync()
    {
        
        _msSqlContainer = new MsSqlBuilder()
            .WithPortBinding(1433)
            .Build();

        await _msSqlContainer.StartAsync();
            
        var connectionString = _msSqlContainer.GetConnectionString();
        
        var optionsBuilder = new DbContextOptionsBuilder<MomentContext>()
            .UseSqlServer(connectionString);
        
        _testContext = new MomentContext(optionsBuilder.Options);
        await _testContext.Database.EnsureCreatedAsync();
        
        var createUserContext = new MomentContext(optionsBuilder.Options);
        _dtu = new CreateUser(createUserContext);
    }

    public async Task DisposeAsync()
    {
        await _msSqlContainer.DisposeAsync();
    }
    
    [Fact]
    public async Task StoreOwner()
    {
        // Act
        await _dtu.CreateAsync(new ValidToken(string.Empty));
        var result = await _testContext.MomentOwners.FirstOrDefaultAsync();

        // Assert
        result.Should().NotBeNull();
    }
    
    [Fact]
    public async Task IndicateThatAOwnerWasCreated()
    {
        // Act
        var result = await _dtu.CreateAsync(new ValidToken(string.Empty));

        // Assert
        result.Should().BeOfType<UserCreated>();
    }
}