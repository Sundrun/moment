using System.Net;
using AwesomeAssertions;
using MomentApi.CreateUser;
using MomentApi.Helpers;

namespace MomentApi.Tests.Unit;

public class CreateUserResponseToStatusCodeShould
{
    private record UnknownResponse : ICreateUserResponse;
    
    [Fact]
    public void IndicateCreatedWhenResponseReportsAUserWasCreated()
    {
        // Arrange
        var response = new UserCreated();

        // Act
        var result = CreateUserResponseToStatusCode.ToStatusCode(response);

        // Assert
        result.Should().Be(HttpStatusCode.Created);
    }
    
    [Fact]
    public void IndicateConflictWhenResponseReportsTheUserAlreadyExists()
    {
        // Arrange
        var response = new UserExists();

        // Act
        var result = CreateUserResponseToStatusCode.ToStatusCode(response);

        // Assert
        result.Should().Be(HttpStatusCode.Conflict);
    }
    
    [Fact]
    public void IndicateErrorIfUnableToHandleResponse()
    {
        // Arrange
        var response = new UnknownResponse();

        // Act
        var result = CreateUserResponseToStatusCode.ToStatusCode(response);

        // Assert
        result.Should().Be(HttpStatusCode.InternalServerError);
    }
}