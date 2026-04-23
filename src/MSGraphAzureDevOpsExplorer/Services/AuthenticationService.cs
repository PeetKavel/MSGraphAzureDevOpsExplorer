using Microsoft.Identity.Client;
using Microsoft.UI.Xaml;
using WinRT.Interop;

namespace MSGraphAzureDevOpsExplorer.Services;

public class AuthenticationService : IAuthenticationService
{
    private static readonly string[] Scopes = new[]
    {
        "User.Read",
        "GroupMember.Read.All"
    };

    private readonly ISettingsService _settingsService;
    private IPublicClientApplication? _msalClient;
    private IAccount? _currentAccount;

    public event EventHandler<string?> AuthenticationStateChanged = delegate { };

    public bool IsSignedIn => _currentAccount != null;
    public string? CurrentUserDisplayName { get; private set; }

    public AuthenticationService(ISettingsService settingsService)
    {
        _settingsService = settingsService;
        _settingsService.TenantIdChanged += OnTenantIdChanged;
        BuildMsalClient();
    }

    private void OnTenantIdChanged(object? sender, string tenantId)
    {
        BuildMsalClient();
        _currentAccount = null;
        CurrentUserDisplayName = null;
        AuthenticationStateChanged?.Invoke(this, null);
    }

    private void BuildMsalClient()
    {
        _msalClient = PublicClientApplicationBuilder
            .Create(_settingsService.ClientId)
            .WithAuthority(AzureCloudInstance.AzurePublic, _settingsService.TenantId)
            .WithDefaultRedirectUri()
            .Build();
    }

    public async Task SignInAsync(Window window)
    {
        try
        {
            var result = await AcquireTokenInteractiveAsync(window, Prompt.SelectAccount);
            if (result != null)
            {
                _currentAccount = result.Account;
                CurrentUserDisplayName = result.Account.Username;
                AuthenticationStateChanged?.Invoke(this, CurrentUserDisplayName);
            }
        }
        catch (MsalException ex) when (ex.ErrorCode == MsalError.AuthenticationCanceledError)
        {
            // User cancelled – no action needed
        }
    }

    public async Task SwitchUserAsync(Window window)
    {
        try
        {
            var result = await AcquireTokenInteractiveAsync(window, Prompt.SelectAccount);
            if (result != null)
            {
                _currentAccount = result.Account;
                CurrentUserDisplayName = result.Account.Username;
                AuthenticationStateChanged?.Invoke(this, CurrentUserDisplayName);
            }
        }
        catch (MsalException ex) when (ex.ErrorCode == MsalError.AuthenticationCanceledError)
        {
            // User cancelled – no action needed
        }
    }

    public async Task SignOutAsync()
    {
        if (_currentAccount != null && _msalClient != null)
        {
            await _msalClient.RemoveAsync(_currentAccount);
            _currentAccount = null;
            CurrentUserDisplayName = null;
            AuthenticationStateChanged?.Invoke(this, null);
        }
    }

    public async Task<string?> AcquireTokenAsync()
    {
        if (_msalClient == null || _currentAccount == null)
            return null;

        try
        {
            var result = await _msalClient
                .AcquireTokenSilent(Scopes, _currentAccount)
                .ExecuteAsync();
            return result.AccessToken;
        }
        catch (MsalUiRequiredException)
        {
            // Token expired or not cached – caller should trigger interactive sign-in
            return null;
        }
    }

    private async Task<AuthenticationResult?> AcquireTokenInteractiveAsync(Window window, Prompt prompt)
    {
        if (_msalClient == null)
            return null;

        var hwnd = WindowNative.GetWindowHandle(window);
        return await _msalClient
            .AcquireTokenInteractive(Scopes)
            .WithParentActivityOrWindow(hwnd)
            .WithPrompt(prompt)
            .ExecuteAsync();
    }
}
