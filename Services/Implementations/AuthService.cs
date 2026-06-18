using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FirinApi.Configuration;
using FirinApi.Models.DTOs.Auth;
using FirinApi.Repositories.Interfaces;
using FirinApi.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FirinApi.Services.Implementations;

public class AuthService(IUserRepository userRepository, IOptions<JwtSettings> jwtOptions) : IAuthService
{
    private readonly JwtSettings _jwt = jwtOptions.Value;

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByUsernameAsync(dto.Username, cancellationToken);
        if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return null;

        var expiresAt = DateTime.UtcNow.AddMinutes(_jwt.ExpiresInMinutes);
        var token = GenerateToken(user.Username, user.Role.ToString(), expiresAt);

        return new AuthResponseDto(token, user.Username, user.Role.ToString(), expiresAt);
    }

    private string GenerateToken(string username, string role, DateTime expiresAt)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role)
        };

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
