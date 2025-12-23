using Entities;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Operations.Commands.CreateMoment;
using Operations.Queries.ValidateToken;

namespace Persistence;

public class CreateMoment(MomentContext context) : ICreateMoment
{
    public async Task<ICreateMomentResponse> CreateAsync(ValidToken token, CancellationToken cancellationToken)
    {
        var owner = await context.GoogleIdentityOwners
            .Include(o => o.GoogleIdentity)
            .Include(o => o.Owner)
            .FirstOrDefaultAsync(o => o.GoogleIdentity.Subject == token.Subject, cancellationToken);

        if (owner == null)
        {
            return new NoUser();
        }
        
        var moment = new CoreMoment();
        await context.CoreMoments.AddAsync(moment, cancellationToken);
        
        var momentOwner = new MomentOwnership()
        {
            Owner = owner.Owner,
            Moment = moment
        };
        await context.MomentOwnerships.AddAsync(momentOwner, cancellationToken);
        
        var metadataTimestamp = new MetadataTimestamp();
        await context.MetadataTimestamps.AddAsync(metadataTimestamp, cancellationToken);
        
        var momentTimestamp = new MomentTimestamp
        {
            Moment = moment,
            Timestamp = metadataTimestamp
        };
        await context.MomentTimestamps.AddAsync(momentTimestamp, cancellationToken);
        
        await context.SaveChangesAsync(cancellationToken);
        
        return new MomentCreated();
    }
}