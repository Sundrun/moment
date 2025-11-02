using AwesomeAssertions;
using Entities.Wrappers;
using Functions.CreateUser;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Operations.Queries.ValidateToken;
using Testcontainers.MsSql;

namespace Persistence.Tests.Integration;

public class CreateUserShould : IAsyncLifetime
{
    private CreateUser _dtu = null!;
    private MsSqlContainer _msSqlContainer = null!;
    private MomentContext _testContext = null!;
    
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
        
        var createUserContext = new MomentContext(optionsBuilder.Options);
        _dtu = new CreateUser(createUserContext);
    }

    public async Task DisposeAsync()
    {
        await _msSqlContainer.DisposeAsync();
    }
    
    [Fact]
    public async Task StoreOwner()
    {
        // Act
        var subject = new GoogleIdentitySubject(string.Empty);
        await _dtu.CreateAsync(new ValidToken(subject));
        var result = await _testContext.MomentOwners.FirstOrDefaultAsync();

        // Assert
        result.Should().NotBeNull();
    }
    
    [Fact]
    public async Task IndicateThatAOwnerWasCreated()
    {
        // Act
        var subject = new GoogleIdentitySubject(string.Empty);
        var result = await _dtu.CreateAsync(new ValidToken(subject));

        // Assert
        result.Should().BeOfType<UserCreated>();
    }
    
    [Fact]
    public async Task StoreGoogleSubject()
    {
        // Arrange
        var expected = new GoogleIdentitySubject(string.Empty);
        var token = new ValidToken(expected);
        
        // Act
        await _dtu.CreateAsync(token);
        var identity = await _testContext.GoogleIdentities.FirstOrDefaultAsync();
        var result = identity!.Subject;

        // Assert
        result.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public async Task NotAddNewOwnerIfSubjectAlreadyExists()
    {
        // Arrange
        var subject = new GoogleIdentitySubject(string.Empty);
        await _dtu.CreateAsync(new ValidToken(subject));
        
        // Act
        await _dtu.CreateAsync(new ValidToken(subject));
        var owners = await _testContext.MomentOwners.ToListAsync();
        var result = owners.Count;

        // Assert
        result.Should().Be(1);
    }
    
    [Fact]
    public async Task NotAddNewIdentityIfSubjectAlreadyExists()
    {
        // Arrange
        var subject = new GoogleIdentitySubject(string.Empty);
        await _dtu.CreateAsync(new ValidToken(subject));
        
        // Act
        await _dtu.CreateAsync(new ValidToken(subject));
        var owners = await _testContext.GoogleIdentityOwners.ToListAsync();
        var result = owners.Count;

        // Assert
        result.Should().Be(1);
    }
    
    [Fact]
    public async Task IndicateIfSubjectAlreadyExists()
    {
        // Arrange
        var subject = new GoogleIdentitySubject(string.Empty);
        await _dtu.CreateAsync(new ValidToken(subject));
        
        // Act
        var result = await _dtu.CreateAsync(new ValidToken(subject));

        // Assert
        result.Should().BeOfType<UserExists>();
    }
}