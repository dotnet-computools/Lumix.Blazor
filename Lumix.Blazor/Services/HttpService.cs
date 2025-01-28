using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Blazored.LocalStorage;
using Lumix.Blazor.Models;

public class HttpService
{
    private static readonly HttpClient _httpClient = new HttpClient();
    private readonly ILogger<HttpService> _logger;
    private readonly ILocalStorageService _localStorage;

    public HttpService(
        ILogger<HttpService> logger,
        ILocalStorageService localStorage)
    {
        _logger = logger;
        _localStorage = localStorage;
    }

    private async Task<ApiResult<T>> SendRequestAsync<T>(HttpRequestMessage request)
    {
        try
        {
            var token = await _localStorage.GetItemAsync<string>("access_token");
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

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await _localStorage.RemoveItemAsync("access_token");
                await _localStorage.RemoveItemAsync("refresh_token");
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