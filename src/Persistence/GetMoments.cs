using Infrastructure.Database;
using Operations.Queries.GetMoments;
using Operations.Queries.ValidateToken;

namespace Persistence;

public class GetMoments(MomentContext context) : IGetMoments
{
    public Task<IGetMomentsResponse> GetMomentsAsync(ValidToken token)
    {
        return Task.FromResult<IGetMomentsResponse>(null!);
    }
}