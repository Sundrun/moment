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
        var owner = new MomentOwner();
        await context.MomentOwners.AddAsync(owner);
        
        var identity = new OwnerGoogleIdentity
        {
            Owner = owner,
            Subject = token.Subject
        };
        await context.OwnerGoogleIdentities.AddAsync(identity);
        
        await context.SaveChangesAsync();

        return new UserCreated();
    }
}