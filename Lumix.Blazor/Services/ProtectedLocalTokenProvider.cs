using Lumix.Blazor.Services.IServices;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace Lumix.Blazor.Services
{
    public class ProtectedLocalTokenProvider : ITokenProvider
    {
        private readonly ProtectedLocalStorage _protectedLocalStorage;
        private const string AccessKey = "accessToken";
        private const string RefreshKey = "refreshToken";

        public ProtectedLocalTokenProvider(ProtectedLocalStorage protectedLocalStorage)
        {
            _protectedLocalStorage = protectedLocalStorage;
        }

        public async Task<string?> GetAccessTokenAsync()
        {
            var res = await _protectedLocalStorage.GetAsync<string>(AccessKey);
            return res.Success ? res.Value : null;
        }
        public async Task<string?> GetRefreshTokenAsync()
        {
            var res = await _protectedLocalStorage.GetAsync<string>(RefreshKey);
            return res.Success ? res.Value : null;
        }

        public async Task SetTokensAsync(string accessToken, string refreshToken)
        => await Task.WhenAll(
            _protectedLocalStorage.SetAsync(AccessKey, accessToken).AsTask(),
            _protectedLocalStorage.SetAsync(RefreshKey, refreshToken).AsTask()
        );

        public async Task ClearAsync()
        => await Task.WhenAll(
            _protectedLocalStorage.DeleteAsync(AccessKey).AsTask(),
            _protectedLocalStorage.DeleteAsync(RefreshKey).AsTask()
        );
    }
}
