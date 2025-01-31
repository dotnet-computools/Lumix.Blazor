using System.Net.Http.Headers;
using Lumix.Blazor.Data;
using Lumix.Blazor.Data.Responses;
using Lumix.Blazor.Models;
using Lumix.Blazor.Services.IServices;

public class PhotoService : IPhotoService
{
    private readonly HttpService _httpService;
    private readonly ILogger<PhotoService> _logger;
    private readonly string _baseUrl = "https://localhost:7231/api/photo";

    public PhotoService(HttpService httpService, ILogger<PhotoService> logger)
    {
        _httpService = httpService;
        _logger = logger;
    }

    public async Task<ApiResult<PhotoUploadResponseDto>> UploadPhotoAsync(PhotoUploadDto uploadDto)
    {
        try
        {
            if (uploadDto.photoFile == null)
            {
                return ApiResult<PhotoUploadResponseDto>.Failure("Photo file is required.");
            }

            using var content = new MultipartFormDataContent();
            
            content.Add(new StringContent(uploadDto.title), "Title");
            
            var fileContent = new StreamContent(uploadDto.photoFile.OpenReadStream(maxAllowedSize: 10485760));
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(uploadDto.photoFile.ContentType);
            content.Add(fileContent, "PhotoFile", uploadDto.photoFile.Name);
            
            if (uploadDto.tags != null)
            {
                foreach (var tag in uploadDto.tags)
                {
                    content.Add(new StringContent(tag), "Tags[]");  
                }
            }

            var response = await _httpService.PostAsync<PhotoUploadResponseDto>($"{_baseUrl}/upload", content);
            
            
            return ApiResult<PhotoUploadResponseDto>.Failure(response.ErrorMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading photo");
            return ApiResult<PhotoUploadResponseDto>.Failure($"Upload failed: {ex.Message}");
        }
    }
}
