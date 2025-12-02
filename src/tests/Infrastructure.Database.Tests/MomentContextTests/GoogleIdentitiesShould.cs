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
        DbContext.MomentOwners.Add(new MomentOwner());
        await DbContext.SaveChangesAsync();
        var storedOwner = await DbContext.MomentOwners.FirstOrDefaultAsync();
        
        var testSubject = new GoogleIdentitySubject("test-subject");
        var newGoogleIdentity = new GoogleIdentity { Subject = testSubject };
        var newGoogleIdentityOwner = new GoogleIdentityOwner
        {
            Owner = storedOwner!,
            GoogleIdentity = newGoogleIdentity
        };
        await DbContext.GoogleIdentities.AddAsync(newGoogleIdentity);
        await DbContext.GoogleIdentityOwners.AddAsync(newGoogleIdentityOwner);
        await DbContext.SaveChangesAsync();
        
        var expected = new GoogleIdentity { Id = new GoogleIdentityId(1), Subject = testSubject };
        
        // Act
        var result = await DbContext.GoogleIdentities.FirstOrDefaultAsync();
        
        // Assert
        result.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public async Task NotAllowDuplicateGoogleIdentities()
    {
        // Arrange
        DbContext.MomentOwners.Add(new MomentOwner());
        DbContext.MomentOwners.Add(new MomentOwner());
        await DbContext.SaveChangesAsync();
        
        var ownerOne = await DbContext.MomentOwners.FirstAsync(x => x.Id == new MomentOwnerId(1));
        var ownerTwo = await DbContext.MomentOwners.FirstAsync(x => x.Id == new MomentOwnerId(2));
        
        var testSubject = new GoogleIdentitySubject("test-subject");
        var googleIdentity = new GoogleIdentity { Subject = testSubject };
        var googleIdentityOwner = new GoogleIdentityOwner { Owner = ownerOne, GoogleIdentity = googleIdentity };

        await DbContext.GoogleIdentities.AddAsync(googleIdentity);
        await DbContext.GoogleIdentityOwners.AddAsync(googleIdentityOwner);
        await DbContext.SaveChangesAsync();
        
        var duplicateGoogleIdentity = new GoogleIdentity { Subject = testSubject };
        var duplicateGoogleIdentityOwner = new GoogleIdentityOwner { Owner = ownerTwo, GoogleIdentity = duplicateGoogleIdentity};
        
        await DbContext.GoogleIdentities.AddAsync(duplicateGoogleIdentity);
        await DbContext.GoogleIdentityOwners.AddAsync(duplicateGoogleIdentityOwner);
        
        // Act
        var dbSaveTask = async () => await DbContext.SaveChangesAsync();
        
        // Assert
        await dbSaveTask.Should().ThrowAsync<DbUpdateException>();
    }
}