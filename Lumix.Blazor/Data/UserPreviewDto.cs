namespace Lumix.Blazor.Data
{
    public class UserPreviewDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
    }
}