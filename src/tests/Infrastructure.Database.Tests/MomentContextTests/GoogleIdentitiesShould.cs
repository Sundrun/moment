using AwesomeAssertions;
using Entities;
using Entities.Wrappers;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Tests.MomentContextTests;

public class GoogleIdentitiesShould : MomentContextBase
{
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
}