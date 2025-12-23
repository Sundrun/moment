using Operations.Queries.ValidateToken;

namespace Operations.Commands.CreateMoment;

public interface ICreateMoment
{
    Task<ICreateMomentResponse> CreateAsync(ValidToken token, CancellationToken cancellationToken);
}