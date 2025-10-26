using System;
using System.Threading.Tasks;
using Testcontainers.MsSql;
using Xunit;

namespace IntegrationTests
{
    // Minimal integration test that uses an externally provided connection string.
    // Set the environment variable INTEGRATION_SQL_CONN to a valid SQL Server connection string
    // (for example: "Server=localhost,1433;Database=master;User Id=sa;Password=yourStrong(!)Password;")
    public class GetTimeIntegrationTests : IAsyncLifetime
    {
        private MsSqlContainer _msSqlContainer;
        
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
        public async Task GetServerTimeAsync_ReturnsDateTime()
        {
            // Arrange: set the expected environment variable used by SqlService
            var service = new SqlService();

            // Act
            var dt = await service.GetServerTimeAsync();

            // Assert
            Assert.True(dt > DateTime.MinValue);
        }


    }
}
