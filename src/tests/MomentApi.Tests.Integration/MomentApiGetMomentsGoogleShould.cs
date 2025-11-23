using System.Net;
using System.Text.Json;
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
    private MomentContext _context = null!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        
        var optionsBuilder = new DbContextOptionsBuilder<MomentContext>()
            .UseSqlServer(ConnectionString);

        _context = new MomentContext(optionsBuilder.Options);

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
        responseData.Body.Returns(new MemoryStream());
        
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
        // Arrange
        var expected = await _context.MomentOwnerships
            .Include(mo => mo.Moment)
            .Include(mo => mo.Owner)
            .Select(mo => mo.Moment)
            .ToListAsync();
        
        var headers = new Dictionary<string, string>
        {
            { "Authorization", $"Bearer {DefaultTestValidateToken.TestSubject.Subject}" },
        };
        
        var context = Substitute.For<FunctionContext>();
        var httpRequest = Substitute.For<HttpRequestData>(context);
        httpRequest.Headers.Returns(new HttpHeadersCollection(headers));
        
        var responseData = Substitute.For<HttpResponseData>(context);
        var bodyStream = new MemoryStream();
        responseData.Body.Returns(bodyStream);
        
        httpRequest.CreateResponse().Returns(responseData);
        
        var function = GetService<HttpGetMomentsGoogleFunction>();
        
        // Act
        await function.GetMomentsGoogle(httpRequest);
        
        bodyStream.Position = 0;
        var result = await JsonSerializer.DeserializeAsync<IEnumerable<Entities.CoreMoment>>(bodyStream);
        
        // Assert
        result.Should().BeEquivalentTo(expected);
    }
    
    private async Task AddMoment()
    {
        var createMoment = new CreateMoment(_context);

        var validToken = new ValidToken(DefaultTestValidateToken.TestSubject);
        await createMoment.CreateAsync(validToken);
    }
}