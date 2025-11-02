using Functions.CreateUser;
using Operations.Queries.ValidateToken;

namespace Operations.Commands.CreateUser;

public interface ICreateUser
{
    Task<ICreateUserResponse> CreateAsync(ValidToken token);
}