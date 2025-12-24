using Lumix.Blazor.Data.Comment;
using Lumix.Blazor.Data.Photo;
using Lumix.Blazor.Data.User;
using Lumix.Blazor.Extensions;
using Lumix.Blazor.Services.IServices;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace Lumix.Blazor.Components.Photo
{
    public partial class PhotoDialogComments
    {
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = null!;
        [Parameter] public PhotoDto Photo { get; set; }
        [Parameter] public UserPreviewDto? Viewer { get; set; }
        [Parameter] public EventCallback<CommentRequest> OnAddComment { get; set; }
        [Inject] public IDialogService DialogService { get; set; }
        [Inject] public IPhotoService PhotoService { get; set; }
        [Inject] public ICommentService CommentService { get; set; }
        [Inject] public ISnackbar Snackbar { get; set; }
        [Inject] public AuthenticationStateProvider AuthStateProvider { get; set; } = null!;
        [Inject] public NavigationManager NavigationManager { get; set; } = null!;

        public string NewComment { get; set; } = string.Empty;
        public Guid? ReplyParentId { get; set; } = null;
        public string? ReplyToUsername { get; set; } = null;
        private Guid? _currentUserId;
        public bool CanDeletePhoto { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthStateProvider.GetAuthenticationStateAsync();
            _currentUserId = authState.User.GetUserId();
            StateHasChanged();
        }

        protected override void OnParametersSet()
        {
            CanDeletePhoto = _currentUserId.HasValue
                             && _currentUserId.Value == Photo.Author.Id;
        }

        private async Task SubmitComment()
        {
            if (string.IsNullOrWhiteSpace(NewComment))
                return;

            var request = new CommentRequest
            {
                Text = NewComment,
                ParentId = ReplyParentId
            };
            await OnAddComment.InvokeAsync(request);
            NewComment = string.Empty;
            ReplyParentId = null;
            ReplyToUsername = string.Empty;
        }

        private Task StartReply(CommentDto comment)
        {
            ReplyParentId = comment.Id;
            ReplyToUsername = comment.Author.Username;
            return Task.CompletedTask;
        }

        private Task CancelReply()
        {
            ReplyParentId = null;
            ReplyToUsername = null;
            return Task.CompletedTask;
        }

        private string GetRelativeTime(DateTime createdAt)
        {
            var diff = DateTime.UtcNow - createdAt.ToUniversalTime();

            if (diff.TotalMinutes < 1) return "щойно";
            if (diff.TotalHours < 1) return $"{(int)diff.TotalMinutes} хв тому";
            if (diff.TotalDays < 1) return $"{(int)diff.TotalHours} год тому";
            return $"{(int)diff.TotalDays} дн тому";
        }

        private async Task ConfirmDeletePhoto(Guid photoId)
        {
            var confirmed = await DialogService.ShowMessageBox(
                "Видалити фото?",
                "Фото буде видалено назавжди.",
                yesText: "Видалити",
                cancelText: "Скасувати"
            );
            if (!confirmed!.Value)
                return;
            await PhotoService.DeletePhotoAsync(photoId);

            Snackbar.Add("Фото видалено", Severity.Success);

            MudDialog.Close(DialogResult.Ok(photoId));
        }

        private async Task ConfirmDeleteComment(Guid commentId)
        {
            var confirmed = await DialogService.ShowMessageBox(
                title: "Видалити коментар?",
                message: "Коментар буде видалено назавжди.",
                yesText: "Видалити",
                cancelText: "Скасувати"
            );
            if (!confirmed!.Value)
                return;
            await CommentService.DeleteCommentAsync(commentId, Photo.Id);
            DeleteCommentFromDto(commentId);

            Snackbar.Add("Коментар видалено", Severity.Success);
            StateHasChanged();
        }

        private bool CanDeleteComment(CommentDto comment)
        {
            return _currentUserId.HasValue
                   && (
                       comment.Author.Id == _currentUserId.Value
                       || Photo.Author.Id == _currentUserId.Value
                   );
        }

        private void DeleteCommentFromDto(Guid commentId)
        {
            var root = Photo.Comments.FirstOrDefault(c => c.Id == commentId);
            if (root != null)
            {
                Photo.Comments.Remove(root);
                return;
            }

            foreach (var comment in Photo.Comments)
            {
                var reply = comment.Children.FirstOrDefault(c => c.Id == commentId);
                if (reply != null)
                {
                    comment.Children.Remove(reply);
                    return;
                }
            }
        }

        private void OpenUserProfile(Guid userId)
        {
            MudDialog.Close();
            NavigationManager.NavigateTo($"/profile/{userId}");
        }
    }
}
