using System.Security.Claims;

namespace Functions.CreateUser;

public interface ICreateUser
{
    Task<ICreateUserResponse> CreateAsync(ClaimsPrincipal principal);
}