using Lumix.Blazor.Data.Comment;
using Lumix.Blazor.Data.User;

namespace Lumix.Blazor.Data.Photo
{
    public class PhotoDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public int LikeCount { get; set; }
        public bool IsAvatar { get; set; }

        public UserPreviewDto Author { get; set; } = default!;
        public List<LikeDto> Likes { get; set; } = new();
        public List<CommentDto> Comments { get; set; } = new();
        public List<PhotoTagDto> PhotoTags { get; set; } = new();
    }
}
