using Lumix.Blazor.Data.User;

namespace Lumix.Blazor.Data.Comment
{
    public class CommentDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid PhotoId { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public Guid? ParentId { get; set; }
        public List<CommentDto> Children { get; set; } = new();
        public UserPreviewDto Author { get; set; } = new();

    }
}