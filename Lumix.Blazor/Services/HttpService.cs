using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Lumix.Blazor.Models;


public class HttpService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HttpService> _logger;

    public HttpService(HttpClient httpClient, ILogger<HttpService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    private async Task<ApiResult<T?>> SendRequestAsync<T>(HttpRequestMessage request, string? token)
    {
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        try
        {
            using var response = await _httpClient.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                
                return ApiResult<T>.Failure("Unauthorized");
            }

            var json = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var data = JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return ApiResult<T>.Success(data);
            }

            return ApiResult<T>.Failure(json);
        }
        catch (Exception ex)
        {
            return ApiResult<T>.Failure(ex.Message);
        }
    }

    public async Task<ApiResult<T?>> GetAsync<T>(string uri, string? token = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        return await SendRequestAsync<T>(request, token);
    }

    public async Task<ApiResult<T>> PostAsync<T>(string endpoint, object data)
    {
        try
        {
            _logger.LogInformation($"Preparing POST request to {endpoint}");
            
            using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            request.Content = JsonContent.Create(data);
            
            using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            
            _logger.LogInformation($"Response received: {response.StatusCode}");
            
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"Response content: {content}");
            
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
            _logger.LogError($"Request failed: {ex.Message}");
            return ApiResult<T>.Failure($"Request failed: {ex.Message}");
        }
    }


    public async Task<ApiResult<T?>> PutAsync<T>(string uri, object value, string? token = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, uri)
        {
            Content = new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json")
        };
        return await SendRequestAsync<T>(request, token);
    }

    public async Task<ApiResult<T?>> DeleteAsync<T>(string uri, string? token = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, uri);
        return await SendRequestAsync<T>(request, token);
    }
    
}