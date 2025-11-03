using System.Net;
using AwesomeAssertions;
using Functions.Functions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using NSubstitute;

namespace MomentApi.Tests.Integration;

public class MomentApiCreateMomentGoogleShould(HttpFunctionFixture fixture) : IClassFixture<HttpFunctionFixture>
{
    [Fact]
    public async Task CreateMoment()
    {
        // Arrange
        var headers = new Dictionary<string, string>
        {
            { "Authorization", "Bearer myTestToken" },
        };
        
        var context = Substitute.For<FunctionContext>();
        var httpRequest = Substitute.For<HttpRequestData>(context);
        httpRequest.Headers.Returns(new HttpHeadersCollection(headers));
        
        var responseData = Substitute.For<HttpResponseData>(context);
        httpRequest.CreateResponse().Returns(responseData);
        
        var function = fixture.GetService<HttpCreateMomentGoogleFunction>();
        
        // Act
        var response = await function.CreateMomentGoogle(httpRequest);
        var result = response.StatusCode;
        
        // Assert
        result.Should().Be(HttpStatusCode.Created);
    }
}