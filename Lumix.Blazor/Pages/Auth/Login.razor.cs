using Blazored.LocalStorage;
using Lumix.Blazor.Data;
using Lumix.Blazor.Services.IServices;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Lumix.Blazor.Pages.Auth
{
    public partial class Login : ComponentBase
    {
        [Inject] private IAuthService AuthService { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private ILogger<Login> Logger { get; set; } = default!;
        [Inject] private ILocalStorageService LocalStorage { get; set; } = default!;

        private LoginDto LoginDto { get; set; } = new();
        private bool success;
        private string ErrorMessage = string.Empty;
        private bool IsProcessing;
        private MudForm Form { get; set; } = default!;
        private bool firstRender = true;
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
        
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                this.firstRender = false;
                try
                {
                    if (await AuthService.IsAuthenticated())
                    {
                        NavigationManager.NavigateTo("/dashboard");
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error checking authentication status");
                }

                await InvokeAsync(StateHasChanged);
            }
        }

        private async Task HandleLogin()
        {
            try
            {
                IsProcessing = true;
                ErrorMessage = string.Empty;

                await Form.Validate();
                if (!Form.IsValid)
                {
                    ErrorMessage = "Будь ласка, заповніть усі обов'язкові поля.";
                    return;
                }

                var result = await AuthService.LoginAsync(LoginDto);
                if (result.IsSuccess)
                {
                    success = true;
                    Logger.LogInformation("User successfully logged in: {Email}", LoginDto.email);

                    await Task.Delay(1000);
                    NavigationManager.NavigateTo("/dashboard");
                }
                else
                {
                    ErrorMessage = result.ErrorMessage;
                    Logger.LogWarning("Login failed for user {Email}: {ErrorMessage}",
                        LoginDto.email, result.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error. Try again.";
                Logger.LogError(ex, "Unhandled exception during login for user {Email}", LoginDto.email);
            }
            finally
            {
                IsProcessing = false;
                await InvokeAsync(StateHasChanged);
            }
        }
    }
}
