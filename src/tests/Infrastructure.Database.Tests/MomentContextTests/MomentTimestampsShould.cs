using AwesomeAssertions;
using Entities;
using Entities.Wrappers;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Tests.MomentContextTests;

public class MomentTimestampsShould : MomentContextBase
{
    [Fact]
    public async Task RetrieveMomentTimestampsWithExpectedTimestamp()
    {
        // Arrange
        var metadataTimestamp = new MetadataTimestamp();
        await _dbContext.MetadataTimestamps.AddAsync(metadataTimestamp);
        
        var moment = new CoreMoment();
        await _dbContext.CoreMoments.AddAsync(moment);
        
        var momentTimestamp = new MomentTimestamp{ Moment = moment, Timestamp = metadataTimestamp };
        await _dbContext.MomentTimestamps.AddAsync(momentTimestamp);
        
        await _dbContext.SaveChangesAsync();
        
        var expected = await _dbContext.MetadataTimestamps.FirstAsync();
        
        // Act
        var storedMomentTimestamp = await _dbContext.MomentTimestamps
            .Include(x => x.Moment)
            .Include(x => x.Timestamp)
            .FirstAsync();
        var result = storedMomentTimestamp.Timestamp;
        
        // Assert
        result.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public async Task RetrieveMomentTimestampsWithExpectedMoment()
    {
        // Arrange
        var metadataTimestamp = new MetadataTimestamp();
        await _dbContext.MetadataTimestamps.AddAsync(metadataTimestamp);
        
        var moment = new CoreMoment();
        await _dbContext.CoreMoments.AddAsync(moment);
        
        var momentTimestamp = new MomentTimestamp{ Moment = moment, Timestamp = metadataTimestamp };
        await _dbContext.MomentTimestamps.AddAsync(momentTimestamp);
        
        await _dbContext.SaveChangesAsync();
        
        var expected = await _dbContext.CoreMoments.FirstAsync();
        
        // Act
        var storedMomentTimestamp = await _dbContext.MomentTimestamps
            .Include(x => x.Moment)
            .Include(x => x.Timestamp)
            .FirstAsync();
        var result = storedMomentTimestamp.Moment;
        
        // Assert
        result.Should().BeEquivalentTo(expected);
    }
}