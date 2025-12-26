using Blazored.LocalStorage;
using Lumix.Blazor.Data.Auth;
using Lumix.Blazor.Services;
using Lumix.Blazor.Services.IServices;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Microsoft.AspNetCore.WebUtilities;

namespace Lumix.Blazor.Pages.Auth
{
    public partial class Login : ComponentBase
    {
        [Inject] private IAuthService AuthService { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private ILogger<Login> Logger { get; set; } = default!;
        [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

        private LoginDto LoginDto { get; set; } = new();
        private bool success;
        private string ErrorMessage = string.Empty;
        private bool IsProcessing;
        private string? _returnUrl;
        private MudForm Form { get; set; } = default!;
        private bool firstRender = true;
        private bool[] activeImages = new bool[3];

        protected override void OnInitialized()
        {
            var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);

            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("returnUrl", out var returnUrl))
            {
                _returnUrl = returnUrl;
            }
        }

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
            if (!firstRender) return;

            try
            {
                var state = await AuthStateProvider.GetAuthenticationStateAsync();
                if (state.User.Identity?.IsAuthenticated == true)
                {
                    NavigationManager.NavigateTo("/dashboard");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error checking authentication status");
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
                    NavigationManager.NavigateTo(string.IsNullOrWhiteSpace(_returnUrl) ? "/" : _returnUrl, forceLoad: true);
                }
                else
                {
                    ErrorMessage = result.ErrorMessage;
                    Logger.LogWarning("Login failed for user {Email}: {ErrorMessage}",
                        LoginDto.Email, result.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error. Try again.";
                Logger.LogError(ex, "Unhandled exception during login for user {Email}", LoginDto.Email);
            }
            finally
            {
                IsProcessing = false;
                await InvokeAsync(StateHasChanged);
            }
        }
    }
}
