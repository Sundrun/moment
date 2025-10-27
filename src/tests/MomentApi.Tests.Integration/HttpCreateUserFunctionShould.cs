using System.Security.Claims;
using AwesomeAssertions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using NSubstitute;

namespace MomentApi.Tests.Integration;

public class HttpCreateUserFunctionShould
{
    [Fact]
    public void IndicateSuccessWhenIdentityIsValid()
    {
        // Arrange
        var func = new HttpCreateUserFunction();
        
        var oidClaim = new Claim("oid", "02223b6b-aa1d-42d4-9ec0-1b2bb9194438");
        var userNameClaim = new Claim(ClaimTypes.Name, "tester");
        var claimsIdentity = new ClaimsIdentity([oidClaim, userNameClaim], "TestAuthentication");
        
        var context = Substitute.For<FunctionContext>();
        var httpRequest = Substitute.For<HttpRequestData>(context);
        httpRequest.Identities.Returns([claimsIdentity]);
        
        var httpResponse = Substitute.For<HttpResponseData>(context);
        httpRequest.CreateResponse().Returns(httpResponse);

        // Act
        var response = func.CreateUser(httpRequest);
        var result = response.StatusCode;

        // Assert
        result.Should().Be(System.Net.HttpStatusCode.OK);
    }
    
    [Fact]
    public void IndicateUnauthorizedWhenNoIdentityIsProvided()
    {
        // Arrange
        var func = new HttpCreateUserFunction();
        
        var context = Substitute.For<FunctionContext>();
        var httpRequest = Substitute.For<HttpRequestData>(context);
        
        var httpResponse = Substitute.For<HttpResponseData>(context);
        httpRequest.CreateResponse().Returns(httpResponse);


        // Act
        var response = func.CreateUser(httpRequest);
        var result = response.StatusCode;

        // Assert
        result.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }
}