using AwesomeAssertions;
using Entities.Wrappers;
using Functions.Functions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using NSubstitute;
using Operations.Commands.CreateMoment;
using Operations.Queries.ValidateToken;

namespace Functions.Tests.Integration;

public class HttpCreateMomentGoogleFunctionShould
{
    private record TestCreateMomentResponse : ICreateMomentResponse;
    
    [Fact]
    public async Task IndicateUnauthorizedWhenNoTokenIsProvided()
    {
        // Arrange
        var validateToken = Substitute.For<IValidateToken>();
        validateToken.ValidateTokenAsync(Arg.Any<string>()).Returns(new InvalidToken());

        var createMoment = Substitute.For<ICreateMoment>();
        
        var func = new HttpCreateMomentGoogleFunction(validateToken, createMoment);
        
        var context = Substitute.For<FunctionContext>();
        var httpRequest = Substitute.For<HttpRequestData>(context);
        httpRequest.Headers.Returns([]);
        
        var httpResponse = Substitute.For<HttpResponseData>(context);
        httpRequest.CreateResponse().Returns(httpResponse);
        
        // Act
        var response = await func.CreateMomentGoogle(httpRequest);
        var result = response.StatusCode;

        // Assert
        result.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task IndicateSuccessWhenMomentWasAdded()
    {
        // Arrange
        var validateToken = Substitute.For<IValidateToken>();
        validateToken.ValidateTokenAsync(Arg.Any<string>()).Returns(new ValidToken(new GoogleIdentitySubject(string.Empty)));
        
        var createMoment = Substitute.For<ICreateMoment>();
        createMoment.CreateAsync(Arg.Any<ValidToken>()).Returns(new MomentCreated());
        
        var func = new HttpCreateMomentGoogleFunction(validateToken, createMoment);
        
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
        var response = await func.CreateMomentGoogle(httpRequest);
        var result = response.StatusCode;
    
        // Assert
        result.Should().Be(System.Net.HttpStatusCode.Created);
    }
    
    [Fact]
    public async Task IndicateBadRequestIfTokenDoesNotBelongToAUser()
    {
        // Arrange
        var validateToken = Substitute.For<IValidateToken>();
        validateToken.ValidateTokenAsync(Arg.Any<string>()).Returns(new ValidToken(new GoogleIdentitySubject(string.Empty)));
        
        var createMoment = Substitute.For<ICreateMoment>();
        createMoment.CreateAsync(Arg.Any<ValidToken>()).Returns(new NoUser());
        
        var func = new HttpCreateMomentGoogleFunction(validateToken, createMoment);
        
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
        var response = await func.CreateMomentGoogle(httpRequest);
        var result = response.StatusCode;
    
        // Assert
        result.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task IndicateUnauthorizedWhenTokenIsInvalid()
    {
        // Arrange
        var validateToken = Substitute.For<IValidateToken>();
        validateToken.ValidateTokenAsync(Arg.Any<string>()).Returns(new InvalidToken());
        
        var createMoment = Substitute.For<ICreateMoment>();
        
        var func = new HttpCreateMomentGoogleFunction(validateToken, createMoment);
        
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
        var response = await func.CreateMomentGoogle(httpRequest);
        var result = response.StatusCode;
    
        // Assert
        result.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task IndicateErrorWhenUserCreatedResultIsUnexpected()
    {
        // Arrange
        var validateToken = Substitute.For<IValidateToken>();
        validateToken.ValidateTokenAsync(Arg.Any<string>()).Returns(new ValidToken(new GoogleIdentitySubject(string.Empty)));
        
        var createMoment = Substitute.For<ICreateMoment>();
        createMoment.CreateAsync(Arg.Any<ValidToken>()).Returns(new TestCreateMomentResponse());
        
        var func = new HttpCreateMomentGoogleFunction(validateToken, createMoment);
        
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
        var response = await func.CreateMomentGoogle(httpRequest);
        var result = response.StatusCode;
    
        // Assert
        result.Should().Be(System.Net.HttpStatusCode.InternalServerError);
    }
}