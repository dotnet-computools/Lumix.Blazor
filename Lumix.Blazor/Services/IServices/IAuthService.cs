using Lumix.Blazor.Data;
using Lumix.Blazor.Data.Responses;
using Lumix.Blazor.Models;

namespace Lumix.Blazor.Services.IServices;

public interface IAuthService
{
    Task<ApiResult<RegisterDto>> RegisterAsync(RegisterDto registerDto);
    Task<ApiResult<LoginResponseDto>> LoginAsync(LoginDto loginDto);
    Task Logout();
    Task<bool> IsAuthenticated();
    Task<string?> GetAccessToken();
}