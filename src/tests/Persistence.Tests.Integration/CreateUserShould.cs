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
        await _dtu.CreateAsync(new ValidToken(_testSubject));
        var result = await _testContext.MomentOwners.FirstOrDefaultAsync();

        // Assert
        result.Should().NotBeNull();
    }
    
    [Fact]
    public async Task IndicateThatAOwnerWasCreated()
    {
        // Act
        var result = await _dtu.CreateAsync(new ValidToken(_testSubject));

        // Assert
        result.Should().BeOfType<UserCreated>();
    }
    
    [Fact]
    public async Task StoreGoogleSubject()
    {
        // Arrange
        var expected = _testSubject;
        var token = new ValidToken(_testSubject);
        
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
        await _dtu.CreateAsync(new ValidToken(_testSubject));
        
        // Act
        await _dtu.CreateAsync(new ValidToken(_testSubject));
        var owners = await _testContext.MomentOwners.ToListAsync();
        var result = owners.Count;

        // Assert
        result.Should().Be(1);
    }
    
    [Fact]
    public async Task NotAddNewIdentityIfSubjectAlreadyExists()
    {
        // Arrange
        await _dtu.CreateAsync(new ValidToken(_testSubject));
        
        // Act
        await _dtu.CreateAsync(new ValidToken(_testSubject));
        var owners = await _testContext.GoogleIdentityOwners.ToListAsync();
        var result = owners.Count;

        // Assert
        result.Should().Be(1);
    }
    
    [Fact]
    public async Task IndicateIfSubjectAlreadyExists()
    {
        // Arrange
        await _dtu.CreateAsync(new ValidToken(_testSubject));
        
        // Act
        var result = await _dtu.CreateAsync(new ValidToken(_testSubject));

        // Assert
        result.Should().BeOfType<UserExists>();
    }
}