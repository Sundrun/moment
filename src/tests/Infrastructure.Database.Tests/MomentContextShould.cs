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
        _dbContext.MomentOwners.Add(new MomentOwner());
        await _dbContext.SaveChangesAsync();
        var storedOwner = await _dbContext.MomentOwners.FirstOrDefaultAsync();
        
        var newMoment = new CoreMoment { Timestamp = new CoreMomentTimestamp(DateTimeOffset.UtcNow) };
        _dbContext.CoreMoments.Add(newMoment);
        
        var newOwnership = new MomentOwnership{ Moment = newMoment, Owner = storedOwner!};
        await _dbContext.MomentOwnerships.AddAsync(newOwnership);
        await _dbContext.SaveChangesAsync();
        
        var storedMoment = await _dbContext.CoreMoments.FirstOrDefaultAsync();
        
        var expected = new MomentOwnership
        {
            Id = new MomentOwnershipId(1),
            MomentId = storedMoment!.Id,
            Moment = storedMoment,
            OwnerId = storedOwner!.Id,
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
    
    [Fact]
    public async Task RetrieveOwnerGoogleIdentity()
    {
        // Arrange
        _dbContext.MomentOwners.Add(new MomentOwner());
        await _dbContext.SaveChangesAsync();
        var storedOwner = await _dbContext.MomentOwners.FirstOrDefaultAsync();
        
        var testSubject = new OwnerGoogleIdentitySubject("test-subject");
        var newGoogleIdentity = new OwnerGoogleIdentity { Owner = storedOwner!, Subject = testSubject };
        await _dbContext.OwnerGoogleIdentities.AddAsync(newGoogleIdentity);
        await _dbContext.SaveChangesAsync();
        
        var expected = new OwnerGoogleIdentity
        {
            Id = new OwnerGoogleIdentityId(1),
            OwnerId = storedOwner!.Id,
            Owner = storedOwner,
            Subject = testSubject
        };
        
        // Act
        var result = await _dbContext.OwnerGoogleIdentities
            .Include(x => x.Owner)
            .FirstOrDefaultAsync();
        
        // Assert
        result.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public async Task NotAllowDuplicateGoogleIdentities()
    {
        // Arrange
        _dbContext.MomentOwners.Add(new MomentOwner());
        _dbContext.MomentOwners.Add(new MomentOwner());
        await _dbContext.SaveChangesAsync();
        
        var ownerOne = await _dbContext.MomentOwners.FirstAsync(x => x.Id == new MomentOwnerId(1));
        var ownerTwo = await _dbContext.MomentOwners.FirstAsync(x => x.Id == new MomentOwnerId(2));
        
        var testSubject = new OwnerGoogleIdentitySubject("test-subject");
        var firstGoogleIdentity = new OwnerGoogleIdentity { Owner = ownerOne, Subject = testSubject };
        await _dbContext.OwnerGoogleIdentities.AddAsync(firstGoogleIdentity);
        await _dbContext.SaveChangesAsync();
        
        var duplicateGoogleIdentity = new OwnerGoogleIdentity { Owner = ownerTwo, Subject = testSubject };
        await _dbContext.OwnerGoogleIdentities.AddAsync(duplicateGoogleIdentity);
        
        // Act
        var dbSaveTask = async () => await _dbContext.SaveChangesAsync();
        
        // Assert
        await dbSaveTask.Should().ThrowAsync<DbUpdateException>();
    }
}