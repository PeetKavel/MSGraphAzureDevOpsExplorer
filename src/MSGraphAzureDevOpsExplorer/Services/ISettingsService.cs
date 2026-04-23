namespace MSGraphAzureDevOpsExplorer.Services;

public interface ISettingsService
{
    string TenantId { get; set; }
    string ClientId { get; set; }
    event EventHandler<string>? TenantIdChanged;

    /// <summary>Persists settings to disk. Returns an error message on failure, or null on success.</summary>
    string? Save();

    /// <summary>Loads settings from disk. Returns an error message on failure, or null on success.</summary>
    string? Load();
}
