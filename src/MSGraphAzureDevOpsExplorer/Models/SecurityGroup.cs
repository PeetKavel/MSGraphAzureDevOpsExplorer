namespace MSGraphAzureDevOpsExplorer.Models;

public class SecurityGroup
{
    public string Id { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Mail { get; set; }
    public bool? SecurityEnabled { get; set; }
}
