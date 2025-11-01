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
        
        var identity = new GoogleIdentity
        {
            Subject = token.Subject,
        };
        var identityOwner = new GoogleIdentityOwner
        {
            Owner = owner,
            GoogleIdentity = identity
        };
        await context.GoogleIdentities.AddAsync(identity);
        await context.GoogleIdentityOwners.AddAsync(identityOwner);
        
        await context.SaveChangesAsync();

        return new UserCreated();
    }
    
    private bool HasAlreadyBeenCreated(ValidToken token)
    {
        var existingIdentity = context.GoogleIdentities
            .FirstOrDefault(i => i.Subject == token.Subject);

        return existingIdentity != null;
    }
}