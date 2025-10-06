using IdentityCredentialService.BusinessLogic.Dtos;
using IdentityCredentialService.BusinessLogic.Services.Implementations;
using IdentityCredentialService.BusinessLogic.Services.Interfaces;
using IdentityCredentialService.Domain.Models;
using IdentityCredentialService.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;

namespace IdentityCredentialService.Tests;

public class UserServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly Mock<IJwtService> _mockJwtService;
    private readonly UserService _userService;

    public UserServiceTests()
    {

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _mockJwtService = new Mock<IJwtService>();
        _userService = new UserService(_context, _mockJwtService.Object);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    #region CreateUserAsync Tests

    [Fact]
    public async Task CreateUserAsync_ValidRequest_ShouldReturnSuccess()
    {
        // Arrange
        var request = new RegisterRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Password = "StrongPassword123!"
        };

        // Act
        var result = await _userService.CreateUserAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal("User created successfully.", result.Message);

        var userInDb = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        Assert.NotNull(userInDb);
        Assert.Equal(request.FirstName, userInDb.FirstName);
        Assert.Equal(request.LastName, userInDb.LastName);
        Assert.Equal(request.Email, userInDb.Email);
        Assert.NotEqual(request.Password, userInDb.Password); // Password should be hashed
    }

    [Fact]
    public async Task CreateUserAsync_MissingFirstName_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new RegisterRequest
        {
            FirstName = "",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Password = "StrongPassword123!"
        };

        // Act
        var result = await _userService.CreateUserAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("First name is required.", result.Message);
    }

    [Fact]
    public async Task CreateUserAsync_MissingLastName_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new RegisterRequest
        {
            FirstName = "John",
            LastName = "",
            Email = "john.doe@example.com",
            Password = "StrongPassword123!"
        };

        // Act
        var result = await _userService.CreateUserAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Last name is required.", result.Message);
    }

    [Fact]
    public async Task CreateUserAsync_MissingEmail_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new RegisterRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "",
            Password = "StrongPassword123!"
        };

        // Act
        var result = await _userService.CreateUserAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Email is required.", result.Message);
    }

    [Fact]
    public async Task CreateUserAsync_MissingPassword_ShouldReturnBadRequest()
    {
        var request = new RegisterRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Password = ""
        };

        var result = await _userService.CreateUserAsync(request);

        Assert.False(result.Success);
        Assert.Equal("Password is required.", result.Message);
    }

    [Fact]
    public async Task CreateUserAsync_ExistingEmail_ShouldReturnBadRequest()
    {
        // Arrange
        var existingUser = new User
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "john.doe@example.com",
            Password = "hashedpassword"
        };
        _context.Users.Add(existingUser);
        await _context.SaveChangesAsync();

        var request = new RegisterRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Password = "StrongPassword123!"
        };

        // Act
        var result = await _userService.CreateUserAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("User with this email already exists.", result.Message);
    }

    [Fact]
    public async Task CreateUserAsync_ExistingEmailDifferentCase_ShouldReturnBadRequest()
    {
        // Arrange
        var existingUser = new User
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "john.doe@example.com",
            Password = "hashedpassword"
        };
        _context.Users.Add(existingUser);
        await _context.SaveChangesAsync();

        var request = new RegisterRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "JOHN.DOE@EXAMPLE.COM", // Different case
            Password = "StrongPassword123!"
        };

        // Act
        var result = await _userService.CreateUserAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("User with this email already exists.", result.Message);
    }

    #endregion

    #region IssueCredentialAsync Tests

    [Fact]
    public async Task IssueCredentialAsync_ValidCredentials_ShouldReturnSuccess()
    {
        // Arrange
        var hashedPassword = PasswordHelper.HashPassword("StrongPassword123!");
        var user = new User
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Password = hashedPassword
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var request = new IssueCredentialRequest
        {
            Email = "john.doe@example.com",
            Password = "StrongPassword123!"
        };

        _mockJwtService.Setup(x => x.GenerateAccessToken(It.IsAny<IEnumerable<Claim>>()))
            .Returns("access-token");
        _mockJwtService.Setup(x => x.GenerateRefreshToken())
            .Returns("refresh-token");

        // Act
        var result = await _userService.IssueCredentialAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal("Credential issued successfully.", result.Message);
        Assert.Equal("access-token", result.Data.AccessToken);
        Assert.Equal("refresh-token", result.Data.RefreshToken);
        Assert.Equal(user.FirstName, result.Data.User.FirstName);
        Assert.Equal(user.LastName, result.Data.User.LastName);
        Assert.Equal(user.Email, result.Data.User.Email);

        _mockJwtService.Verify(x => x.GenerateAccessToken(It.IsAny<IEnumerable<Claim>>()), Times.Once);
        _mockJwtService.Verify(x => x.GenerateRefreshToken(), Times.Once);
    }

    [Fact]
    public async Task IssueCredentialAsync_MissingEmail_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new IssueCredentialRequest
        {
            Email = "",
            Password = "StrongPassword123!"
        };

        // Act
        var result = await _userService.IssueCredentialAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Email is required.", result.Message);
    }

    [Fact]
    public async Task IssueCredentialAsync_MissingPassword_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new IssueCredentialRequest
        {
            Email = "john.doe@example.com",
            Password = ""
        };

        // Act
        var result = await _userService.IssueCredentialAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Password is required.", result.Message);
    }

    [Fact]
    public async Task IssueCredentialAsync_NonExistentUser_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new IssueCredentialRequest
        {
            Email = "nonexistent@example.com",
            Password = "StrongPassword123!"
        };

        // Act
        var result = await _userService.IssueCredentialAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Invalid email or password.", result.Message);
    }

    [Fact]
    public async Task IssueCredentialAsync_WrongPassword_ShouldReturnBadRequest()
    {
        // Arrange
        var hashedPassword = PasswordHelper.HashPassword("CorrectPassword123!");
        var user = new User
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Password = hashedPassword
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var request = new IssueCredentialRequest
        {
            Email = "john.doe@example.com",
            Password = "WrongPassword123!"
        };

        // Act
        var result = await _userService.IssueCredentialAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Invalid email or password.", result.Message);
    }

    [Fact]
    public async Task IssueCredentialAsync_EmailDifferentCase_ShouldReturnSuccess()
    {
        // Arrange
        var hashedPassword = PasswordHelper.HashPassword("StrongPassword123!");
        var user = new User
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Password = hashedPassword
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var request = new IssueCredentialRequest
        {
            Email = "JOHN.DOE@EXAMPLE.COM", // Different case
            Password = "StrongPassword123!"
        };

        _mockJwtService.Setup(x => x.GenerateAccessToken(It.IsAny<IEnumerable<Claim>>()))
            .Returns("access-token");
        _mockJwtService.Setup(x => x.GenerateRefreshToken())
            .Returns("refresh-token");

        // Act
        var result = await _userService.IssueCredentialAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal("Credential issued successfully.", result.Message);
    }

    [Fact]
    public async Task IssueCredentialAsync_ShouldGenerateCorrectClaims()
    {
        // Arrange
        var hashedPassword = PasswordHelper.HashPassword("StrongPassword123!");
        var user = new User
        {
            Id = "test-user-id",
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Password = hashedPassword
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var request = new IssueCredentialRequest
        {
            Email = "john.doe@example.com",
            Password = "StrongPassword123!"
        };

        List<Claim> capturedClaims = null;
        _mockJwtService.Setup(x => x.GenerateAccessToken(It.IsAny<IEnumerable<Claim>>()))
            .Callback<IEnumerable<Claim>>(claims => capturedClaims = claims.ToList())
            .Returns("access-token");
        _mockJwtService.Setup(x => x.GenerateRefreshToken())
            .Returns("refresh-token");

        // Act
        await _userService.IssueCredentialAsync(request);

        // Assert
        Assert.NotNull(capturedClaims);
        Assert.Equal(2, capturedClaims.Count);
        
        var userIdClaim = capturedClaims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        Assert.NotNull(userIdClaim);
        Assert.Equal(user.Id, userIdClaim.Value);
        
        var emailClaim = capturedClaims.FirstOrDefault(c => c.Type == "email");
        Assert.NotNull(emailClaim);
        Assert.Equal(user.Email, emailClaim.Value);
    }

    #endregion
}