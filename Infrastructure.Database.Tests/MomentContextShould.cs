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
        var expected = new CoreMoment{Id = new CoreMomentId{Id = Guid.NewGuid()}, Timestamp = new CoreMomentTimestamp{Timestamp = DateTimeOffset.UtcNow}};
        
        await _dbContext.CoreMoments.AddAsync(expected);
        await _dbContext.SaveChangesAsync();
        
        // Act
        var result = _dbContext.CoreMoments.FirstOrDefault();
        
        // Assert
        result.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public async Task RetrieveExpectedMomentOwner()
    {
        // Arrange
        var expected = new MomentOwner{Id = new MomentOwnerId{Id = Guid.NewGuid()}};
        
        await _dbContext.MomentOwners.AddAsync(expected);
        await _dbContext.SaveChangesAsync();
        
        // Act
        var result = _dbContext.MomentOwners.FirstOrDefault();
        
        // Assert
        result.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public async Task RetrieveExpectedMomentOwnership()
    {
        // Arrange
        var moment = new CoreMoment
        {
            Id = new CoreMomentId
            {
                Id = Guid.NewGuid()
            },
            Timestamp = new CoreMomentTimestamp
            {
                Timestamp = DateTimeOffset.UtcNow
            }
        };
        _dbContext.CoreMoments.Add(moment);
        
        var owner = new MomentOwner{Id = new MomentOwnerId{Id = Guid.NewGuid()}};
        _dbContext.MomentOwners.Add(owner);
        
        var expected = new MomentOwnership{Id = new MomentOwnershipId{Id = Guid.NewGuid()}, 
            MomentId = moment.Id,
            OwnerId = owner.Id};
        await _dbContext.MomentOwnerships.AddAsync(expected);
        
        await _dbContext.SaveChangesAsync();
        
        // Act
        var result = _dbContext.MomentOwnerships.FirstOrDefault();
        
        // Assert
        result.Should().BeEquivalentTo(expected);
    }
}