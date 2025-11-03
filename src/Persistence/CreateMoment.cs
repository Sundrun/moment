using Infrastructure.Database;
using Operations.Commands.CreateMoment;
using Operations.Queries.ValidateToken;

namespace Persistence;

public class CreateMoment(MomentContext context) : ICreateMoment
{
    public async Task<ICreateMomentResponse> CreateAsync(ValidToken token)
    {
        return new MomentCreated();
    }
}