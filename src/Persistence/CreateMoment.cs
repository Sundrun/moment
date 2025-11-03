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
            .Include(o => o.Owner)
            .FirstOrDefault(o => o.GoogleIdentity.Subject == token.Subject);

        if (owner == null)
        {
            return new NoUser();
        }
        
        return new MomentCreated();
    }
}