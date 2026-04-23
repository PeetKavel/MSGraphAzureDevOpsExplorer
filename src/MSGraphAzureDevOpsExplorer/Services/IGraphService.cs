using MSGraphAzureDevOpsExplorer.Models;

namespace MSGraphAzureDevOpsExplorer.Services;

public interface IGraphService
{
    Task<IReadOnlyList<SecurityGroup>> GetSecurityGroupsAsync(string[] prefixes);
}
