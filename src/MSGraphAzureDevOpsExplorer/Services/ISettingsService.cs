namespace MSGraphAzureDevOpsExplorer.Services;

public interface ISettingsService
{
    string TenantId { get; set; }
    string ClientId { get; set; }
    event EventHandler<string>? TenantIdChanged;
    void Save();
    void Load();
}
