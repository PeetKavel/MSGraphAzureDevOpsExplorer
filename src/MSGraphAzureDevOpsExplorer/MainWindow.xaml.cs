using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using MSGraphAzureDevOpsExplorer.Services;

namespace MSGraphAzureDevOpsExplorer;

public sealed partial class MainWindow : Window
{
    private readonly IAuthenticationService _authService;

    public MainWindow()
    {
        this.InitializeComponent();

        _authService = App.Services.GetRequiredService<IAuthenticationService>();
        _authService.AuthenticationStateChanged += OnAuthenticationStateChanged;

        UpdateAuthUI(null);
    }

    private void OnAuthenticationStateChanged(object? sender, string? displayName)
    {
        DispatcherQueue.TryEnqueue(() => UpdateAuthUI(displayName));
    }

    private void UpdateAuthUI(string? displayName)
    {
        if (string.IsNullOrEmpty(displayName))
        {
            UserDisplayName.Text = "Not signed in";
            SignInButton.Visibility = Visibility.Visible;
            SwitchUserButton.Visibility = Visibility.Collapsed;
            SignOutButton.Visibility = Visibility.Collapsed;
        }
        else
        {
            UserDisplayName.Text = displayName;
            SignInButton.Visibility = Visibility.Collapsed;
            SwitchUserButton.Visibility = Visibility.Visible;
            SignOutButton.Visibility = Visibility.Visible;
        }
    }

    private async void SignInButton_Click(object sender, RoutedEventArgs e)
    {
        SignInButton.IsEnabled = false;
        try
        {
            await _authService.SignInAsync(this);
        }
        finally
        {
            SignInButton.IsEnabled = true;
        }
    }

    private async void SwitchUserButton_Click(object sender, RoutedEventArgs e)
    {
        SwitchUserButton.IsEnabled = false;
        try
        {
            await _authService.SwitchUserAsync(this);
        }
        finally
        {
            SwitchUserButton.IsEnabled = true;
        }
    }

    private async void SignOutButton_Click(object sender, RoutedEventArgs e)
    {
        SignOutButton.IsEnabled = false;
        try
        {
            await _authService.SignOutAsync();
        }
        finally
        {
            SignOutButton.IsEnabled = true;
        }
    }
}
