using System.Net;
using Microsoft.Azure.Functions.Worker.Http;

namespace Functions.Extensions;

public static class HttpRequestDataCreateResponseExtension
{
    public static HttpResponseData CreateResponse(this HttpRequestData request, HttpStatusCode statusCode, string message)
    {
        var response = request.CreateResponse(statusCode);
        
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
        response.WriteStringAsync(message);
        
        return response;
        // TODO test this
    }
}