using Entities;

namespace Operations.Queries.GetMoments;

public record UserMoments(IEnumerable<CoreMoment> Moments) : IGetMomentsResponse;