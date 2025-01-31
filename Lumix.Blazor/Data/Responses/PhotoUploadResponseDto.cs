namespace Lumix.Blazor.Data.Responses;

public class PhotoUploadResponseDto
{
    public Guid photoId { get; set; }
    public string photoUrl { get; set; } = string.Empty;
    public bool success { get; set; }
    public string? message { get; set; }
}