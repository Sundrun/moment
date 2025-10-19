using Microsoft.Data.SqlClient;

public class SqlService
{
    private readonly string _connectionString;

    public SqlService()
    {
        // Expect connection string in environment variable or Azure Function app setting named 'SqlConnectionString'
        _connectionString = Environment.GetEnvironmentVariable("SqlConnectionString") ?? throw new InvalidOperationException("SqlConnectionString is not configured.");
    }

    public async Task<DateTime> GetServerTimeAsync()
    {
        using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT SYSDATETIME()";
        var result = await cmd.ExecuteScalarAsync();
        if (result is DateTime dt)
            return dt;
        if (result is DateTimeOffset dto)
            return dto.DateTime;
        throw new InvalidOperationException("Unexpected result from database.");
    }
}
