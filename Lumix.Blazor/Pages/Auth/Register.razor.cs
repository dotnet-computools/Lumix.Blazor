using Lumix.Blazor.Data;
using Lumix.Blazor.Services.IServices;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Lumix.Blazor.Pages.Auth
{
    public partial class Register : ComponentBase
    {
        [Inject] private IAuthService AuthService { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private ILogger<Register> Logger { get; set; } = default!;

        private RegisterDto RegisterDto { get; set; } = new();
        private bool success { get; set; }
        private string ErrorMessage { get; set; }
        private bool IsProcessing { get; set; }
        private MudForm Form { get; set; }
        private bool[] activeImages = new bool[3];
        
        private void HandleImageHover(int index)
        {
            activeImages[index] = true;
            StateHasChanged();
        }

        private void HandleImageHoverEnd(int index)
        {
            activeImages[index] = false;
            StateHasChanged();
        }

        private async Task CreateAccount()
        {
            try
            {
                IsProcessing = true;
                ErrorMessage = string.Empty;

                await Form.Validate();
                if (!Form.IsValid)
                {
                    ErrorMessage = "Please fill in all required fields correctly.";
                    return;
                }

                var result = await AuthService.RegisterAsync(RegisterDto);
                if (result.IsSuccess)
                {
                    success = true;
                    await Task.Delay(1000);
                    NavigationManager.NavigateTo("login");
                }
                else
                {
                    ErrorMessage = result.ErrorMessage;
                    Logger.LogError("Registration failed: {ErrorMessage}", result.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "An unexpected error occurred. Please try again.";
                Logger.LogError(ex, "Unhandled exception during registration");
            }
            finally
            {
                IsProcessing = false;
            }
        }
    }
}