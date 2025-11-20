using Lumix.Blazor.Data;
using Lumix.Blazor.Models;

namespace Lumix.Blazor.Services.IServices
{
    public interface IUserService
    {
        Task<ApiResult<UserProfileDto>> GetProfileAsync();
    }
}
