using AwesomeAssertions;
using Entities;
using Entities.Wrappers;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Operations.Queries.GetMoments;
using Operations.Queries.ValidateToken;
using Testcontainers.MsSql;

namespace Persistence.Tests.Integration;

[Collection("RunInSerialOrderToAvoidTestContainerConflicts")]
public class GetMomentsShould : IAsyncLifetime
{
    private GetMoments _dtu = null!;
    private MsSqlContainer _msSqlContainer = null!;
    private MomentContext _testContext = null!;
    private MomentOwner _momentOwner = null!;
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
        await _testContext.GoogleIdentities.AddAsync(identity);
        
        var identityOwner = new GoogleIdentityOwner
        {
            Owner = owner,
            GoogleIdentity = identity
        };
        await _testContext.GoogleIdentityOwners.AddAsync(identityOwner);
        await _testContext.SaveChangesAsync();

        _momentOwner = await _testContext.MomentOwners.FirstAsync();
        
        var createUserContext = new MomentContext(optionsBuilder.Options);
        _dtu = new GetMoments(createUserContext);
    }

    public async Task DisposeAsync()
    {
        await _msSqlContainer.DisposeAsync();
    }
    
    [Fact]
    public async Task IndicateIfUserHasNoMoments()
    {
        // Act
        var result = await _dtu.GetMomentsAsync(new ValidToken(_testSubject));

        // Assert
        result.Should().BeOfType<NoMoments>();
    }
    
    [Fact]
    public async Task RetrieveExpectedMoment()
    {
        // Arrange
        var newMoment = new CoreMoment();
        
        var ownership = new MomentOwnership
        {
            Moment = newMoment,
            Owner = _momentOwner
        };
        
        await _testContext.CoreMoments.AddAsync(newMoment);
        await _testContext.MomentOwnerships.AddAsync(ownership);
        await _testContext.SaveChangesAsync();
        
        var expectedCoreMoment = new CoreMoment
        {
            Id = new CoreMomentId(1)
        };
        var expected = new UserMoments([expectedCoreMoment]);
        
        // Act
        var result = (UserMoments)await _dtu.GetMomentsAsync(new ValidToken(_testSubject));

        // Assert
        result.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public async Task BeAbleToRetrieveMultipleMoments()
    {
        // Arrange
        var newMoment1 = new CoreMoment();
        var ownership1 = new MomentOwnership
        {
            Moment = newMoment1,
            Owner = _momentOwner
        };
        await _testContext.CoreMoments.AddAsync(newMoment1);
        await _testContext.MomentOwnerships.AddAsync(ownership1);
        
        var newMoment2 = new CoreMoment();
        var ownership2 = new MomentOwnership
        {
            Moment = newMoment2,
            Owner = _momentOwner
        };
        await _testContext.CoreMoments.AddAsync(newMoment2);
        await _testContext.MomentOwnerships.AddAsync(ownership2);
        
        await _testContext.SaveChangesAsync();

        // Act
        var response = (UserMoments)await _dtu.GetMomentsAsync(new ValidToken(_testSubject));
        var result = response.Moments.Count();

        // Assert
        result.Should().Be(2);
    }
    
    [Fact]
    public async Task NotRetrieveMomentsForOtherUsers()
    {
        // Arrange
        var otherOwner = new MomentOwner();
        
        var identity = new GoogleIdentity
        {
            Subject = new GoogleIdentitySubject("OtherSubject")
        };
        
        var identityOwner = new GoogleIdentityOwner
        {
            Owner = otherOwner,
            GoogleIdentity = identity
        };
        
        await _testContext.MomentOwners.AddAsync(otherOwner);
        await _testContext.GoogleIdentities.AddAsync(identity);
        await _testContext.GoogleIdentityOwners.AddAsync(identityOwner);
        
        var newMoment = new CoreMoment();
        
        var ownership = new MomentOwnership
        {
            Moment = newMoment,
            Owner = otherOwner
        };


        await _testContext.CoreMoments.AddAsync(newMoment);
        await _testContext.MomentOwnerships.AddAsync(ownership);
        await _testContext.SaveChangesAsync();
        
        // Act
        var result = await _dtu.GetMomentsAsync(new ValidToken(_testSubject));

        // Assert
        result.Should().BeOfType<NoMoments>();
    }
    
    [Fact]
    public async Task IndicateIfTheUserDoesNotExist()
    {
        // Act
        var nonUserSubject = new GoogleIdentitySubject("NonUserSubject");
        var result = await _dtu.GetMomentsAsync(new ValidToken(nonUserSubject));

        // Assert
        result.Should().BeOfType<NoUser>();
    }
}