using Operations.Queries.ValidateToken;

namespace Operations.Queries.GetMoments;

public interface IGetMoments
{
    Task<IGetMomentsResponse> GetMomentsAsync(ValidToken token, CancellationToken cancellationToken);
}