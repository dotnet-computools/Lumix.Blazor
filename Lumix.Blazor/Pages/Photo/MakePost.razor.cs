using System.Text.RegularExpressions;
using Lumix.Blazor.Data.Photo;
using Lumix.Blazor.Services.IServices;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace Lumix.Blazor.Pages.Photo
{
    public partial class MakePost
    {
        public string? TagsInput { get; set; }
        private readonly HashSet<string> _previewTags = new(StringComparer.OrdinalIgnoreCase);
        private string? _previewUrl;
        private PhotoUploadDto _model = new();
        private bool isPhotoSelected = false;

        [Inject] IPhotoService PhotoService { get; set; }
        [Inject] ISnackbar Snackbar { get; set; }
        [Inject] NavigationManager NavigationManager { get; set; }

        private static readonly Regex TagSplitRegex =
            new(@"[,\s./]+", RegexOptions.Compiled);

        private void OnFileSelected(IBrowserFile file)
        {
            _model.PhotoFile = file;
            isPhotoSelected = true;
        }

        private void HandleTagKeyDown(KeyboardEventArgs e)
        {
            if (e.Key != "Enter")
                return;

            CommitTags();
        }

        private async Task HandleUpload()
        {
            CommitTags();

            _model.Tags = _previewTags.ToList();

            if (_model.PhotoFile is null)
            {
                Snackbar.Add("Будь ласка, оберіть фото для завантаження.", Severity.Error);
                return;
            }

            var result = await PhotoService.UploadPhotoAsync(_model);

            if (result.IsSuccess)
            {
                Snackbar.Add("Фото успішно опубліковано!", Severity.Success);
                NavigationManager.NavigateTo("/profile");
            }
            else
            {
                Snackbar.Add($"Помилка при публікації фото: {result.ErrorMessage}", Severity.Error);
            }
        }

        private void CommitTags()
        {
            foreach (var tag in ParseTags(TagsInput))
                _previewTags.Add(tag);

            TagsInput = string.Empty;
        }

        private IEnumerable<string> ParseTags(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return Enumerable.Empty<string>();

            return TagSplitRegex
                .Split(input.Trim())
                .Where(t => !string.IsNullOrWhiteSpace(t));
        }
    }
}
