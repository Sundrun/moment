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
}