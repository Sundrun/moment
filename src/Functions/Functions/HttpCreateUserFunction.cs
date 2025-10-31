using System.Net;
using Functions.CreateUser;
using Functions.Extensions;
using Functions.Helpers;
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

        TokenValidationResult validationResult;
        try
        {
            validationResult =  await validateToken.ValidateTokenAsync(token);
        }
        catch (SecurityTokenExpiredException)
        {
            return request.CreateResponse(HttpStatusCode.Unauthorized, "Token has expired.");
        }
        catch (Exception ex)
        {
            return request.CreateResponse(HttpStatusCode.Unauthorized, $"Token validation failed: {ex.Message}");
        }
        
        if (!validationResult.IsValid)
        {
            return request.CreateResponse(HttpStatusCode.Unauthorized, "Invalid token.");
        }

        var identity = validationResult.ClaimsIdentity;
        return request.CreateResponse(HttpStatusCode.OK);
        
        
        // var result = await createUser.CreateAsync(principal);
        // var responseCode = ToStatusCodeHelper.ToStatusCode(result);
        //
        // return request.CreateResponse(responseCode);
    }
}