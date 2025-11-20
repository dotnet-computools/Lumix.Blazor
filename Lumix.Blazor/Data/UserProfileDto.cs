namespace Lumix.Blazor.Data
{
    public class UserProfileDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string ProfilePictureUrl { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;

        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }
        public int PhotosCount { get; set; }

        public List<PhotoPrewiewDto> Photos { get; set; } = new();
    }
}
