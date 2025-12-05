using Lumix.Blazor.Data;
using Lumix.Blazor.Data.Photo;
using Lumix.Blazor.Data.User;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Lumix.Blazor.Components.Photo
{
    public partial class PhotoDetailsDialog
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Parameter] public Guid PhotoId { get; set; }
        [Parameter] public UserProfileDto CurrentUser { get; set; }



        public PhotoDto Photo { get; set; }
        private bool isLoading = true;

        protected override async Task OnInitializedAsync()
        {
            var result = await PhotoService.GetPhotoByIdAsync(PhotoId);

            if (!result.IsSuccess || result.Value is null)
            {
                Snackbar.Add("Не вдалося завантажити фото.", Severity.Error);
                isLoading = false;
                return;
            }

            Photo = result.Value;
            isLoading = false;
        }

        void Cancel() => MudDialog.Cancel();

        private Task HandleCommentAdded(string comment)
        {
            if(Photo is null || string.IsNullOrWhiteSpace(comment))
            {
                return Task.CompletedTask;
            }
            Photo.Comments.Add(new CommentDto
            {
                Id = Guid.NewGuid(),
                PhotoId = Photo.Id,
                UserId = CurrentUser.Id,
                Text = comment,
                CreatedAt = DateTime.UtcNow
            });

            //API Request

            return Task.CompletedTask;
        }
    }
}
