using AwesomeAssertions;
using Entities;
using Entities.Wrappers;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Tests.MomentContextTests;

public class MomentOwnersShould : MomentContextBase
{
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
}