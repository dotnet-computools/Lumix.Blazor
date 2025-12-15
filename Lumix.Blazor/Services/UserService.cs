using Lumix.Blazor.Configuration;
using Lumix.Blazor.Data.User;
using Lumix.Blazor.Models;
using Lumix.Blazor.Services.IServices;
using Microsoft.Extensions.Options;

namespace Lumix.Blazor.Services
{
    public class UserService : IUserService
    {
        private readonly HttpService _httpService;
        private readonly ILogger<UserService> _logger;
        private readonly string _url;
        public UserService(HttpService httpService, ILogger<UserService> logger, IOptions<ApiSettings> settings)
        {
            _httpService = httpService;
            _logger = logger;
            var baseUrl = settings.Value.BaseUrl.TrimEnd('/');
            _url = $"{baseUrl}/api/user";
        }

        public async Task<ApiResult<UserProfileDto>> GetProfileAsync()
        {
            try
            {
                _logger.LogInformation("Getting current user");
                var response = await _httpService.GetAsync<UserProfileDto>($"{_url}/profile");
                if (response.IsSuccess && response.Value != null)
                {
                    _logger.LogInformation("Current user retrieved");
                    return ApiResult<UserProfileDto>.Success(response.Value);
                }
                return ApiResult<UserProfileDto>.Failure(response.ErrorMessage ?? "Unknown API error");
            }catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to get current user");
                return ApiResult<UserProfileDto>.Failure(ex.Message);
            }
        }

        public async Task<ApiResult<Guid>> GetMeAsync()
        {
            try
            {
                _logger.LogInformation("Getting current user");
                var response = await _httpService.GetAsync<Guid>($"{_url}/me");
                if (response.IsSuccess && response.Value != null)
                {
                    _logger.LogInformation("Current user retrieved");
                    return ApiResult<Guid>.Success(response.Value);
                }
                return ApiResult<Guid>.Failure(response.ErrorMessage ?? "Unknown API error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get current user");
                return ApiResult<Guid>.Failure(ex.Message);
            }
        }
        }
    }
