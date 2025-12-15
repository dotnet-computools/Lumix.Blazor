using Microsoft.AspNetCore.Components.Forms;

namespace Lumix.Blazor.Data.Photo;

public class PhotoUploadDto
{
    public string? Title { get; set; }
    public IBrowserFile? PhotoFile { get; set; }
    public IEnumerable<string>? Tags { get; set; }
    public bool IsAvatar { get; set; }
}