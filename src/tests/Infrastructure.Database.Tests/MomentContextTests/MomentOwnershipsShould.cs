using AwesomeAssertions;
using Entities;
using Entities.Wrappers;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Tests.MomentContextTests;

public class MomentOwnershipsShould : MomentContextBase
{
    [Fact]
    public async Task RetrieveExpectedMomentOwnership()
    {
        // Arrange
        DbContext.MomentOwners.Add(new MomentOwner());
        await DbContext.SaveChangesAsync();
        var storedOwner = await DbContext.MomentOwners.FirstOrDefaultAsync();
        
        var newMoment = new CoreMoment();
        DbContext.CoreMoments.Add(newMoment);
        
        var newOwnership = new MomentOwnership{ Moment = newMoment, Owner = storedOwner!};
        await DbContext.MomentOwnerships.AddAsync(newOwnership);
        await DbContext.SaveChangesAsync();
        
        var storedMoment = await DbContext.CoreMoments.FirstOrDefaultAsync();
        
        var expected = new MomentOwnership
        {
            Id = new MomentOwnershipId(1),
            MomentId = storedMoment!.Id,
            Moment = storedMoment,
            OwnerId = storedOwner!.Id,
            Owner = storedOwner
        };
        
        // Act
        var result = await DbContext.MomentOwnerships
            .Include(x => x.Moment)
            .Include(x => x.Owner)
            .FirstOrDefaultAsync();
        
        // Assert
        result.Should().BeEquivalentTo(expected);
    }
}