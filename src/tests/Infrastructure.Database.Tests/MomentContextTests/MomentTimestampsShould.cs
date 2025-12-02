using AwesomeAssertions;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Tests.MomentContextTests;

public class MomentTimestampsShould : MomentContextBase
{
    [Fact]
    public async Task RetrieveMomentTimestampsWithExpectedTimestamp()
    {
        // Arrange
        var metadataTimestamp = new MetadataTimestamp();
        await DbContext.MetadataTimestamps.AddAsync(metadataTimestamp);
        
        var moment = new CoreMoment();
        await DbContext.CoreMoments.AddAsync(moment);
        
        var momentTimestamp = new MomentTimestamp{ Moment = moment, Timestamp = metadataTimestamp };
        await DbContext.MomentTimestamps.AddAsync(momentTimestamp);
        
        await DbContext.SaveChangesAsync();
        
        var expected = await DbContext.MetadataTimestamps.FirstAsync();
        
        // Act
        var storedMomentTimestamp = await DbContext.MomentTimestamps
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
        await DbContext.MetadataTimestamps.AddAsync(metadataTimestamp);
        
        var moment = new CoreMoment();
        await DbContext.CoreMoments.AddAsync(moment);
        
        var momentTimestamp = new MomentTimestamp{ Moment = moment, Timestamp = metadataTimestamp };
        await DbContext.MomentTimestamps.AddAsync(momentTimestamp);
        
        await DbContext.SaveChangesAsync();
        
        var expected = await DbContext.CoreMoments.FirstAsync();
        
        // Act
        var storedMomentTimestamp = await DbContext.MomentTimestamps
            .Include(x => x.Moment)
            .Include(x => x.Timestamp)
            .FirstAsync();
        var result = storedMomentTimestamp.Moment;
        
        // Assert
        result.Should().BeEquivalentTo(expected);
    }
}