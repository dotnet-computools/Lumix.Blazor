using Lumix.Blazor.Components.Photo;
using Lumix.Blazor.Data.User;
using Lumix.Blazor.Services.IServices;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Lumix.Blazor.Pages
{
    public partial class Profile
    {
        public UserProfileDto? ProfileDto { get; set; } = new();
        [Inject] private IUserService UserService { get; set; } = null!;
        [Inject] private IDialogService DialogService { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            var result = await UserService.GetProfileAsync();
            ProfileDto = result.Value;
        }

        private async Task OpenPhotoDialog(Guid photoId)
        {
            var options = new DialogOptions
            {
                MaxWidth = MaxWidth.ExtraLarge,
                FullWidth = true,
                CloseOnEscapeKey = true,
                CloseButton = true
            };

            var parameters = new DialogParameters { { "PhotoId", photoId }, { "CurrentUser", ProfileDto } };

            var dialog = await DialogService.ShowAsync<PhotoDetailsDialog>("", parameters, options);
            var result = await dialog.Result;

            if (!result.Cancelled && result.Data is Guid deletedPhotoId)
            {
                ProfileDto?.Photos.RemoveAll(p => p.Id == deletedPhotoId);
            }

        }
    }
}
