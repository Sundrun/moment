using AwesomeAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace MomentApi.Tests.Integration;

public class HttpCreateUserFunctionShould
{
    [Fact]
    public void IndicateSuccess()
    {
        // Arrange
        var func = new HttpCreateUserFunction();
        var request = Substitute.For<HttpRequest>();

        // Act
        var result = func.CreateUser(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }
}