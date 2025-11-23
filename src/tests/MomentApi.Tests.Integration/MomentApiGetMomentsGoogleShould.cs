using System.Net;
using AwesomeAssertions;
using Functions.Functions;
using Infrastructure.Database;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using MomentApi.Tests.Integration.Fixtures;
using NSubstitute;
using Operations.Queries.ValidateToken;
using Persistence;

namespace MomentApi.Tests.Integration;

[Collection("RunInSerialOrderToAvoidTestContainerConflicts")]
public class MomentApiGetMomentsGoogleShould: HttpFunctionFixture<DefaultTestValidateToken>
{
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        await AddMoment();
    }

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
    
    [Fact]
    public async Task RetrieveExpectedData()
    {
        throw new NotImplementedException();
    }
    
    private async Task AddMoment()
    {
        var optionsBuilder = new DbContextOptionsBuilder<MomentContext>()
            .UseSqlServer(ConnectionString);

        var context = new MomentContext(optionsBuilder.Options);
        var createMoment = new CreateMoment(context);

        var validToken = new ValidToken(DefaultTestValidateToken.TestSubject);
        await createMoment.CreateAsync(validToken);
    }
}