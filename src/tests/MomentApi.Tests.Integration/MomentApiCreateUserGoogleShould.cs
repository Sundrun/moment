using System.Net;
using AwesomeAssertions;
using Entities.Wrappers;
using Functions.Functions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using MomentApi.Tests.Integration.Fixtures;
using NSubstitute;
using Operations.Queries.ValidateToken;

namespace MomentApi.Tests.Integration;

public class ValidateTokenNewUser : IValidateToken
{
    public static GoogleIdentitySubject NewSubject { get; } = new("NewSubject");
    
    public Task<IValidatedToken> ValidateTokenAsync(string token) => Task.FromResult<IValidatedToken>(new ValidToken(NewSubject));
}

public class MomentApiCreateUserGoogleShould: HttpFunctionFixture<ValidateTokenNewUser>
{
    [Fact]
    public async Task CreateUser()
    {
        // Arrange
        var headers = new Dictionary<string, string>
        {
            { "Authorization", $"Bearer {ValidateTokenNewUser.NewSubject}" },
        };
        
        var context = Substitute.For<FunctionContext>();
        var httpRequest = Substitute.For<HttpRequestData>(context);
        httpRequest.Headers.Returns(new HttpHeadersCollection(headers));
        
        var responseData = Substitute.For<HttpResponseData>(context);
        httpRequest.CreateResponse().Returns(responseData);
        
        var function = GetService<HttpCreateUserGoogleFunction>();
        
        // Act
        var response = await function.CreateUserGoogle(httpRequest);
        var result = response.StatusCode;
        
        // Assert
        result.Should().Be(HttpStatusCode.Created);
    }
}