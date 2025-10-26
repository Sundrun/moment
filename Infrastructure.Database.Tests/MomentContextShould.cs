using Entities;

namespace Infrastructure.Database.Tests;

public class MomentContextShould
{
    [Fact]
    public void StoreAndRetrieveExpectedCoreMoment()
    {
        // Arrange
        var expectedCoreMoment = new CoreMoment(new CoreMomentId(Guid.NewGuid()), new CoreMomentTimestamp(DateTimeOffset.UtcNow));
        
        // Act
        
        // Assert
    }
}