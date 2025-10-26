using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace MomentApi;

public class HttpSqlFunction(SqlService sqlService)
{
    [Function(nameof(GetTime))]
    public async Task<HttpResponseData> GetTime([HttpTrigger(AuthorizationLevel.Function, "get", Route = "time")] HttpRequestData req)
    {
        var response = req.CreateResponse();
        try
        {
            var time = await sqlService.GetServerTimeAsync();
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