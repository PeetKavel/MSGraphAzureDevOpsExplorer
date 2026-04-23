using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace MSGraphAzureDevOpsExplorer.Features.Settings;

public sealed partial class SettingsPage : UserControl
{
    public SettingsViewModel ViewModel { get; }

    public SettingsPage()
    {
        ViewModel = App.Services.GetRequiredService<SettingsViewModel>();
        this.InitializeComponent();
    }
}
