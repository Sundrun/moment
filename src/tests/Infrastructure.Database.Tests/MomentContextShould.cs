using AwesomeAssertions;
using Entities;
using Entities.Wrappers;
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
        var testTimestamp = new CoreMomentTimestamp(DateTimeOffset.UtcNow);
        
        var newMoment = new CoreMoment{Timestamp = testTimestamp};
        await _dbContext.CoreMoments.AddAsync(newMoment);
        await _dbContext.SaveChangesAsync();
        
        var expected = new CoreMoment{Id = new CoreMomentId(1), Timestamp = testTimestamp};
        
        // Act
        var result = await _dbContext.CoreMoments.FirstOrDefaultAsync();
        
        // Assert
        result.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public async Task RetrieveExpectedMomentOwner()
    {
        // Arrange
        var newOwner = new MomentOwner();
        await _dbContext.MomentOwners.AddAsync(newOwner);
        await _dbContext.SaveChangesAsync();

        var expected = new MomentOwner{Id = new MomentOwnerId(1)};
        
        // Act
        var result = await _dbContext.MomentOwners.FirstOrDefaultAsync();
        
        // Assert
        result.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public async Task RetrieveExpectedMomentOwnership()
    {
        // Arrange
        var newMoment = new CoreMoment { Timestamp = new CoreMomentTimestamp(DateTimeOffset.UtcNow) };
        _dbContext.CoreMoments.Add(newMoment);
        
        var newOwner = new MomentOwner();
        _dbContext.MomentOwners.Add(newOwner);
        await _dbContext.SaveChangesAsync();
        
        var storedMoment = await _dbContext.CoreMoments.FirstOrDefaultAsync();
        var storedOwner = await _dbContext.MomentOwners.FirstOrDefaultAsync();
        
        var newOwnership = new MomentOwnership{ MomentId = storedMoment!.Id, OwnerId = storedOwner!.Id };
        
        await _dbContext.MomentOwnerships.AddAsync(newOwnership);
        await _dbContext.SaveChangesAsync();

        var expected = new MomentOwnership
        {
            Id = new MomentOwnershipId(1),
            MomentId = storedMoment.Id,
            Moment = storedMoment,
            OwnerId = storedOwner.Id,
            Owner = storedOwner
        };
        
        // Act
        var result = await _dbContext.MomentOwnerships
            .Include(x => x.Moment)
            .Include(x => x.Owner)
            .FirstOrDefaultAsync();
        
        // Assert
        result.Should().BeEquivalentTo(expected);
    }
}