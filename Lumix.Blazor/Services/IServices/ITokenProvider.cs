namespace Lumix.Blazor.Services.IServices
{
    public interface ITokenProvider
    {
        Task<string?> GetAccessTokenAsync();
        Task<string?> GetRefreshTokenAsync();
        Task SetTokensAsync(string accessToken, string refreshToken);
        Task ClearAsync();
    }

}
