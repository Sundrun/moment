using System.Security.Claims;
using Functions.CreateUser;

namespace Persistence;

public class CreateUser : ICreateUser
{
    private record NotImplementedResponse : ICreateUserResponse;
    
    public Task<ICreateUserResponse> CreateAsync(ClaimsPrincipal principal)
    {
        return Task.FromResult<ICreateUserResponse>(new NotImplementedResponse());
    }
}