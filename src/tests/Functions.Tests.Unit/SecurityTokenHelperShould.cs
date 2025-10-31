using AwesomeAssertions;
using Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using NSubstitute;

namespace Functions.Tests.Unit;

public class SecurityTokenHelperShould
{
    [Fact]
    public void IndicateFailureWhenNoHeadersArePresent()
    {
        // Arrange
        var context = Substitute.For<FunctionContext>();
        var httpRequest = Substitute.For<HttpRequestData>(context);
        httpRequest.Headers.Returns([]);
        
        // Act
        var result = httpRequest.TryExtractToken(out _);
        
        // Assert
        result.Should().BeFalse();
    }
    
    [Fact]
    public void IndicateFailureWhenHeadersToNotIncludeAuthorizationInformation()
    {
        // Arrange
        var headers = new Dictionary<string, string>
        {
            { "X-Custom-Header", "Custom header value" },
        };
        
        var context = Substitute.For<FunctionContext>();
        var httpRequest = Substitute.For<HttpRequestData>(context);
        httpRequest.Headers.Returns(new HttpHeadersCollection(headers));
        
        // Act
        var result = httpRequest.TryExtractToken(out _);
        
        // Assert
        result.Should().BeFalse();
    }
    
    [Fact]
    public void IndicateFailureWhenAuthenticationIsNotOfBearerType()
    {
        // Arrange
        var headers = new Dictionary<string, string>
        {
            { "Authorization", "Basic myTestToken" },
        };
        
        var context = Substitute.For<FunctionContext>();
        var httpRequest = Substitute.For<HttpRequestData>(context);
        httpRequest.Headers.Returns(new HttpHeadersCollection(headers));
        
        // Act
        var result = httpRequest.TryExtractToken(out _);
        
        // Assert
        result.Should().BeFalse();
    }
    
    [Fact]
    public void IndicateSuccessIfHeaderContainsValidToken()
    {
        // Arrange
        var headers = new Dictionary<string, string>
        {
            { "Authorization", "Bearer myTestToken" },
        };
        
        var context = Substitute.For<FunctionContext>();
        var httpRequest = Substitute.For<HttpRequestData>(context);
        httpRequest.Headers.Returns(new HttpHeadersCollection(headers));
        
        // Act
        var result = httpRequest.TryExtractToken(out _);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public void ExtractExpectedToken()
    {
        // Arrange
        var expected = "myTestToken";
        var headers = new Dictionary<string, string>
        {
            { "Authorization", $"Bearer {expected}" },
        };
        
        var context = Substitute.For<FunctionContext>();
        var httpRequest = Substitute.For<HttpRequestData>(context);
        httpRequest.Headers.Returns(new HttpHeadersCollection(headers));
        
        // Act
        httpRequest.TryExtractToken(out var result);
        
        // Assert
        result.Should().Be(expected);
    }
}