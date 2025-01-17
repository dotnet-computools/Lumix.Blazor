using Lumix.Blazor.Data;
using Lumix.Blazor.Models;
using Lumix.Blazor.Services.IServices;

namespace Lumix.Blazor.Services;

public class AuthService : IAuthService
{
    private readonly HttpService _httpService;
    private readonly ILogger<AuthService> _logger;
    private readonly string _baseUrl = "api/auth";

    public AuthService(HttpService httpService, ILogger<AuthService> logger)
    {
        _httpService = httpService;
        _logger = logger;
    }

    public async Task<ApiResult<RegisterDto>> RegisterAsync(RegisterDto registerDto)
    {
        try
        {
            _logger.LogInformation($"Starting registration for {registerDto.Email}");
            
            var result = await _httpService.PostAsync<RegisterResponse>($"{_baseUrl}/register", registerDto);
            
            if (result.IsSuccess)
            {
                _logger.LogInformation($"Registration successful for {registerDto.Email}");
                return ApiResult<RegisterDto>.Success(registerDto);
            }
            
            _logger.LogWarning($"Registration failed for {registerDto.Email}: {result.ErrorMessage}");
            return ApiResult<RegisterDto>.Failure(result.ErrorMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Exception during registration for {registerDto.Email}");
            return ApiResult<RegisterDto>.Failure(ex.Message);
        }
    }
    public async Task<ApiResult<LoginResponseDto>> LoginAsync(LoginDto loginDto)
    {
        return await _httpService.PostAsync<LoginResponseDto>($"{_baseUrl}/login", loginDto);
    }

    public async Task<ApiResult<string>> RefreshTokenAsync(string refreshToken)
    {
        return await _httpService.PostAsync<string>($"{_baseUrl}/refresh-token", new { RefreshToken = refreshToken });
    }

    public async Task<ApiResult<UserDto>> GetCurrentUserAsync()
    {
        return await _httpService.GetAsync<UserDto>($"{_baseUrl}/get-current-user");
    }

    public async Task<ApiResult<string>> LogoutAsync()
    {
        return await _httpService.PostAsync<string>($"{_baseUrl}/logout", null);
    }
}

public class RegisterResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
}