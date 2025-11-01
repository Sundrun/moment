using Functions.ValidateToken;

namespace Functions.CreateMoment;

public interface ICreateMoment
{
    Task<ICreateMomentResponse> CreateAsync(ValidToken token);
}