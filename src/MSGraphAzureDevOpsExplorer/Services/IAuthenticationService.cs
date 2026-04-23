using Microsoft.UI.Xaml;

namespace MSGraphAzureDevOpsExplorer.Services;

public interface IAuthenticationService
{
    event EventHandler<string?> AuthenticationStateChanged;

    bool IsSignedIn { get; }
    string? CurrentUserDisplayName { get; }

    Task SignInAsync(Window window);
    Task SwitchUserAsync(Window window);
    Task SignOutAsync();
    Task<string?> AcquireTokenAsync();
}
