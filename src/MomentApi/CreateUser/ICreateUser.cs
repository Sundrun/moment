using System.Security.Claims;

namespace MomentApi.CreateUser;

public interface ICreateUser
{
    Task<ICreateUserResponse> CreateAsync(ClaimsPrincipal principal);
}