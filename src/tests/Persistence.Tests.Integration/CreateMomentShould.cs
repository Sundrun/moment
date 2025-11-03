using AwesomeAssertions;
using Entities;
using Entities.Wrappers;
using Functions.CreateUser;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Operations.Commands.CreateMoment;
using Operations.Queries.ValidateToken;
using Testcontainers.MsSql;

namespace Persistence.Tests.Integration;

public class CreateMomentShould : IAsyncLifetime
{
    private CreateMoment _dtu = null!;
    private MsSqlContainer _msSqlContainer = null!;
    private MomentContext _testContext = null!;
    private readonly GoogleIdentitySubject _testSubject = new("TestSubject");
    
    public async Task InitializeAsync()
    {
        
        _msSqlContainer = new MsSqlBuilder()
            .WithPortBinding(1433)
            .Build();

        await _msSqlContainer.StartAsync();
            
        var connectionString = _msSqlContainer.GetConnectionString();
        
        var optionsBuilder = new DbContextOptionsBuilder<MomentContext>()
            .UseSqlServer(connectionString);
        
        _testContext = new MomentContext(optionsBuilder.Options);
        await _testContext.Database.EnsureCreatedAsync();
        
        var owner = new MomentOwner();
        await _testContext.MomentOwners.AddAsync(owner);
        
        var identity = new GoogleIdentity
        {
            Subject = _testSubject
        };
        var identityOwner = new GoogleIdentityOwner
        {
            Owner = owner,
            GoogleIdentity = identity
        };
        await _testContext.GoogleIdentities.AddAsync(identity);
        await _testContext.GoogleIdentityOwners.AddAsync(identityOwner);
        
        var createUserContext = new MomentContext(optionsBuilder.Options);
        _dtu = new CreateMoment(createUserContext);
    }

    public async Task DisposeAsync()
    {
        await _msSqlContainer.DisposeAsync();
    }
    
    [Fact]
    public async Task StoreMoment()
    {
        // Act
        await _dtu.CreateAsync(new ValidToken(_testSubject));
        var result = await _testContext.CoreMoments.FirstOrDefaultAsync();

        // Assert
        result.Should().NotBeNull();
    }
    
    [Fact]
    public async Task IndicateThatAMomentWasCreated()
    {
        // Act
        var result = await _dtu.CreateAsync(new ValidToken(_testSubject));

        // Assert
        result.Should().BeOfType<MomentCreated>();
    }
    
    [Fact]
    public async Task IndicateIfTheUserDoesNotExist()
    {
        // Act
        var nonUserSubject = new GoogleIdentitySubject("NonUserSubject");
        var result = await _dtu.CreateAsync(new ValidToken(nonUserSubject));

        // Assert
        result.Should().BeOfType<NoUser>();
    }
}