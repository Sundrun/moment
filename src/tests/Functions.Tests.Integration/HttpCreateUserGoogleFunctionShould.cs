using System.Security.Claims;
using AwesomeAssertions;
using Entities.Wrappers;
using Functions.CreateUser;
using Functions.Functions;
using Functions.ValidateToken;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace Functions.Tests.Integration;

public class HttpCreateUserGoogleFunctionShould
{
    private record TestCreateUserResult : ICreateUserResponse;
    
    [Fact]
    public async Task IndicateSuccessWhenIdentityIsValid()
    {
        // Arrange
        var validateToken = Substitute.For<IValidateToken>();
        validateToken.ValidateTokenAsync(Arg.Any<string>()).Returns(new ValidToken(new GoogleIdentitySubject(string.Empty)));
        
        var createUser = Substitute.For<ICreateUser>();
        createUser.CreateAsync(Arg.Any<ValidToken>()).Returns(new UserCreated());
        var func = new HttpCreateUserGoogleFunction(validateToken, createUser);
        
        var headers = new Dictionary<string, string>
        {
            { "Authorization", "Bearer myTestToken" },
        };
        
        var context = Substitute.For<FunctionContext>();
        var httpRequest = Substitute.For<HttpRequestData>(context);
        httpRequest.Headers.Returns(new HttpHeadersCollection(headers));
        
        var httpResponse = Substitute.For<HttpResponseData>(context);
        httpRequest.CreateResponse().Returns(httpResponse);

        // Act
        var response = await func.CreateUserGoogle(httpRequest);
        var result = response.StatusCode;

        // Assert
        result.Should().Be(System.Net.HttpStatusCode.Created);
    }
    
    [Fact]
    public async Task IndicateUnauthorizedWhenTokenIsInvalid()
    {
        // Arrange
        var validateToken = Substitute.For<IValidateToken>();
        validateToken.ValidateTokenAsync(Arg.Any<string>()).Returns(new InvalidToken());
        
        var createUser = Substitute.For<ICreateUser>();
        createUser.CreateAsync(Arg.Any<ValidToken>()).Returns(new UserCreated());
        var func = new HttpCreateUserGoogleFunction(validateToken, createUser);
        
        var headers = new Dictionary<string, string>
        {
            { "Authorization", "Bearer myTestToken" },
        };
        
        var context = Substitute.For<FunctionContext>();
        var httpRequest = Substitute.For<HttpRequestData>(context);
        httpRequest.Headers.Returns(new HttpHeadersCollection(headers));
        
        var httpResponse = Substitute.For<HttpResponseData>(context);
        httpRequest.CreateResponse().Returns(httpResponse);
        
        // Act
        var response = await func.CreateUserGoogle(httpRequest);
        var result = response.StatusCode;

        // Assert
        result.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task IndicateUnauthorizedWhenNoTokenIsProvided()
    {
        // Arrange
        var validateToken = Substitute.For<IValidateToken>();
        validateToken.ValidateTokenAsync(Arg.Any<string>()).Returns(new InvalidToken());
        
        var createUser = Substitute.For<ICreateUser>();
        createUser.CreateAsync(Arg.Any<ValidToken>()).Returns(new UserCreated());
        var func = new HttpCreateUserGoogleFunction(validateToken, createUser);
        
        var context = Substitute.For<FunctionContext>();
        var httpRequest = Substitute.For<HttpRequestData>(context);
        httpRequest.Headers.Returns([]);
        
        var httpResponse = Substitute.For<HttpResponseData>(context);
        httpRequest.CreateResponse().Returns(httpResponse);
        
        // Act
        var response = await func.CreateUserGoogle(httpRequest);
        var result = response.StatusCode;

        // Assert
        result.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task IndicateConflictIfUserAlreadyExists()
    {
        // Arrange
        var validateToken = Substitute.For<IValidateToken>();
        validateToken.ValidateTokenAsync(Arg.Any<string>()).Returns(new ValidToken(new GoogleIdentitySubject(string.Empty)));
        
        var createUser = Substitute.For<ICreateUser>();
        createUser.CreateAsync(Arg.Any<ValidToken>()).Returns(new UserExists());
        var func = new HttpCreateUserGoogleFunction(validateToken, createUser);
        
        var headers = new Dictionary<string, string>
        {
            { "Authorization", "Bearer myTestToken" },
        };
        
        var context = Substitute.For<FunctionContext>();
        var httpRequest = Substitute.For<HttpRequestData>(context);
        httpRequest.Headers.Returns(new HttpHeadersCollection(headers));
        
        var httpResponse = Substitute.For<HttpResponseData>(context);
        httpRequest.CreateResponse().Returns(httpResponse);

        // Act
        var response = await func.CreateUserGoogle(httpRequest);
        var result = response.StatusCode;

        // Assert
        result.Should().Be(System.Net.HttpStatusCode.Conflict);
    }
    
    [Fact]
    public async Task IndicateErrorWhenUserCreatedResultIsUnexpected()
    {
        // Arrange
        var validateToken = Substitute.For<IValidateToken>();
        validateToken.ValidateTokenAsync(Arg.Any<string>()).Returns(new ValidToken(new GoogleIdentitySubject(string.Empty)));
        
        var createUser = Substitute.For<ICreateUser>();
        createUser.CreateAsync(Arg.Any<ValidToken>()).Returns(new TestCreateUserResult());
        var func = new HttpCreateUserGoogleFunction(validateToken, createUser);
        
        var headers = new Dictionary<string, string>
        {
            { "Authorization", "Bearer myTestToken" },
        };
        
        var context = Substitute.For<FunctionContext>();
        var httpRequest = Substitute.For<HttpRequestData>(context);
        httpRequest.Headers.Returns(new HttpHeadersCollection(headers));
        
        var httpResponse = Substitute.For<HttpResponseData>(context);
        httpRequest.CreateResponse().Returns(httpResponse);

        // Act
        var response = await func.CreateUserGoogle(httpRequest);
        var result = response.StatusCode;

        // Assert
        result.Should().Be(System.Net.HttpStatusCode.InternalServerError);
    }
    
    [Fact]
    public async Task IndicateErrorWhenNoUserCreatedResultIsGenerated()
    {
        // Arrange
        var validateToken = Substitute.For<IValidateToken>();
        validateToken.ValidateTokenAsync(Arg.Any<string>()).Returns(new ValidToken(new GoogleIdentitySubject(string.Empty)));
        
        var createUser = Substitute.For<ICreateUser>();
        createUser.CreateAsync(Arg.Any<ValidToken>()).ReturnsNull();
        var func = new HttpCreateUserGoogleFunction(validateToken, createUser);
        
        var headers = new Dictionary<string, string>
        {
            { "Authorization", "Bearer myTestToken" },
        };
        
        var context = Substitute.For<FunctionContext>();
        var httpRequest = Substitute.For<HttpRequestData>(context);
        httpRequest.Headers.Returns(new HttpHeadersCollection(headers));
        
        var httpResponse = Substitute.For<HttpResponseData>(context);
        httpRequest.CreateResponse().Returns(httpResponse);

        // Act
        var response = await func.CreateUserGoogle(httpRequest);
        var result = response.StatusCode;

        // Assert
        result.Should().Be(System.Net.HttpStatusCode.InternalServerError);
    }
}