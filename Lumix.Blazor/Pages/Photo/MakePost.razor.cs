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
        private List<string> _previewTags = new();
        private string? _previewUrl;
        private PhotoUploadDto _model = new();
        private bool isPhotoSelected = false;

        [Inject] IPhotoService PhotoService { get; set; }
        [Inject] ISnackbar Snackbar { get; set; }
        [Inject] NavigationManager NavigationManager { get; set; }

        private async Task OnFileSelected(IBrowserFile file)
        {
            Console.WriteLine("FILE SELECTED: " + (file?.Name ?? "NULL"));
            _model.PhotoFile = file;
            isPhotoSelected = true;
        }

        private async Task HandleTagKeyDown(KeyboardEventArgs e)
        {
            Console.WriteLine($"Key pressed: {e.Key}");
            if (e.Key != "Enter")
                return;

            var raw = TagsInput.Trim();
            if (string.IsNullOrWhiteSpace(raw))
                return;

            var parts = raw
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (parts.Length == 1)
            {
                var tag = parts[0];
                if (!_previewTags.Contains(tag, StringComparer.OrdinalIgnoreCase))
                {
                    _previewTags.Add(tag);
                }
            }
            else
            {
                foreach (var tag in parts)
                {
                    if (!_previewTags.Contains(tag, StringComparer.OrdinalIgnoreCase))
                        _previewTags.Add(tag);
                }
            }
            TagsInput = string.Empty;
        }


        private async Task HandleUpload()
        {
            if (!string.IsNullOrWhiteSpace(TagsInput))
            {
                var raw = TagsInput.Trim();
                var parts = raw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                foreach (var tag in parts)
                {
                    if (!_previewTags.Contains(tag, StringComparer.OrdinalIgnoreCase))
                        _previewTags.Add(tag);
                }
            }

            _model.Tags = new List<string>(_previewTags);
            if (_model.PhotoFile == null)
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
    }
}
