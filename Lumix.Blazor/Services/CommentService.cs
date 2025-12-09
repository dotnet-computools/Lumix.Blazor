using Lumix.Blazor.Configuration;
using Lumix.Blazor.Data.Comment;
using Lumix.Blazor.Models;
using Lumix.Blazor.Services.IServices;

namespace Lumix.Blazor.Services
{
    public class CommentService : ICommentService
    {
        private readonly HttpService _httpService;
        private readonly ILogger<PhotoService> _logger;
        private readonly string _url;

        public CommentService(HttpService httpService, ILogger<PhotoService> logger, ApiSettings settings)
        {
            _httpService = httpService;
            _logger = logger;
            var baseUrl = settings.BaseUrl.TrimEnd('/');
            _url = $"{baseUrl}/api/comment";
        }

        public async Task<ApiResult<IEnumerable<CommentDto>>> GetCommentByIdAsync(Guid photoId)
        {
            try
            {
                var url = $"{_url}/all/{photoId}";
                var response = await _httpService.GetAsync<IEnumerable<CommentDto>>(url);
                if (!response.IsSuccess || response.Value is null)
                    return ApiResult<IEnumerable<CommentDto>>.Failure(response.ErrorMessage ?? "Не вдалося завантажити коментарі");

                return ApiResult<IEnumerable<CommentDto>>.Success(response.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading comments for photo {PhotoId}", photoId);
                return ApiResult<IEnumerable<CommentDto>>.Failure($"Помилка при завантаженні коментарів: {ex.Message}");
            }
        }

        public async Task<ApiResult<CommentDto>> PostCommentAsync(Guid photoId, CommentRequest comment)
        {
            try{

                var url = $"{_url}/{photoId}";

                var response = await _httpService.PostAsync<CommentDto>(url, comment);

                if(!response.IsSuccess)
                    return ApiResult<CommentDto>.Failure(response.ErrorMessage ?? "Не вдалося додати коментар");

                return ApiResult<CommentDto>.Success(response.Value);
            }catch (Exception ex)
            {
                _logger.LogError(ex, "Error posting comment for photo {PhotoId}", photoId);
                return ApiResult<CommentDto>.Failure($"Помилка при відправці коментаря: {ex.Message}");
            }
        }
    }
}
