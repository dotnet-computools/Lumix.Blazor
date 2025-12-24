using Lumix.Blazor.Services.IServices;
using Microsoft.JSInterop;

namespace Lumix.Blazor.Services
{
    public class CookieTokenProvider : ITokenProvider
    {
        private readonly IJSRuntime _js;

        private const string AccessTokenKey = "accessToken";
        private const string RefreshTokenKey = "refreshToken";

        public CookieTokenProvider(IJSRuntime js)
        {
            _js = js;
        }

        public Task<string?> GetAccessTokenAsync()
            => _js.InvokeAsync<string?>("getCookie", AccessTokenKey).AsTask();

        public Task<string?> GetRefreshTokenAsync()
            => _js.InvokeAsync<string?>("getCookie", RefreshTokenKey).AsTask();
    }
}
