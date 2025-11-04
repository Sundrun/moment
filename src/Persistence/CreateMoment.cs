using Entities;
using Entities.Wrappers;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Operations.Commands.CreateMoment;
using Operations.Queries.ValidateToken;

namespace Persistence;

public class CreateMoment(MomentContext context) : ICreateMoment
{
    public async Task<ICreateMomentResponse> CreateAsync(ValidToken token)
    {
        var owner = context.GoogleIdentityOwners
            .Include(o => o.GoogleIdentity)
            .Include(o => o.Owner)
            .FirstOrDefault(o => o.GoogleIdentity.Subject == token.Subject);

        if (owner == null)
        {
            return new NoUser();
        }
        
        var moment = new CoreMoment
        {
            Timestamp = new CoreMomentTimestamp(DateTimeOffset.UtcNow)
        };
        
        var momentOwner = new MomentOwnership()
        {
            Owner = owner.Owner,
            Moment = moment
        };
        
        await context.CoreMoments.AddAsync(moment);
        await context.MomentOwnerships.AddAsync(momentOwner);
        await context.SaveChangesAsync();
        
        return new MomentCreated();
    }
}