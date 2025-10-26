using AwesomeAssertions;
using Entities;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

namespace Infrastructure.Database.Tests;

public class MomentContextShould : IAsyncLifetime
{
    private MsSqlContainer _msSqlContainer = null!;
    private MomentContext _dbContext = null!;
    
    public async Task InitializeAsync()
    {
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
    public async Task RetrieveExpectedCoreMoment()
    {
        // Arrange
        var expected = new CoreMoment(new CoreMomentId(Guid.NewGuid()), new CoreMomentTimestamp(DateTimeOffset.UtcNow));
        
        await _dbContext.CoreMoments.AddAsync(expected);
        await _dbContext.SaveChangesAsync();
        
        // Act
        var result = _dbContext.CoreMoments.FirstOrDefault();
        
        // Assert
        result.Should().Be(expected);
    }
    
    [Fact]
    public async Task RetrieveExpectedMomentOwner()
    {
        // Arrange
        var expected = new MomentOwner(new MomentOwnerId(Guid.NewGuid()));
        
        await _dbContext.MomentOwners.AddAsync(expected);
        await _dbContext.SaveChangesAsync();
        
        // Act
        var result = _dbContext.MomentOwners.FirstOrDefault();
        
        // Assert
        result.Should().Be(expected);
    }
    
    [Fact]
    public async Task RetrieveExpectedMomentOwnership()
    {
        // Arrange
        var expected = new MomentOwnership(
            new MomentOwnershipId(Guid.NewGuid()),
            new CoreMomentId(Guid.NewGuid()),
            new MomentOwnerId(Guid.NewGuid()));
        
        await _dbContext.MomentOwnerships.AddAsync(expected);
        await _dbContext.SaveChangesAsync();
        
        // Act
        var result = _dbContext.MomentOwnerships.FirstOrDefault();
        
        // Assert
        result.Should().Be(expected);
    }
}