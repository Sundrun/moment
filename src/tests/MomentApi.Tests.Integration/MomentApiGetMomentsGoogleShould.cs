using System.Net;
using AwesomeAssertions;
using Functions.Functions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using MomentApi.Tests.Integration.Fixtures;
using NSubstitute;

namespace MomentApi.Tests.Integration;

[Collection("RunInSerialOrderToAvoidTestContainerConflicts")]
public class MomentApiGetMomentsGoogleShould: HttpFunctionFixture<DefaultTestValidateToken>
{
    [Fact]
    public async Task IndicateSuccess()
    {
        // Arrange
        var headers = new Dictionary<string, string>
        {
            { "Authorization", $"Bearer {DefaultTestValidateToken.TestSubject.Subject}" },
        };
        
        var context = Substitute.For<FunctionContext>();
        var httpRequest = Substitute.For<HttpRequestData>(context);
        httpRequest.Headers.Returns(new HttpHeadersCollection(headers));
        
        var responseData = Substitute.For<HttpResponseData>(context);
        httpRequest.CreateResponse().Returns(responseData);
        
        var function = GetService<HttpGetMomentsGoogleFunction>();
        
        // Act
        var response = await function.GetMomentsGoogle(httpRequest);
        var result = response.StatusCode;
        
        // Assert
        result.Should().Be(HttpStatusCode.OK);
    }
}