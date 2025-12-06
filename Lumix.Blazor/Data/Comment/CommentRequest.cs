namespace Lumix.Blazor.Data.Comment
{
    public class CommentRequest
    {
        public string Text { get; set; } = string.Empty;
        public Guid? ParentId { get; set; }
    }
}
