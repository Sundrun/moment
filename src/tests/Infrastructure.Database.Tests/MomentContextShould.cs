using AwesomeAssertions;
using Entities;
using Entities.Wrappers;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

namespace Infrastructure.Database.Tests;

[Collection("RunInSerialOrderToAvoidTestContainerConflicts")]
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
    public async Task StoreExpectedGoogleIdentityOwner()
    {
        // Arrange
        _dbContext.MomentOwners.Add(new MomentOwner());
        await _dbContext.SaveChangesAsync();
        var storedOwner = await _dbContext.MomentOwners.FirstOrDefaultAsync();
        
        var testSubject = new GoogleIdentitySubject("test-subject");
        var newGoogleIdentity = new GoogleIdentity { Subject = testSubject };
        var newGoogleIdentityOwner = new GoogleIdentityOwner
        {
            Owner = storedOwner!,
            GoogleIdentity = newGoogleIdentity
        };
        await _dbContext.GoogleIdentities.AddAsync(newGoogleIdentity);
        await _dbContext.GoogleIdentityOwners.AddAsync(newGoogleIdentityOwner);
        await _dbContext.SaveChangesAsync();
        
        var storedGoogleIdentity = await _dbContext.GoogleIdentities.FirstOrDefaultAsync();
        
        var expected = new GoogleIdentityOwner
        {
            Id = new OwnerGoogleIdentityId(1),
            GoogleIdentityId = storedGoogleIdentity!.Id,
            GoogleIdentity = storedGoogleIdentity,
            OwnerId = storedOwner!.Id,
            Owner = storedOwner,
        };
        
        // Act
        var result = await _dbContext.GoogleIdentityOwners
            .Include(x => x.Owner)
            .FirstOrDefaultAsync();
        
        // Assert
        result.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public async Task StoreExpectedGoogleIdentity()
    {
        // Arrange
        _dbContext.MomentOwners.Add(new MomentOwner());
        await _dbContext.SaveChangesAsync();
        var storedOwner = await _dbContext.MomentOwners.FirstOrDefaultAsync();
        
        var testSubject = new GoogleIdentitySubject("test-subject");
        var newGoogleIdentity = new GoogleIdentity { Subject = testSubject };
        var newGoogleIdentityOwner = new GoogleIdentityOwner
        {
            Owner = storedOwner!,
            GoogleIdentity = newGoogleIdentity
        };
        await _dbContext.GoogleIdentities.AddAsync(newGoogleIdentity);
        await _dbContext.GoogleIdentityOwners.AddAsync(newGoogleIdentityOwner);
        await _dbContext.SaveChangesAsync();
        
        var expected = new GoogleIdentity { Id = new GoogleIdentityId(1), Subject = testSubject };
        
        // Act
        var result = await _dbContext.GoogleIdentities.FirstOrDefaultAsync();
        
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
        
        var testSubject = new GoogleIdentitySubject("test-subject");
        var googleIdentity = new GoogleIdentity { Subject = testSubject };
        var googleIdentityOwner = new GoogleIdentityOwner { Owner = ownerOne, GoogleIdentity = googleIdentity };

        await _dbContext.GoogleIdentities.AddAsync(googleIdentity);
        await _dbContext.GoogleIdentityOwners.AddAsync(googleIdentityOwner);
        await _dbContext.SaveChangesAsync();
        
        var duplicateGoogleIdentity = new GoogleIdentity { Subject = testSubject };
        var duplicateGoogleIdentityOwner = new GoogleIdentityOwner { Owner = ownerTwo, GoogleIdentity = duplicateGoogleIdentity};
        
        await _dbContext.GoogleIdentities.AddAsync(duplicateGoogleIdentity);
        await _dbContext.GoogleIdentityOwners.AddAsync(duplicateGoogleIdentityOwner);
        
        // Act
        var dbSaveTask = async () => await _dbContext.SaveChangesAsync();
        
        // Assert
        await dbSaveTask.Should().ThrowAsync<DbUpdateException>();
    }
    
    [Fact]
    public async Task RetrieveExpectedMetadataTimestampId()
    {
        // Arrange
        var timestamp = new MetadataTimestamp();
        await _dbContext.MetadataTimestamps.AddAsync(timestamp);
        await _dbContext.SaveChangesAsync();

        const long expected = 1;
        
        // Act
        var storedTimestamp = await _dbContext.MetadataTimestamps.FirstAsync();
        var result = storedTimestamp.Id.Id;
        
        
        // Assert
        result.Should().Be(expected);
    }
    
    [Fact]
    public async Task RetrieveExpectedMetadataTimestampDateTime()
    {
        // Arrange
        var testStartTime = DateTimeOffset.UtcNow;
        
        var timestamp = new MetadataTimestamp();
        await _dbContext.MetadataTimestamps.AddAsync(timestamp);
        await _dbContext.SaveChangesAsync();
        
        // Act
        var storedTimestamp = await _dbContext.MetadataTimestamps.FirstAsync();
        var result = storedTimestamp.Timestamp.Timestamp;
        
        
        // Assert
        result.Should().BeAfter(testStartTime);
        result.Should().BeBefore(DateTimeOffset.MaxValue);
    }
}