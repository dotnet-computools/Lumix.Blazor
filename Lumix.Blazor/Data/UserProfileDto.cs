namespace Lumix.Blazor.Data
{
    public class UserProfileDto
    {
        public Guid id { get; set; }
        public string username { get; set; } = string.Empty;
        public string profilePictureUrl { get; set; } = string.Empty;
        public string bio { get; set; } = string.Empty;

        public int followersCount { get; set; }
        public int followingCount { get; set; }
        public int photosCount { get; set; }

        public List<PhotoPrewiewDto> photos { get; set; } = new();
    }
}
