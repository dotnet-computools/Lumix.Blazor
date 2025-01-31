using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Lumix.Blazor.Data;
using Lumix.Blazor.Data.Responses;
using Lumix.Blazor.Models;
using Lumix.Blazor.Services.IServices;
using Microsoft.JSInterop;

public class AuthService : IAuthService
{
    private readonly HttpService _httpService;
    private readonly ILogger<AuthService> _logger;
    private readonly IJSRuntime _jsRuntime;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly string _baseUrl = "https://localhost:7231/api/auth";

    private const string accessToken = "accessToken";
    private const string refreshToken = "refreshToken";

    public AuthService(
        HttpService httpService,
        ILogger<AuthService> logger,
        IJSRuntime jsRuntime,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpService = httpService;
        _logger = logger;
        _jsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ApiResult<LoginResponseDto>> LoginAsync(LoginDto loginDto)
    {
        try
        {
            _logger.LogInformation($"Attempting login for user: {loginDto.email}");

            var result = await _httpService.PostAsync<LoginResponseDto>($"{_baseUrl}/login", loginDto);

            if (result.IsSuccess && result.Value != null)
            {
                try
                {
                    await _jsRuntime.InvokeVoidAsync("setCookie", accessToken, result.Value.accessToken, 1);
                    await _jsRuntime.InvokeVoidAsync("setCookie", refreshToken, result.Value.refreshToken, 1);

                    _logger.LogInformation("Login successful, tokens stored in cookies");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to store tokens in cookies");
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login failed with exception");
            return ApiResult<LoginResponseDto>.Failure(ex.Message);
        }
    }

    

    public async Task<bool> IsAuthenticated()
    {
        try
        {
            var token = await GetAccessToken();
            return !string.IsNullOrEmpty(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking authentication status");
            return false;
        }
    }

    public async Task<string?> GetAccessToken()
    {
        try
        {
            return await Task.FromResult(_httpContextAccessor.HttpContext?.Request.Cookies[accessToken]);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting access token from cookies");
            return null;
        }
    }

    public async Task<ApiResult<Guid?>> GetCurrentUserAsync()
    {
        try
        {
            _logger.LogInformation("Getting current user");

            var result = await _httpService.GetAsync<UserResponseDto>($"{_baseUrl}/get-current-user");

            if (result.IsSuccess && result.Value != null)
            {
                _logger.LogInformation("Current user retrieved");
                return ApiResult<Guid?>.Success(result.Value.userId);
            }

            _logger.LogWarning($"Failed to get current user: {result.ErrorMessage}");
            return ApiResult<Guid?>.Failure(result.ErrorMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current user");
            return ApiResult<Guid?>.Failure(ex.Message);
        }
    }

    public async Task Logout()
    {
        try
        {
            var options = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(-1)
            };

            _httpContextAccessor.HttpContext?.Response.Cookies.Append(accessToken, "", options);
            _httpContextAccessor.HttpContext?.Response.Cookies.Append(refreshToken, "", options);

            _logger.LogInformation("User logged out, tokens removed from cookies");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
        }
    }

    public async Task<ApiResult<RegisterDto>> RegisterAsync(RegisterDto registerDto)
    {
        try
        {
            _logger.LogInformation($"Starting registration for {registerDto.email}");

            var result = await _httpService.PostAsync<RegisterDto>($"{_baseUrl}/register", registerDto);

            if (result.IsSuccess)
            {
                _logger.LogInformation($"Registration successful for {registerDto.email}");
                return ApiResult<RegisterDto>.Success(registerDto);
            }

            _logger.LogWarning($"Registration failed for {registerDto.email}: {result.ErrorMessage}");
            return ApiResult<RegisterDto>.Failure(result.ErrorMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Exception during registration for {registerDto.email}");
            return ApiResult<RegisterDto>.Failure(ex.Message);
        }
    }
}
