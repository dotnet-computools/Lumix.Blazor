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

        public string NewComment { get; set; } = string.Empty;
        public Guid? ReplyParentId { get; set; } = null;
        public string? ReplyToUsername { get; set; } = null;


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
    }
}
