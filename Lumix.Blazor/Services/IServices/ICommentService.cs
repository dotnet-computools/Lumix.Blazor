using Lumix.Blazor.Data.Comment;
using Lumix.Blazor.Data.Photo;
using Lumix.Blazor.Data.Responses;
using Lumix.Blazor.Models;

namespace Lumix.Blazor.Services.IServices
{
    public interface ICommentService
    {
        Task<ApiResult<CommentDto>> PostCommentAsync(Guid photoId, CommentRequest comment);
        Task<ApiResult<IEnumerable<CommentDto>>> GetCommentByIdAsync(Guid photoId);
    }
}
