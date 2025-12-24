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
    private readonly ITokenProvider _tokenProvider;
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly string _url;

    public AuthService(
        HttpService httpService,
        ILogger<AuthService> logger,
        ITokenProvider tokenProvider,
        IOptions<ApiSettings> settings,
        AuthenticationStateProvider authStateProvider)
    {
        _httpService = httpService;
        _logger = logger;
        _tokenProvider = tokenProvider;
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
                await _tokenProvider.SetTokensAsync(
                    result.Value.AccessToken,
                    result.Value.RefreshToken);

                if (_authStateProvider is CustomAuthenticationStateProvider customAuthenticationStateProvider)
                    customAuthenticationStateProvider.NotifyUserAuthenticationStateChanged();
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
        await _tokenProvider.ClearAsync();
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

