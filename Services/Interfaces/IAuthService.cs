using FirinApi.Models.DTOs.Auth;

namespace FirinApi.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto?> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default);
}
