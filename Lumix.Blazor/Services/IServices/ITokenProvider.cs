namespace Lumix.Blazor.Services.IServices
{
    public interface ITokenProvider
    {
        Task<string?> GetAccessTokenAsync();
        Task<string?> GetRefreshTokenAsync();
    }

}
