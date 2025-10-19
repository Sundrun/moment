using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

public class HttpSqlFunction
{
    private readonly SqlService _sqlService;

    public HttpSqlFunction(SqlService sqlService)
    {
        _sqlService = sqlService;
    }

    [Function("GetTimeFromDb")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "time")] HttpRequestData req)
    {
        var response = req.CreateResponse();
        try
        {
            var time = await _sqlService.GetServerTimeAsync();
            await response.WriteStringAsync($"DB Server Time: {time}");
            response.StatusCode = HttpStatusCode.OK;
        }
        catch (Exception ex)
        {
            response.StatusCode = HttpStatusCode.InternalServerError;
            await response.WriteStringAsync($"Error: {ex.Message}");
        }

        return response;
    }
}
