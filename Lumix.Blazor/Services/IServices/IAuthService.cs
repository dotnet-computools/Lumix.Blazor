using Lumix.Blazor.Data;
using Lumix.Blazor.Models;

namespace Lumix.Blazor.Services.IServices;

public interface IAuthService
{
    Task<ApiResult<RegisterDto>> RegisterAsync(RegisterDto registerDto);
    Task<ApiResult<LoginResponseDto>> LoginAsync(LoginDto loginDto);
    Task<ApiResult<string>> RefreshTokenAsync(string refreshToken);
    Task<ApiResult<UserDto>> GetCurrentUserAsync();
    Task<ApiResult<string>> LogoutAsync();
}