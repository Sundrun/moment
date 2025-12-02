using AwesomeAssertions;
using Entities;
using Entities.Wrappers;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Tests.MomentContextTests;

public class MetadataTimestampsShould : MomentContextBase
{
    [Fact]
    public async Task RetrieveExpectedMetadataTimestampId()
    {
        // Arrange
        var timestamp = new MetadataTimestamp();
        await _dbContext.MetadataTimestamps.AddAsync(timestamp);
        await _dbContext.SaveChangesAsync();

        const long expected = 1;
        
        // Act
        var storedTimestamp = await _dbContext.MetadataTimestamps.FirstAsync();
        var result = storedTimestamp.Id.Id;
        
        // Assert
        result.Should().Be(expected);
    }
    
    [Fact]
    public async Task RetrieveExpectedMetadataTimestampDateTime()
    {
        // Arrange
        var testStartTime = DateTimeOffset.UtcNow;
        
        var timestamp = new MetadataTimestamp();
        await _dbContext.MetadataTimestamps.AddAsync(timestamp);
        await _dbContext.SaveChangesAsync();
        
        // Act
        var storedTimestamp = await _dbContext.MetadataTimestamps.FirstAsync();
        var result = storedTimestamp.Timestamp.Timestamp;
        
        
        // Assert
        result.Should().BeAfter(testStartTime);
        result.Should().BeBefore(DateTimeOffset.MaxValue);
    }
}