using Microsoft.Extensions.Hosting;
using MomentApi.Extensions;

var host = new HostBuilder().ConfigureMomentApi().Build();

await host.RunAsync();

public partial class Program { }
