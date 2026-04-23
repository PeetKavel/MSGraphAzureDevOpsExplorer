using System.Text.Json;

namespace MSGraphAzureDevOpsExplorer.Services;

public class SettingsService : ISettingsService
{
    private const string DefaultTenantId = "common";
    private const string DefaultClientId = "";

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

    public string? Save()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SettingsFilePath)!);
            var data = new SettingsData { TenantId = _tenantId, ClientId = _clientId };
            var json = JsonSerializer.Serialize(data, SettingsJsonContext.Default.SettingsData);
            File.WriteAllText(SettingsFilePath, json);
            return null;
        }
        catch (Exception ex)
        {
            return $"Could not save settings: {ex.Message}";
        }
    }

    public string? Load()
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
                    _clientId = data.ClientId ?? DefaultClientId;
                }
            }
            return null;
        }
        catch (Exception ex)
        {
            _tenantId = DefaultTenantId;
            _clientId = DefaultClientId;
            return $"Could not load settings: {ex.Message}";
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
