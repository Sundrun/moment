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
        await DbContext.MomentOwners.AddAsync(newOwner);
        await DbContext.SaveChangesAsync();

        var expected = new MomentOwner{Id = new MomentOwnerId(1)};
        
        // Act
        var result = await DbContext.MomentOwners.FirstOrDefaultAsync();
        
        // Assert
        result.Should().BeEquivalentTo(expected);
    }
}