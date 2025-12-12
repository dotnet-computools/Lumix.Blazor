using Lumix.Blazor.Data.Photo;
using Lumix.Blazor.Data.Responses;
using Lumix.Blazor.Models;

namespace Lumix.Blazor.Services.IServices;

public interface IPhotoService
{
    Task<ApiResult<PhotoUploadResponseDto>> UploadPhotoAsync(PhotoUploadDto uploadDto);
    Task<ApiResult<PhotoDto>> GetPhotoByIdAsync(Guid photoId);
}