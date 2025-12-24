using Lumix.Blazor.Components.Photo;
using Lumix.Blazor.Data.User;
using Lumix.Blazor.Services.IServices;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Lumix.Blazor.Pages
{
    public partial class Profile
    {
        [Parameter] public Guid? UserId { get; set; }
        public UserPreviewDto? Viewer { get; set; }
        public UserProfileDto? ProfileDto { get; set; }
        [Inject] private IUserService UserService { get; set; } = null!;
        [Inject] private IDialogService DialogService { get; set; } = null!;
        [Inject] private ISnackbar Snackbar { get; set; } = null!;
        public bool IsLoading { get; set; }
        public string? Error { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            await LoadProfile();
        }

        private async Task LoadProfile()
        {
            IsLoading = true;
            var meResult = await UserService.GetMyProfileAsync();
            if (!meResult.IsSuccess)
            {
                IsLoading = false;
                Snackbar.Add("Failed to load your profile info.", Severity.Error);
                return;
            }

            var me = meResult.Value;

            Viewer = new UserPreviewDto()
            {
                Id = me.Id,
                ProfilePictureUrl = me.ProfilePictureUrl,
                Username = me.Username
            };



            var profileResult = UserId is null
                ? meResult
                : await UserService.GetProfileAsync(UserId.Value);

            if (!profileResult.IsSuccess)
            {
                IsLoading = false;
                Snackbar.Add("Failed to load user profile.", Severity.Error);
                return;
            }

            ProfileDto = profileResult.Value;
            IsLoading = false;
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

            var parameters = new DialogParameters { { "PhotoId", photoId }, { "Viewer", Viewer } };

            var dialog = await DialogService.ShowAsync<PhotoDetailsDialog>("", parameters, options);
            var result = await dialog.Result;

            if (!result.Cancelled && result.Data is Guid deletedPhotoId)
            {
                ProfileDto?.Photos.RemoveAll(p => p.Id == deletedPhotoId);
            }

        }
    }
}
