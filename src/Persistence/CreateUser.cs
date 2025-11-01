using System.Security.Claims;
using Entities;
using Functions.CreateUser;
using Functions.ValidateToken;
using Infrastructure.Database;

namespace Persistence;

public class CreateUser(MomentContext context) : ICreateUser
{
    
    public async Task<ICreateUserResponse> CreateAsync(ValidToken token)
    {
        await context.MomentOwners.AddAsync(new MomentOwner());
        await context.SaveChangesAsync();

        return new UserCreated();
    }
}