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
        private string _newComment = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            var result = await PhotoService.GetPhotoByIdAsync(PhotoId);

            if (!result.IsSuccess || result.Value is null)
            {
                Snackbar.Add("Не вдалося завантажити фото.", Severity.Error);
                return;
            }

            Photo = result.Value;
            isLoading = false;
        }

        void Cancel() => MudDialog.Cancel();

        async Task SubmitComment()
        {
            if (string.IsNullOrWhiteSpace(_newComment) || Photo is null)
                return;


            Photo.Comments.Add(new CommentDto
            {
                Id = Guid.NewGuid(),
                Text = _newComment,
                CreatedAt = DateTime.UtcNow
            });

            _newComment = string.Empty;
        }
    }
}
