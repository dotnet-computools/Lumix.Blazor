using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Lumix.Blazor.Models;

public class HttpService
{
    private static readonly HttpClient _httpClient = new HttpClient();
    private readonly ILogger<HttpService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpService(
        ILogger<HttpService> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ApiResult<T?>> GetAsync<T>(string uri)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        return await SendRequestAsync<T>(request);
    }

    public async Task<ApiResult<T>> SendRequestAsync<T>(HttpRequestMessage request)
    {
        try
        {
            var token = _httpContextAccessor.HttpContext?.Request.Cookies["accessToken"];
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            using var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            _logger.LogInformation($"Response status: {response.StatusCode}, Content: {content}");

            if (response.IsSuccessStatusCode)
            {
                if (typeof(T) == typeof(object))
                {
                    return ApiResult<T>.Success(default);
                }

                var result = JsonSerializer.Deserialize<T>(content);
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

    public async Task<ApiResult<T>> PostAsync<T>(string endpoint, object data)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
        request.Content = JsonContent.Create(data);
        return await SendRequestAsync<T>(request);
    }
}
