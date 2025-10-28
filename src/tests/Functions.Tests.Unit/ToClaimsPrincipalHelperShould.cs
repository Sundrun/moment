using System.Security.Claims;
using AwesomeAssertions;
using Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using NSubstitute;

namespace Functions.Tests.Unit;

public class ToClaimsPrincipalHelperShould
{
    [Fact]
    public void IndicateFailureIfRequestDoesNotContainIdentityInformation()
    {
        // Arrange
        var request = HttpRequest();

        // Act
        var result = ToClaimsPrincipalHelper.TryConvertToClaimsPrincipal(request, out _);

        // Assert
        result.Should().BeFalse();
    }
    
    [Fact]
    public void IndicateSuccessWhenAbleToConvert()
    {
        // Arrange
        var claimsIdentity = MockClaim();
        var request = HttpRequest(claimsIdentity);

        // Act
        var result = ToClaimsPrincipalHelper.TryConvertToClaimsPrincipal(request, out _);

        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public void CreateExpectedPrincipalWhenAbleToConvert()
    {
        // Arrange
        var claimsIdentity = MockClaim();
        var request = HttpRequest(claimsIdentity);
        var expected = new ClaimsPrincipal(claimsIdentity);

        // Act
        ToClaimsPrincipalHelper.TryConvertToClaimsPrincipal(request, out var result);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    private static ClaimsIdentity MockClaim()
    {
        var oidClaim = new Claim("oid", "02223b6b-aa1d-42d4-9ec0-1b2bb9194438");
        var userNameClaim = new Claim(ClaimTypes.Name, "tester");
        var claimsIdentity = new ClaimsIdentity([oidClaim, userNameClaim], "TestAuthentication");
        return claimsIdentity;
    }
    
    private static HttpRequestData HttpRequest(params ClaimsIdentity[] claimsIdentities)
    {
        var context = Substitute.For<FunctionContext>();
        var request = Substitute.For<HttpRequestData>(context);
        request.Identities.Returns(claimsIdentities);
        return request;
    }
}