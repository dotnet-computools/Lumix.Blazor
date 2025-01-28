using Blazored.LocalStorage;
using Lumix.Blazor.Data;
using Lumix.Blazor.Data.Responses;
using Lumix.Blazor.Models;
using Lumix.Blazor.Services.IServices;

public class AuthService : IAuthService
{
    private readonly HttpService _httpService;
    private readonly ILogger<AuthService> _logger;
    private readonly ILocalStorageService _localStorage;
    private readonly string _baseUrl = "https://localhost:7231/api/auth";
    
    private const string AccessTokenKey = "access_token";
    private const string RefreshTokenKey = "refresh_token";

    public AuthService(
        HttpService httpService, 
        ILogger<AuthService> logger,
        ILocalStorageService localStorage)
    {
        _httpService = httpService;
        _logger = logger;
        _localStorage = localStorage;
    }

    public async Task<ApiResult<LoginResponseDto>> LoginAsync(LoginDto loginDto)
    {
        try
        {
            _logger.LogInformation($"Attempting login for user: {loginDto.Email}");
            
            var result = await _httpService.PostAsync<LoginResponseDto>($"{_baseUrl}/login", loginDto);
            
            if (result.IsSuccess && result.Value != null)
            {
                try
                {
                    await _localStorage.SetItemAsync(AccessTokenKey, result.Value.AccessToken);
                    await _localStorage.SetItemAsync(RefreshTokenKey, result.Value.RefreshToken);
                    _logger.LogInformation("Login successful, tokens stored");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to store tokens");
                    // Продовжуємо роботу, навіть якщо зберегти токени не вдалося
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
            return await _localStorage.GetItemAsync<string>(AccessTokenKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting access token");
            return null;
        }
    }

    public async Task Logout()
    {
        try
        {
            await _localStorage.RemoveItemAsync(AccessTokenKey);
            await _localStorage.RemoveItemAsync(RefreshTokenKey);
            _logger.LogInformation("User logged out, tokens removed");
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
}

public class RegisterResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
}