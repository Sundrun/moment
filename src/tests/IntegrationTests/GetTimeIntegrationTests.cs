using System;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTests
{
    // Minimal integration test that uses an externally provided connection string.
    // Set the environment variable INTEGRATION_SQL_CONN to a valid SQL Server connection string
    // (for example: "Server=localhost,1433;Database=master;User Id=sa;Password=yourStrong(!)Password;")
    public class GetTimeIntegrationTests
    {
        [Fact]
        public async Task GetServerTimeAsync_ReturnsDateTime()
        {
            var integrationConn = Environment.GetEnvironmentVariable("INTEGRATION_SQL_CONN");
            if (string.IsNullOrWhiteSpace(integrationConn))
            {
                // No integration connection provided: skip the test to avoid environment-specific failures.
                return;
            }

            // Arrange: set the expected environment variable used by SqlService
            Environment.SetEnvironmentVariable("SqlConnectionString", integrationConn);
            var service = new SqlService();

            // Act
            var dt = await service.GetServerTimeAsync();

            // Assert
            Assert.True(dt > DateTime.MinValue);
        }
    }
}
