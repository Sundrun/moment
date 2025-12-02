using AwesomeAssertions;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Tests.MomentContextTests;

public class MetadataTimestampsShould : MomentContextBase
{
    [Fact]
    public async Task RetrieveExpectedMetadataTimestampId()
    {
        // Arrange
        var timestamp = new MetadataTimestamp();
        await DbContext.MetadataTimestamps.AddAsync(timestamp);
        await DbContext.SaveChangesAsync();

        const long expected = 1;
        
        // Act
        var storedTimestamp = await DbContext.MetadataTimestamps.FirstAsync();
        var result = storedTimestamp.Id.Id;
        
        // Assert
        result.Should().Be(expected);
    }
    
    [Fact]
    public async Task RetrieveExpectedMetadataTimestampDateTime()
    {
        // Arrange
        var timestamp = new MetadataTimestamp();
        await DbContext.MetadataTimestamps.AddAsync(timestamp);
        await DbContext.SaveChangesAsync();
        
        // Act
        var storedTimestamp = await DbContext.MetadataTimestamps.FirstAsync();
        var result = storedTimestamp.Timestamp.Timestamp;
        
        
        // Assert
        result.Should().BeAfter(TestStartTime);
        result.Should().BeBefore(DateTimeOffset.MaxValue);
    }
}