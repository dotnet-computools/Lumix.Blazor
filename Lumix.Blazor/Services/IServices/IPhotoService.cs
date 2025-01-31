using Lumix.Blazor.Data;
using Lumix.Blazor.Data.Responses;
using Lumix.Blazor.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace Lumix.Blazor.Services.IServices;

public interface IPhotoService
{
    Task<ApiResult<PhotoUploadResponseDto>> UploadPhotoAsync(PhotoUploadDto uploadDto);
}