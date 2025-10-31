// using System.Security.Claims;
// using AwesomeAssertions;
// using Functions.CreateUser;
// using Functions.Functions;
// using Microsoft.Azure.Functions.Worker;
// using Microsoft.Azure.Functions.Worker.Http;
// using NSubstitute;
//
// namespace Functions.Tests.Integration;
//
// // TODO 
// public class HttpCreateUserFunctionShould
// {
//     [Fact]
//     public async Task IndicateSuccessWhenIdentityIsValid()
//     {
//         // Arrange
//         var createUser = Substitute.For<ICreateUser>();
//         createUser.CreateAsync(Arg.Any<ClaimsPrincipal>()).Returns(new UserCreated());
//         var func = new HttpCreateUserFunction(createUser);
//         
//         var oidClaim = new Claim("oid", "02223b6b-aa1d-42d4-9ec0-1b2bb9194438");
//         var userNameClaim = new Claim(ClaimTypes.Name, "tester");
//         var claimsIdentity = new ClaimsIdentity([oidClaim, userNameClaim], "TestAuthentication");
//         
//         var context = Substitute.For<FunctionContext>();
//         var httpRequest = Substitute.For<HttpRequestData>(context);
//         httpRequest.Identities.Returns([claimsIdentity]);
//         
//         var httpResponse = Substitute.For<HttpResponseData>(context);
//         httpRequest.CreateResponse().Returns(httpResponse);
//
//         // Act
//         var response = await func.CreateUserAsync(httpRequest);
//         var result = response.StatusCode;
//
//         // Assert
//         result.Should().Be(System.Net.HttpStatusCode.Created);
//     }
//     
//     [Fact]
//     public async Task IndicateUnauthorizedWhenNoIdentityIsProvided()
//     {
//         // Arrange
//         var createUser = Substitute.For<ICreateUser>();
//         createUser.CreateAsync(Arg.Any<ClaimsPrincipal>()).Returns(new UserCreated());
//         var func = new HttpCreateUserFunction(createUser);
//         
//         var context = Substitute.For<FunctionContext>();
//         var httpRequest = Substitute.For<HttpRequestData>(context);
//         
//         var httpResponse = Substitute.For<HttpResponseData>(context);
//         httpRequest.CreateResponse().Returns(httpResponse);
//         
//         // Act
//         var response = await func.CreateUserAsync(httpRequest);
//         var result = response.StatusCode;
//
//         // Assert
//         result.Should().Be(System.Net.HttpStatusCode.Unauthorized);
//     }
//     
//     [Fact]
//     public async Task IndicateConflictIfUserAlreadyExists()
//     {
//         // Arrange
//         var createUser = Substitute.For<ICreateUser>();
//         createUser.CreateAsync(Arg.Any<ClaimsPrincipal>()).Returns(new UserExists());
//         var func = new HttpCreateUserFunction(createUser);
//         
//         var oidClaim = new Claim("oid", "02223b6b-aa1d-42d4-9ec0-1b2bb9194438");
//         var userNameClaim = new Claim(ClaimTypes.Name, "tester");
//         var claimsIdentity = new ClaimsIdentity([oidClaim, userNameClaim], "TestAuthentication");
//         
//         var context = Substitute.For<FunctionContext>();
//         var httpRequest = Substitute.For<HttpRequestData>(context);
//         httpRequest.Identities.Returns([claimsIdentity]);
//         
//         var httpResponse = Substitute.For<HttpResponseData>(context);
//         httpRequest.CreateResponse().Returns(httpResponse);
//
//         // Act
//         var response = await func.CreateUserAsync(httpRequest);
//         var result = response.StatusCode;
//
//         // Assert
//         result.Should().Be(System.Net.HttpStatusCode.Conflict);
//     }
// }