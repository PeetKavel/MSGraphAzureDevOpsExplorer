using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MSGraphAzureDevOpsExplorer.Services;

namespace MSGraphAzureDevOpsExplorer.Features.Settings;

public partial class SettingsViewModel : ObservableObject
{
    private readonly ISettingsService _settingsService;

    [ObservableProperty]
    private string tenantId = string.Empty;

    [ObservableProperty]
    private string clientId = string.Empty;

    [ObservableProperty]
    private bool isSaved;

    [ObservableProperty]
    private string? saveMessage;

    public SettingsViewModel(ISettingsService settingsService)
    {
        _settingsService = settingsService;
        TenantId = _settingsService.TenantId;
        ClientId = _settingsService.ClientId;
    }

    [RelayCommand]
    private void SaveSettings()
    {
        _settingsService.TenantId = TenantId;
        _settingsService.ClientId = ClientId;
        _settingsService.Save();
        IsSaved = true;
        SaveMessage = "Settings saved successfully.";
    }

    [RelayCommand]
    private void ResetToDefaults()
    {
        TenantId = "common";
        ClientId = string.Empty;
        IsSaved = false;
        SaveMessage = null;
    }

    partial void OnTenantIdChanged(string value)
    {
        IsSaved = false;
        SaveMessage = null;
    }

    partial void OnClientIdChanged(string value)
    {
        IsSaved = false;
        SaveMessage = null;
    }
}
