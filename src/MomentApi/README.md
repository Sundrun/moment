# MomentApi

This is a minimal .NET 8 isolated Azure Functions app that demonstrates connecting to an Azure SQL Database using `Microsoft.Data.SqlClient`.

Files added:
- `Program.cs` - host bootstrap and DI registration
- `HttpSqlFunction.cs` - HTTP-triggered function at route `/api/time` that returns the database server time
- `SqlService.cs` - simple helper service that reads `SqlConnectionString` from environment variables and queries `SYSDATETIME()`
- `local.settings.json.template` - template for local settings including the `SqlConnectionString`

Setup
1. Install .NET 8 SDK.
2. Install Azure Functions Core Tools (v4) if you want to run locally.
3. Copy `local.settings.json.template` to `local.settings.json` and set your database connection string in `SqlConnectionString`.

Run locally
1. From the `MomentApi` folder run `dotnet build` to restore packages and build.
2. Start the function host with Azure Functions Core Tools or `func start`.
3. Call the function: `GET http://localhost:7071/api/time` (add function key if required).

Notes
- Keep secrets out of source control. Use Azure Key Vault or application settings in Azure Function App when deploying.
- The project expects `SqlConnectionString` to be present as an app setting.
