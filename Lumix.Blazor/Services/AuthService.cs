using Lumix.Blazor.Configuration;
using Lumix.Blazor.Data.Auth;
using Lumix.Blazor.Data.Responses;
using Lumix.Blazor.Models;
using Lumix.Blazor.Services;
using Lumix.Blazor.Services.IServices;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;


public class AuthService : IAuthService
{
    private readonly HttpService _httpService;
    private readonly ILogger<AuthService> _logger;
    private readonly IJSRuntime _jsRuntime;
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly string _url;

    private const string accessToken = "accessToken";
    private const string refreshToken = "refreshToken";

    public AuthService(
        HttpService httpService,
        ILogger<AuthService> logger,
        IJSRuntime jsRuntime,
        IOptions<ApiSettings> settings,
        AuthenticationStateProvider authStateProvider)
    {
        _httpService = httpService;
        _logger = logger;
        _jsRuntime = jsRuntime;
        _authStateProvider = authStateProvider;

        var baseUrl = settings.Value.BaseUrl.TrimEnd('/');
        _url = $"{baseUrl}/api/auth";
    }

    public async Task<ApiResult<LoginResponseDto>> LoginAsync(LoginDto loginDto)
    {
        try
        {
            var result = await _httpService.PostAsync<LoginResponseDto>(
                $"{_url}/login", loginDto);

            if (result.IsSuccess && result.Value != null)
            {
                await _jsRuntime.InvokeVoidAsync(
                    "setCookie", accessToken, result.Value.AccessToken, 1);

                await _jsRuntime.InvokeVoidAsync(
                    "setCookie", refreshToken, result.Value.RefreshToken, 1);

                (_authStateProvider as CustomAuthenticationStateProvider)
                    ?.NotifyUserAuthenticationStateChanged();
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login failed");
            return ApiResult<LoginResponseDto>.Failure(ex.Message);
        }
    }

    public async Task Logout()
    {
        await _jsRuntime.InvokeVoidAsync("eraseCookie", accessToken);
        await _jsRuntime.InvokeVoidAsync("eraseCookie", refreshToken);

        (_authStateProvider as CustomAuthenticationStateProvider)
            ?.NotifyUserAuthenticationStateChanged();
    }

    public async Task<ApiResult<RegisterDto>> RegisterAsync(RegisterDto registerDto)
    {
        return await _httpService.PostAsync<RegisterDto>(
            $"{_url}/register", registerDto);
    }

    public async Task<ApiResult<Guid?>> GetCurrentUserAsync()
    {
        var result = await _httpService.GetAsync<UserResponseDto>(
            $"{_url}/get-current-user");

        return result.IsSuccess && result.Value != null
            ? ApiResult<Guid?>.Success(result.Value.UserId)
            : ApiResult<Guid?>.Failure(result.ErrorMessage);
    }
}

