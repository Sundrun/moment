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
        var testTimestamp = new CoreMomentTimestamp(DateTimeOffset.UtcNow);
        
        var newMoment = new CoreMoment{Timestamp = testTimestamp};
        await _dbContext.CoreMoments.AddAsync(newMoment);
        await _dbContext.SaveChangesAsync();
        
        var expected = new CoreMoment{Id = new CoreMomentId(1), Timestamp = testTimestamp};
        
        // Act
        var result = await _dbContext.CoreMoments.FirstOrDefaultAsync();
        
        // Assert
        result.Should().BeEquivalentTo(expected);
    }
}