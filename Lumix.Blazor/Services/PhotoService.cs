using System.Net.Http.Headers;
using Lumix.Blazor.Configuration;
using Lumix.Blazor.Data.Photo;
using Lumix.Blazor.Data.Responses;
using Lumix.Blazor.Models;
using Lumix.Blazor.Services.IServices;
using Microsoft.Extensions.Options;

public class PhotoService : IPhotoService
{
    private readonly HttpService _httpService;
    private readonly ILogger<PhotoService> _logger;
    private readonly string _url;

    public PhotoService(HttpService httpService, ILogger<PhotoService> logger, IOptions<ApiSettings> settings)
    {
        _httpService = httpService;
        _logger = logger;
        var baseUrl = settings.Value.BaseUrl.TrimEnd('/');
        _url = $"{baseUrl}/api/photo";
    }

    public async Task<ApiResult<PhotoUploadResponseDto>> UploadPhotoAsync(PhotoUploadDto uploadDto)
    {
        try
        {
            if (uploadDto.PhotoFile == null)
            {
                return ApiResult<PhotoUploadResponseDto>.Failure("Photo file is required.");
            }

            using var content = new MultipartFormDataContent();
            
            content.Add(new StringContent(uploadDto.Title ?? string.Empty), "Title");
            content.Add(new StringContent(uploadDto.IsAvatar.ToString().ToLowerInvariant()), "IsAvatar");
            
            var fileContent = new StreamContent(uploadDto.PhotoFile.OpenReadStream(maxAllowedSize: 10485760));
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(uploadDto.PhotoFile.ContentType);
            content.Add(fileContent, "PhotoFile", uploadDto.PhotoFile.Name);
            
            if (uploadDto.Tags != null)
            {
                foreach (var tag in uploadDto.Tags)
                {
                    content.Add(new StringContent(tag), "Tags");
                }
            }

            var response = await _httpService.PostFormAsync<PhotoUploadResponseDto>($"{_url}/upload", content);
            
            if(!response.IsSuccess)
            {
                return ApiResult<PhotoUploadResponseDto>.Failure(response.ErrorMessage ?? "Upload failed");
            }
            return ApiResult<PhotoUploadResponseDto>.Success(response.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading photo");
            return ApiResult<PhotoUploadResponseDto>.Failure($"Upload failed: {ex.Message}");
        }
    }

    public async Task<ApiResult<PhotoDto>> GetPhotoByIdAsync(Guid photoId)
    {
        try
        {
            var url = $"{_url}/{photoId}";
            var response = await _httpService.GetAsync<PhotoDto>(url);
            
            if(!response.IsSuccess || response.Value is null)
                return ApiResult<PhotoDto>.Failure(response.ErrorMessage ?? "Photo not found");

            return ApiResult<PhotoDto>.Success(response.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading photo {PhotoId}", photoId);
            return ApiResult<PhotoDto>.Failure($"Failed to load photo: {ex.Message}");
        }
    }
}
