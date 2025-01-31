using Microsoft.AspNetCore.Components.Forms;

namespace Lumix.Blazor.Data;

public class PhotoUploadDto
{
    public string title { get; set; } = string.Empty;
    public IBrowserFile? photoFile { get; set; }
    public IEnumerable<string>? tags { get; set; }
}