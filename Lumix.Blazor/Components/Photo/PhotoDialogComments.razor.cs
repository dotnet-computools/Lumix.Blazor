using Lumix.Blazor.Data.Comment;
using Lumix.Blazor.Data.Photo;
using Lumix.Blazor.Data.User;
using Microsoft.AspNetCore.Components;

namespace Lumix.Blazor.Components.Photo
{
    public partial class PhotoDialogComments
    {
        [Parameter] public PhotoDto Photo { get; set; }
        [Parameter] public UserProfileDto CurrentUser { get; set; }
        [Parameter] public EventCallback<CommentRequest> OnAddComment { get; set; }

        private string newComment = string.Empty;
        private Guid? replyParentId = null;
        private string? replyToUsername;


        private async Task SubmitComment()
        {
            if (string.IsNullOrWhiteSpace(newComment))
                return;

            var request = new CommentRequest
            {
                Text = newComment,
                ParentId = replyParentId
            };
            await OnAddComment.InvokeAsync(request);
            newComment = string.Empty;
            replyParentId = null;
            replyToUsername = string.Empty;
        }

        private async Task StartReply(CommentDto comment)
        {
            replyParentId = comment.Id;
            replyToUsername = comment.Author.Username;
        }

        private async Task CancelReply()
        {
            replyParentId = null;
            replyToUsername = null;
        }

        private string GetRelativeTime(DateTime createdAt)
        {
            var diff = DateTime.UtcNow - createdAt.ToUniversalTime();

            if (diff.TotalMinutes < 1) return "щойно";
            if (diff.TotalHours < 1) return $"{(int)diff.TotalMinutes} хв тому";
            if (diff.TotalDays < 1) return $"{(int)diff.TotalHours} год тому";
            return $"{(int)diff.TotalDays} дн тому";
        }
    }
}
