using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace MSGraphAzureDevOpsExplorer.Features.SecurityGroups;

public sealed partial class SecurityGroupsPage : UserControl
{
    public SecurityGroupsViewModel ViewModel { get; }

    public SecurityGroupsPage()
    {
        ViewModel = App.Services.GetRequiredService<SecurityGroupsViewModel>();
        this.InitializeComponent();
    }
}
