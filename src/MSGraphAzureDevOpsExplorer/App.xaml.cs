using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using MSGraphAzureDevOpsExplorer.Features.SecurityGroups;
using MSGraphAzureDevOpsExplorer.Features.Settings;
using MSGraphAzureDevOpsExplorer.Services;

namespace MSGraphAzureDevOpsExplorer;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    public App()
    {
        this.InitializeComponent();

        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        Services = serviceCollection.BuildServiceProvider();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<ISettingsService, SettingsService>();
        services.AddSingleton<IAuthenticationService, AuthenticationService>();
        services.AddSingleton<IGraphService, GraphService>();

        services.AddTransient<SecurityGroupsViewModel>();
        services.AddTransient<SettingsViewModel>();
    }

    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        var window = new MainWindow();
        window.Activate();
    }
}
