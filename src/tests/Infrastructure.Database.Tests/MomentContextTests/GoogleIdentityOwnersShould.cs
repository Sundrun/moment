using AwesomeAssertions;
using Entities;
using Entities.Wrappers;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Tests.MomentContextTests;

public class GoogleIdentityOwnersShould : MomentContextBase
{
    [Fact]
    public async Task StoreExpectedGoogleIdentityOwner()
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
        
        var storedGoogleIdentity = await DbContext.GoogleIdentities.FirstOrDefaultAsync();
        
        var expected = new GoogleIdentityOwner
        {
            Id = new OwnerGoogleIdentityId(1),
            GoogleIdentityId = storedGoogleIdentity!.Id,
            GoogleIdentity = storedGoogleIdentity,
            OwnerId = storedOwner!.Id,
            Owner = storedOwner,
        };
        
        // Act
        var result = await DbContext.GoogleIdentityOwners
            .Include(x => x.Owner)
            .FirstOrDefaultAsync();
        
        // Assert
        result.Should().BeEquivalentTo(expected);
    }
}