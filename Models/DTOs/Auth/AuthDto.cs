namespace FirinApi.Models.DTOs.Auth;

public record LoginDto(string Username, string Password);

public record AuthResponseDto(string Token, string Username, string Role, DateTime ExpiresAt);
