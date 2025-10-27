using Microsoft.Azure.Functions.Worker;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MomentApi;

public class HttpCreateUserFunction
{
    [Function(nameof(CreateUser))]
    public IActionResult CreateUser([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        return new OkObjectResult("User created successfully.");
    }
}