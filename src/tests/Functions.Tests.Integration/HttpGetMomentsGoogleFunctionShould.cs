using System.Text.Json;
using AwesomeAssertions;
using Entities;
using Entities.Wrappers;
using Functions.Functions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using NSubstitute;
using Operations.Queries.GetMoments;
using Operations.Queries.ValidateToken;

namespace Functions.Tests.Integration;

public class HttpGetMomentsGoogleFunctionShould
{
    private record TestGetMomentsResponse : IGetMomentsResponse;
    
    [Fact]
    public async Task IndicateUnauthorizedWhenNoTokenIsProvided()
    {
        // Arrange
        var validateToken = Substitute.For<IValidateToken>();
        validateToken.ValidateTokenAsync(Arg.Any<string>()).Returns(new InvalidToken());

        var getMoments = Substitute.For<IGetMoments>();
        
        var func = new HttpGetMomentsGoogleFunction(validateToken, getMoments);
        
        var context = Substitute.For<FunctionContext>();
        var httpRequest = Substitute.For<HttpRequestData>(context);
        httpRequest.Headers.Returns([]);
        
        var httpResponse = Substitute.For<HttpResponseData>(context);
        httpResponse.Body.Returns(new MemoryStream());
        httpRequest.CreateResponse().Returns(httpResponse);
        
        // Act
        var response = await func.GetMomentsGoogle(httpRequest);
        var result = response.StatusCode;

        // Assert
        result.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task IndicateSuccessWhenMomentsWereRetrieved()
    {
        // Arrange
        var validateToken = Substitute.For<IValidateToken>();
        validateToken.ValidateTokenAsync(Arg.Any<string>()).Returns(new ValidToken(new GoogleIdentitySubject(string.Empty)));
        
        var moments = new List<CoreMoment>
        {
            new()
            {
                Id = new CoreMomentId(1),
                Timestamp = new CoreMomentTimestamp(DateTimeOffset.UtcNow)
            }
        };
        
        var createMoment = Substitute.For<IGetMoments>();
        createMoment.GetMomentsAsync(Arg.Any<ValidToken>()).Returns(new UserMoments(moments));
        
        var func = new HttpGetMomentsGoogleFunction(validateToken, createMoment);
        
        var headers = new Dictionary<string, string>
        {
            { "Authorization", "Bearer myTestToken" },
        };
        
        var context = Substitute.For<FunctionContext>();
        
        var httpRequest = Substitute.For<HttpRequestData>(context);
        httpRequest.Headers.Returns(new HttpHeadersCollection(headers));
        
        var httpResponse = Substitute.For<HttpResponseData>(context);
        httpResponse.Body.Returns(new MemoryStream());
        httpRequest.CreateResponse().Returns(httpResponse);
    
        // Act
        var response = await func.GetMomentsGoogle(httpRequest);
        var result = response.StatusCode;
    
        // Assert
        result.Should().Be(System.Net.HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task ReturnExpectedMomentInformation()
    {
        // Arrange
        var validateToken = Substitute.For<IValidateToken>();
        validateToken.ValidateTokenAsync(Arg.Any<string>()).Returns(new ValidToken(new GoogleIdentitySubject(string.Empty)));
        
        var moments = new List<CoreMoment>
        {
            new()
            {
                Id = new CoreMomentId(1),
                Timestamp = new CoreMomentTimestamp(DateTimeOffset.UtcNow)
            }
        };

        var expected = JsonSerializer.Serialize(moments);
        
        var getMoments = Substitute.For<IGetMoments>();
        getMoments.GetMomentsAsync(Arg.Any<ValidToken>()).Returns(new UserMoments(moments));
        
        var func = new HttpGetMomentsGoogleFunction(validateToken, getMoments);
        
        var headers = new Dictionary<string, string>
        {
            { "Authorization", "Bearer myTestToken" },
        };
        
        var context = Substitute.For<FunctionContext>();
        var httpRequest = Substitute.For<HttpRequestData>(context);
        httpRequest.Headers.Returns(new HttpHeadersCollection(headers));
        
        var httpResponse = Substitute.For<HttpResponseData>(context);
        var bodyStream = new MemoryStream();
        httpResponse.Body.Returns(bodyStream);
        httpRequest.CreateResponse().Returns(httpResponse);
    
        // Act
        await func.GetMomentsGoogle(httpRequest);
        
        bodyStream.Position = 0;
        using var reader = new StreamReader(bodyStream);
        var result = await reader.ReadToEndAsync();
    
        // Assert
        result.Should().Be(expected);
    }
    
    [Fact]
    public async Task IndicateSuccessWhenNoMomentsWereRetrieved()
    {
        // Arrange
        var validateToken = Substitute.For<IValidateToken>();
        validateToken.ValidateTokenAsync(Arg.Any<string>()).Returns(new ValidToken(new GoogleIdentitySubject(string.Empty)));
        
        var getMoments = Substitute.For<IGetMoments>();
        getMoments.GetMomentsAsync(Arg.Any<ValidToken>()).Returns(new NoMoments());
        
        var func = new HttpGetMomentsGoogleFunction(validateToken, getMoments);
        
        var headers = new Dictionary<string, string>
        {
            { "Authorization", "Bearer myTestToken" },
        };
        
        var context = Substitute.For<FunctionContext>();
        var httpRequest = Substitute.For<HttpRequestData>(context);
        httpRequest.Headers.Returns(new HttpHeadersCollection(headers));
        
        var httpResponse = Substitute.For<HttpResponseData>(context);
        httpResponse.Body.Returns(new MemoryStream());
        httpRequest.CreateResponse().Returns(httpResponse);
    
        // Act
        var response = await func.GetMomentsGoogle(httpRequest);
        var result = response.StatusCode;
    
        // Assert
        result.Should().Be(System.Net.HttpStatusCode.NoContent);
    }
    
    [Fact]
    public async Task IndicateBadRequestIfTokenDoesNotBelongToAUser()
    {
        // Arrange
        var validateToken = Substitute.For<IValidateToken>();
        validateToken.ValidateTokenAsync(Arg.Any<string>()).Returns(new ValidToken(new GoogleIdentitySubject(string.Empty)));
        
        var getMoments = Substitute.For<IGetMoments>();
        getMoments.GetMomentsAsync(Arg.Any<ValidToken>()).Returns(new NoUser());
        
        var func = new HttpGetMomentsGoogleFunction(validateToken, getMoments);
        
        var headers = new Dictionary<string, string>
        {
            { "Authorization", "Bearer myTestToken" },
        };
        
        var context = Substitute.For<FunctionContext>();
        var httpRequest = Substitute.For<HttpRequestData>(context);
        httpRequest.Headers.Returns(new HttpHeadersCollection(headers));
        
        var httpResponse = Substitute.For<HttpResponseData>(context);
        httpResponse.Body.Returns(new MemoryStream());
        httpRequest.CreateResponse().Returns(httpResponse);
    
        // Act
        var response = await func.GetMomentsGoogle(httpRequest);
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
        
        var getMoments = Substitute.For<IGetMoments>();
        
        var func = new HttpGetMomentsGoogleFunction(validateToken, getMoments);
        
        var headers = new Dictionary<string, string>
        {
            { "Authorization", "Bearer myTestToken" },
        };
        
        var context = Substitute.For<FunctionContext>();
        var httpRequest = Substitute.For<HttpRequestData>(context);
        httpRequest.Headers.Returns(new HttpHeadersCollection(headers));
        
        var httpResponse = Substitute.For<HttpResponseData>(context);
        httpResponse.Body.Returns(new MemoryStream());
        httpRequest.CreateResponse().Returns(httpResponse);
        
        // Act
        var response = await func.GetMomentsGoogle(httpRequest);
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
        
        var getMoments = Substitute.For<IGetMoments>();
        getMoments.GetMomentsAsync(Arg.Any<ValidToken>()).Returns(new TestGetMomentsResponse());
        
        var func = new HttpGetMomentsGoogleFunction(validateToken, getMoments);
        
        var headers = new Dictionary<string, string>
        {
            { "Authorization", "Bearer myTestToken" },
        };
        
        var context = Substitute.For<FunctionContext>();
        var httpRequest = Substitute.For<HttpRequestData>(context);
        httpRequest.Headers.Returns(new HttpHeadersCollection(headers));
        
        var httpResponse = Substitute.For<HttpResponseData>(context);
        httpResponse.Body.Returns(new MemoryStream());
        httpRequest.CreateResponse().Returns(httpResponse);
    
        // Act
        var response = await func.GetMomentsGoogle(httpRequest);
        var result = response.StatusCode;
    
        // Assert
        result.Should().Be(System.Net.HttpStatusCode.InternalServerError);
    }
}