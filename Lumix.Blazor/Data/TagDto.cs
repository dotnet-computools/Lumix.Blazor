namespace Lumix.Blazor.Data
{
    public class TagDto
    {
        private const string PATTERN = @"^#[a-zA-Z0-9]{1,29}$";

        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<PhotoTagDto> PhotoTags { get; set; }

    }
}