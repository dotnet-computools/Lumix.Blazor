using Lumix.Blazor.Data;
using Lumix.Blazor.Data.Comment;
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
            await LoadCommentsAsync();
            isLoading = false;

        }

        private async Task LoadCommentsAsync()
        {
            var commentsResult = await CommentService.GetCommentByIdAsync(PhotoId);
            if(!commentsResult.IsSuccess || commentsResult.Value is null)
            {
                Snackbar.Add($"Не вдалося завантажити коментарі: {commentsResult.ErrorMessage}", Severity.Error);
                Photo.Comments = new List<CommentDto>();
                return;
            }
            Photo.Comments = commentsResult.Value.ToList();
        }

        void Cancel() => MudDialog.Cancel();

        private async Task HandleCommentAdded(CommentRequest request)
        {
            if (Photo is null || string.IsNullOrWhiteSpace(request.Text))
                return;


            var response = await CommentService.PostCommentAsync(PhotoId, request);
            if (!response.IsSuccess)
            {
                Snackbar.Add($"Не вдалося додати коментар: {response.ErrorMessage}", Severity.Error);
                return;
            }
            await LoadCommentsAsync();
        }
    }
}
