using System.Net;
using Functions.CreateUser;
using Functions.Helpers;
using Functions.ValidateToken;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Functions.Functions;

public class HttpCreateUserFunction(IValidateToken validateToken, ICreateUser createUser)
{
    [Function(nameof(CreateGoogleUser))]
    public async Task<HttpResponseData> CreateGoogleUser([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData request)
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
        
        var createUserResult =  await createUser.CreateAsync(validToken);
        return createUserResult switch
        {
            UserExists => request.CreateResponse(HttpStatusCode.Conflict),
            UserCreated => request.CreateResponse(HttpStatusCode.Created),
            _ => request.CreateResponse(HttpStatusCode.InternalServerError)
        };
    }
}