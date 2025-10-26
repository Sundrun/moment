using AwesomeAssertions;
using Entities;
using Testcontainers.MsSql;

namespace Infrastructure.Database.Tests;

public class MomentContextShould : IAsyncLifetime
{
    private MsSqlContainer _msSqlContainer = null!;
    
    public async Task InitializeAsync()
    {
        _msSqlContainer = new MsSqlBuilder().Build();

        await _msSqlContainer.StartAsync();
            
        var connectionString = _msSqlContainer.GetConnectionString();
        Environment.SetEnvironmentVariable("SqlConnectionString", connectionString);
    }

    public async Task DisposeAsync()
    {
        await _msSqlContainer.DisposeAsync();
    }
    
    [Fact]
    public async Task RetrieveExpectedCoreMoment()
    {
        // Arrange
        var expected = new CoreMoment(new CoreMomentId(Guid.NewGuid()), new CoreMomentTimestamp(DateTimeOffset.UtcNow));
        
        var context = new MomentContext();
        await context.Moments.AddAsync(expected);
        await context.SaveChangesAsync();
        
        // Act
        var result = context.Moments.FirstOrDefault();
        
        // Assert
        result.Should().Be(expected);
    }
}