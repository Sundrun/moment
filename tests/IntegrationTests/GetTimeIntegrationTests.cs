using System;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Xunit;

public class GetTimeIntegrationTests : IAsyncLifetime
{
    private MsSqlTestcontainer _container;
    private bool _dockerAvailable = true;

    public async Task InitializeAsync()
    {
        // Probe Docker to see if it's available before trying to start containers.
        try
        {
            var dockerUriString = Environment.GetEnvironmentVariable("DOCKER_HOST");
            var dockerUri = string.IsNullOrEmpty(dockerUriString)
                ? new Uri("npipe://./pipe/docker_engine")
                : new Uri(dockerUriString);

            using var dockerConfig = new DockerClientConfiguration(dockerUri);
            using var docker = dockerConfig.CreateClient();
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
            // Try a lightweight system call to verify the daemon is reachable.
            await docker.System.GetSystemInfoAsync(cts.Token).ConfigureAwait(false);
        }
        catch
        {
            // Docker is not available; flag tests to skip.
            _dockerAvailable = false;
            return;
        }

        var builder = new TestcontainersBuilder<MsSqlTestcontainer>()
            .WithDatabase(new MsSqlTestcontainerConfiguration
            {
                Password = "yourStrong(!)Password",
                // Image is configured by the library for MsSqlTestcontainer; no need to assign Image here.
            })
            .WithCleanUp(true);

        _container = builder.Build();
        await _container.StartAsync();

        // ConnectionString from Testcontainers is fine; set environment variable for the library under test
        var connStr = _container.ConnectionString;
        Environment.SetEnvironmentVariable("SqlConnectionString", connStr);
    }

    public async Task DisposeAsync()
    {
        if (_container != null)
        {
            await _container.StopAsync();
        }
    }

    [Fact]
    public async Task GetServerTimeAsync_ReturnsDateTime()
    {
        if (!_dockerAvailable)
        {
            // Docker isn't reachable in this environment; return early so the test runner doesn't fail the run.
            return;
        }

        // Arrange
        var service = new SqlService();

        // Act
        var dt = await service.GetServerTimeAsync();

        // Assert
        Assert.True(dt > DateTime.MinValue);
    }
}
