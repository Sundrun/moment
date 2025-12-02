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
        _dbContext.MomentOwners.Add(new MomentOwner());
        await _dbContext.SaveChangesAsync();
        var storedOwner = await _dbContext.MomentOwners.FirstOrDefaultAsync();
        
        var newMoment = new CoreMoment();
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
}