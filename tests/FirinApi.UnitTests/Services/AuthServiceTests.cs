using FirinApi.Configuration;
using FirinApi.Models.DTOs.Auth;
using FirinApi.Models.Entities;
using FirinApi.Models.Enums;
using FirinApi.Repositories.Interfaces;
using FirinApi.Services.Implementations;
using Microsoft.Extensions.Options;
using Moq;

namespace FirinApi.UnitTests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        var jwtSettings = Options.Create(new JwtSettings
        {
            Key = "TestSecretKeyForUnitTests_Minimum32Characters!",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            ExpiresInMinutes = 60
        });

        _service = new AuthService(_userRepo.Object, jwtSettings);
    }

    [Fact]
    public async Task LoginAsync_DogruBilgiler_TokenDondurur()
    {
        var user = new User
        {
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            Role = UserRole.Admin
        };

        _userRepo.Setup(r => r.GetByUsernameAsync("admin", It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var result = await _service.LoginAsync(new LoginDto("admin", "Admin123!"));

        Assert.NotNull(result);
        Assert.Equal("admin", result.Username);
        Assert.Equal("Admin", result.Role);
        Assert.False(string.IsNullOrWhiteSpace(result.Token));
    }

    [Fact]
    public async Task LoginAsync_YanlisSifre_NullDondurur()
    {
        var user = new User
        {
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!")
        };

        _userRepo.Setup(r => r.GetByUsernameAsync("admin", It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var result = await _service.LoginAsync(new LoginDto("admin", "YanlisSifre"));

        Assert.Null(result);
    }
}
