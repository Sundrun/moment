using System.Security.Claims;
using Functions.ValidateToken;

namespace Functions.CreateUser;

public interface ICreateUser
{
    Task<ICreateUserResponse> CreateAsync(ValidToken token);
}