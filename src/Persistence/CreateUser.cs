using System.Security.Claims;
using Functions.CreateUser;
using Functions.ValidateToken;

namespace Persistence;

public class CreateUser : ICreateUser
{
    private record NotImplementedResponse : ICreateUserResponse;
    
    public Task<ICreateUserResponse> CreateAsync(ValidToken token)
    {
        return Task.FromResult<ICreateUserResponse>(new NotImplementedResponse());
    }
}