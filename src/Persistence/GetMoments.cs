using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Operations.Queries.GetMoments;
using Operations.Queries.ValidateToken;

namespace Persistence;

public class GetMoments(MomentContext context) : IGetMoments
{
    public async Task<IGetMomentsResponse> GetMomentsAsync(ValidToken token, CancellationToken cancellationToken)
    {
        var owner = await context.GoogleIdentityOwners
            .Include(o => o.GoogleIdentity)
            .Include(o => o.Owner)
            .FirstOrDefaultAsync(o => o.GoogleIdentity.Subject == token.Subject, cancellationToken);
        
        if (owner == null)
        {
            return new NoUser();
        }
        
        var moments = await context.MomentOwnerships
            .Include(mo => mo.Moment)
            .Include(mo => mo.Owner)
            .Where(mo => mo.Owner.Id == owner.Owner.Id)
            .Select(mo => mo.Moment)
            .ToListAsync(cancellationToken);
        
        if (moments.Count == 0)
        {
            return new NoMoments();
        }

        return new UserMoments(moments);
    }
}