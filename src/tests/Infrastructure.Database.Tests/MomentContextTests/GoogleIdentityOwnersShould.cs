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
}