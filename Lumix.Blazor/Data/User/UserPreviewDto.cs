namespace Lumix.Blazor.Data.User
{
    public class UserPreviewDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
    }
}