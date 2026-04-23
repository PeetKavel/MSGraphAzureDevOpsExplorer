using System.Text.Json;

namespace MSGraphAzureDevOpsExplorer.Services;

public class SettingsService : ISettingsService
{
    private const string DefaultTenantId = "common";
    private const string DefaultClientId = "YOUR_CLIENT_ID_HERE";

    private static readonly string SettingsFilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "MSGraphAzureDevOpsExplorer",
        "settings.json");

    public event EventHandler<string>? TenantIdChanged;

    private string _tenantId = DefaultTenantId;
    private string _clientId = DefaultClientId;

    public string TenantId
    {
        get => _tenantId;
        set
        {
            if (_tenantId != value)
            {
                _tenantId = value;
                TenantIdChanged?.Invoke(this, value);
            }
        }
    }

    public string ClientId
    {
        get => _clientId;
        set => _clientId = value;
    }

    public SettingsService()
    {
        Load();
    }

    public void Save()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SettingsFilePath)!);
            var data = new SettingsData { TenantId = _tenantId, ClientId = _clientId };
            var json = JsonSerializer.Serialize(data, SettingsJsonContext.Default.SettingsData);
            File.WriteAllText(SettingsFilePath, json);
        }
        catch
        {
            // Silently ignore save failures
        }
    }

    public void Load()
    {
        try
        {
            if (File.Exists(SettingsFilePath))
            {
                var json = File.ReadAllText(SettingsFilePath);
                var data = JsonSerializer.Deserialize(json, SettingsJsonContext.Default.SettingsData);
                if (data != null)
                {
                    _tenantId = string.IsNullOrWhiteSpace(data.TenantId) ? DefaultTenantId : data.TenantId;
                    _clientId = string.IsNullOrWhiteSpace(data.ClientId) ? DefaultClientId : data.ClientId;
                }
            }
        }
        catch
        {
            _tenantId = DefaultTenantId;
            _clientId = DefaultClientId;
        }
    }

    private sealed class SettingsData
    {
        public string TenantId { get; set; } = DefaultTenantId;
        public string ClientId { get; set; } = DefaultClientId;
    }

    [System.Text.Json.Serialization.JsonSerializable(typeof(SettingsData))]
    private partial class SettingsJsonContext : JsonSerializerContext { }
}
