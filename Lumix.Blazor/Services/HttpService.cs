using Lumix.Blazor.Models;
using Lumix.Blazor.Services.IServices;
using System.Net.Http.Headers;
using System.Text.Json;

public class HttpService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HttpService> _logger;
    private readonly ITokenProvider _tokenProvider;
    private readonly JsonSerializerOptions _jsonOptions;

    public HttpService(
        HttpClient httpClient,
        ILogger<HttpService> logger,
        ITokenProvider tokenProvider)
    {
        _httpClient = httpClient;
        _logger = logger;
        _tokenProvider = tokenProvider;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    private async Task AttachAuthAsync(HttpRequestMessage request)
    {
        var token = await _tokenProvider.GetAccessTokenAsync();
        if (!string.IsNullOrWhiteSpace(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<ApiResult<T?>> GetAsync<T>(string uri)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, uri);
        await AttachAuthAsync(request);
        return await SendRequestAsync<T>(request);
    }

    public async Task<ApiResult<T>> PostAsync<T>(string endpoint, object data)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = JsonContent.Create(data, options: _jsonOptions)
        };
        await AttachAuthAsync(request);
        return await SendRequestAsync<T>(request);
    }

    public async Task<ApiResult<T>> PostFormAsync<T>(string endpoint, MultipartFormDataContent formData)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, endpoint) { Content = formData };
        await AttachAuthAsync(request);
        return await SendRequestAsync<T>(request);
    }

    public async Task<ApiResult<T>> DeleteAsync<T>(string uri)
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, uri);
        await AttachAuthAsync(request);
        return await SendRequestAsync<T>(request);
    }

    public async Task<ApiResult<T>> SendRequestAsync<T>(HttpRequestMessage request)
    {
        try
        {
            using var response = await _httpClient.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return ApiResult<T>.Success(default);

            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Response status: {StatusCode}", response.StatusCode);

            if (response.IsSuccessStatusCode)
            {
                if (typeof(T) == typeof(object))
                    return ApiResult<T>.Success(default);

                var result = JsonSerializer.Deserialize<T>(content, _jsonOptions);
                return ApiResult<T>.Success(result);
            }

            return ApiResult<T>.Failure(content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Request failed");
            return ApiResult<T>.Failure($"Request failed: {ex.Message}");
        }
    }
}

