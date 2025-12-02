using AwesomeAssertions;
using Entities;
using Entities.Wrappers;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Tests.MomentContextTests;

public class CoreMomentsShould : MomentContextBase
{
    [Fact]
    public async Task RetrieveExpectedCoreMoment()
    {
        // Arrange
        var newMoment = new CoreMoment();
        await DbContext.CoreMoments.AddAsync(newMoment);
        await DbContext.SaveChangesAsync();
        
        var expected = new CoreMoment{Id = new CoreMomentId(1)};
        
        // Act
        var result = await DbContext.CoreMoments.FirstOrDefaultAsync();
        
        // Assert
        result.Should().BeEquivalentTo(expected);
    }
}