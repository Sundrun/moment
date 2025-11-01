using System.Security.Claims;
using Entities;
using Functions.CreateUser;
using Functions.ValidateToken;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public class CreateUser(MomentContext context) : ICreateUser
{
    
    public async Task<ICreateUserResponse> CreateAsync(ValidToken token)
    {
        if (HasAlreadyBeenCreated(token))
        {
            return new UserExists();
        }
        
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
    
    private bool HasAlreadyBeenCreated(ValidToken token)
    {
        var existingIdentity = context.OwnerGoogleIdentities
            .FirstOrDefault(i => i.Subject == token.Subject);

        return existingIdentity != null;
    }
}