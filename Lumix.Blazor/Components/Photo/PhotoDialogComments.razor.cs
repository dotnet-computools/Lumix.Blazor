using System.Runtime.InteropServices;
using Lumix.Blazor.Data.Comment;
using Lumix.Blazor.Data.Photo;
using Lumix.Blazor.Data.User;
using Lumix.Blazor.Services.IServices;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Lumix.Blazor.Components.Photo
{
    public partial class PhotoDialogComments
    {
        [CascadingParameter]
        private MudDialogInstance MudDialog { get; set; } = default!;
        [Parameter] public PhotoDto Photo { get; set; }
        [Parameter] public UserProfileDto CurrentUser { get; set; }
        [Parameter] public Guid CurrentUserId { get; set; }
        [Parameter] public EventCallback<CommentRequest> OnAddComment { get; set; }
        [Inject] public IDialogService DialogService { get; set; }
        [Inject] public IPhotoService PhotoService { get; set; }
        [Inject] public ISnackbar Snackbar { get; set; }

        public string NewComment { get; set; } = string.Empty;
        public Guid? ReplyParentId { get; set; } = null;
        public string? ReplyToUsername { get; set; } = null;
        public bool CanDeletePhoto { get; set; }

        protected override Task OnInitializedAsync()
        {
            CanDeletePhoto = CurrentUserId == Photo.Author.Id;
            return base.OnInitializedAsync();
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
            if (confirmed != true)
                return;
            await PhotoService.DeletePhotoAsync(photoId);

            Snackbar.Add("Фото видалено", Severity.Success);

            MudDialog.Close(DialogResult.Ok(photoId));
        }
    }
}
