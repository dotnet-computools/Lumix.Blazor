using Microsoft.AspNetCore.Components.Forms;

namespace Lumix.Blazor.Data;

public class PhotoUploadDto
{
    public string Title { get; set; } = string.Empty;
    public IBrowserFile? PhotoFile { get; set; }
    public IEnumerable<string>? Tags { get; set; }
    public bool IsAvatar { get; set; }
}