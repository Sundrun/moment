using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Operations.Queries.GetMoments;
using Operations.Queries.ValidateToken;

namespace Functions.Functions;

public class HttpGetMomentsGoogleFunction(IValidateToken validateToken, IGetMoments getMoments) : IHttpFunction
{
    [Function(nameof(GetMomentsGoogle))]
    public async Task<HttpResponseData> GetMomentsGoogle([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData request)
    {
        throw new NotImplementedException();
    }
}