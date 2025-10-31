using System.Net;
using Functions.CreateUser;
using Functions.Extensions;
using Functions.Helpers;
using Functions.ValidateToken;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.IdentityModel.Tokens;

namespace Functions.Functions;

public class HttpCreateUserFunction(IValidateToken validateToken, ICreateUser createUser)
{
    [Function(nameof(CreateUserAsync))]
    public async Task<HttpResponseData> CreateUserAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData request)
    {
        if (!request.TryExtractToken(out var token))
        {
            return request.CreateResponse(HttpStatusCode.Unauthorized);
        }

        var validationResult = await validateToken.ValidateTokenAsync(token);
        if (validationResult is not ValidToken validToken)
        {
            return request.CreateResponse(HttpStatusCode.Unauthorized);
        }

        var userId = validToken.Subject;
        return request.CreateResponse(HttpStatusCode.OK);
    }
}