using Lumix.Blazor.Data.User;
using Lumix.Blazor.Models;

namespace Lumix.Blazor.Services.IServices
{
    public interface IUserService
    {
        Task<ApiResult<UserProfileDto>> GetProfileAsync();
        public Task<ApiResult<Guid>> GetMeAsync();
    }
}
