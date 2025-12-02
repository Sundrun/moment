using Entities;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Operations.Commands.CreateMoment;
using Operations.Queries.ValidateToken;

namespace Persistence;

public class CreateMoment(MomentContext context) : ICreateMoment
{
    public async Task<ICreateMomentResponse> CreateAsync(ValidToken token)
    {
        var owner = await context.GoogleIdentityOwners
            .Include(o => o.GoogleIdentity)
            .Include(o => o.Owner)
            .FirstOrDefaultAsync(o => o.GoogleIdentity.Subject == token.Subject);

        if (owner == null)
        {
            return new NoUser();
        }
        
        var moment = new CoreMoment();
        await context.CoreMoments.AddAsync(moment);
        
        var momentOwner = new MomentOwnership()
        {
            Owner = owner.Owner,
            Moment = moment
        };
        await context.MomentOwnerships.AddAsync(momentOwner);
        
        var metadataTimestamp = new MetadataTimestamp();
        await context.MetadataTimestamps.AddAsync(metadataTimestamp);
        
        var momentTimestamp = new MomentTimestamp
        {
            Moment = moment,
            Timestamp = metadataTimestamp
        };
        await context.MomentTimestamps.AddAsync(momentTimestamp);
        
        await context.SaveChangesAsync();
        
        return new MomentCreated();
    }
}