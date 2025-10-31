using Infrastructure.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistence;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration((_, config) =>
    {
        config.AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        services.AddAuthenticationServices();
        services.AddPersistenceServices();

        var tmp = context.Configuration.GetValue<string>("Google:ClientId");

        // services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        //     .AddJwtBearer(options =>
        //     {
        //         var googleConfig = context.Configuration.GetSection("Google");
        //         var clientId = googleConfig.GetValue<string>("ClientId")!;
        //         var clientSecret = googleConfig.GetValue<string>("ClientSecret")!;
        //         var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(clientSecret));
        //         
        //         // options.Authority = "https://accounts.google.com";
        //         // options.Audience = clientId;
        //         
        //         options.TokenValidationParameters = new TokenValidationParameters
        //         {
        //             ValidateIssuer = true,
        //             ValidIssuer = "https://accounts.google.com",
        //             ValidateAudience = true,
        //             ValidAudience = clientId,
        //             ValidateLifetime = true,
        //             ValidateIssuerSigningKey = true,
        //             IssuerSigningKey = signingKey
        //         };
        //         
        //         // options.TokenValidationParameters = new TokenValidationParameters
        //         // {
        //         //     ValidateIssuer = true,
        //         //     ValidateAudience = true,
        //         //     ValidateLifetime = true,
        //         //     ValidateIssuerSigningKey = true,
        //         //     ValidIssuer = "your_issuer",
        //         //     ValidAudience = "your_audience",
        //         //     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_secret_key"))
        //         // };
        //     });

    })
    .ConfigureLogging(logging => logging.AddConsole())
    .Build();

await host.RunAsync();

public partial class Program { }
