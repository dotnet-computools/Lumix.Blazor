namespace Lumix.Blazor.Data.Responses;

public class PhotoUploadResponseDto
{
    public Guid PhotoId { get; set; }
    public string PhotoUrl { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? Message { get; set; }
}