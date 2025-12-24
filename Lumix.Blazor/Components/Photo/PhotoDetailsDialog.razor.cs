using Lumix.Blazor.Data.Comment;
using Lumix.Blazor.Data.Photo;
using Lumix.Blazor.Data.User;
using Lumix.Blazor.Services.IServices;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Lumix.Blazor.Components.Photo
{
    public partial class PhotoDetailsDialog
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Parameter] public Guid PhotoId { get; set; }
        [Parameter] public UserPreviewDto Viewer { get; set; }
        [Inject] public IPhotoService PhotoService { get; set; } = default!;
        [Inject] public ICommentService CommentService { get; set; } = default!;
        [Inject] public ISnackbar Snackbar { get; set; } = default!;

        public PhotoDto? Photo { get; set; }
        private bool _isLoading = true;

        protected override async Task OnInitializedAsync()
        {
            var result = await PhotoService.GetPhotoByIdAsync(PhotoId);

            if (!result.IsSuccess || result.Value is null)
            {
                Snackbar.Add("Не вдалося завантажити фото.", Severity.Error);
                _isLoading = false;
                return;
            }

            Photo = result.Value;
            await LoadCommentsAsync();
            _isLoading = false;

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

            var createdComment = response.Value;

            Photo.Comments ??= new List<CommentDto>();

            if (createdComment.ParentId is null)
            {
                Photo.Comments.Add(createdComment);
            }
            else
            {
                var parent = Photo.Comments.FirstOrDefault(c => c.Id == createdComment.ParentId);
                if(parent is not null)
                {
                    parent.Children ??= new List<CommentDto>();
                    parent.Children.Add(createdComment);
                }
            }
            Photo.Comments = Photo.Comments.OrderByDescending(c => c.CreatedAt).ToList();

        }
    }
}
