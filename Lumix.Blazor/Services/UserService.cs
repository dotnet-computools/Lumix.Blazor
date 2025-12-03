using Lumix.Blazor.Data.User;
using Lumix.Blazor.Models;
using Lumix.Blazor.Services.IServices;

namespace Lumix.Blazor.Services
{
    public class UserService : IUserService
    {
        private readonly HttpService _httpService;
        private readonly ILogger<UserService> _logger;
        private readonly string _baseUrl = "https://localhost:7231/api/user";
        public UserService(HttpService httpService, ILogger<UserService> logger)
        {
            _httpService = httpService;
            _logger = logger;
        }

        public async Task<ApiResult<UserProfileDto>> GetProfileAsync()
        {
            try
            {
                _logger.LogInformation("Getting current user");
                var response = await _httpService.GetAsync<UserProfileDto>($"{_baseUrl}/profile");
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
    }
}
