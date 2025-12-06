using Lumix.Blazor.Data.Comment;
using Lumix.Blazor.Models;
using Lumix.Blazor.Services.IServices;

namespace Lumix.Blazor.Services
{
    public class CommentService : ICommentService
    {
        private readonly HttpService _httpService;
        private readonly ILogger<PhotoService> _logger;
        private readonly string _baseUrl = "https://localhost:7231/api/comment";

        public CommentService(HttpService httpService, ILogger<PhotoService> logger)
        {
            _httpService = httpService;
            _logger = logger;
        }

        public async Task<ApiResult<IEnumerable<CommentDto>>> GetCommentByIdAsync(Guid photoId)
        {
            try
            {
                var url = $"{_baseUrl}/all/{photoId}";
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

        public async Task<ApiResult<bool>> PostCommentAsync(Guid photoId, CommentRequest comment)
        {
            try
            {
                using var content = new MultipartFormDataContent();
                content.Add(new StringContent(comment.Text), "Text");

                if (comment.ParentId.HasValue)
                    content.Add(new StringContent(comment.ParentId.Value.ToString()), "ParentId");

                var url = $"{_baseUrl}/{photoId}";

                var response = await _httpService.PostFormAsync<object>(url, content);

                if(!response.IsSuccess)
                    return ApiResult<bool>.Failure(response.ErrorMessage ?? "Не вдалося додати коментар");

                return ApiResult<bool>.Success(true);
            }catch (Exception ex)
            {
                _logger.LogError(ex, "Error posting comment for photo {PhotoId}", photoId);
                return ApiResult<bool>.Failure($"Помилка при відправці коментаря: {ex.Message}");
            }
        }
    }
}
