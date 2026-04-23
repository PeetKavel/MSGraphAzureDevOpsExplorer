using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Kiota.Abstractions.Authentication;
using MSGraphAzureDevOpsExplorer.Models;
using SecurityGroup = MSGraphAzureDevOpsExplorer.Models.SecurityGroup;

namespace MSGraphAzureDevOpsExplorer.Services;

public class GraphService : IGraphService
{
    private readonly IAuthenticationService _authService;

    public GraphService(IAuthenticationService authService)
    {
        _authService = authService;
    }

    private GraphServiceClient CreateGraphClient()
    {
        var tokenProvider = new TokenProvider(_authService);
        var authProvider = new BaseBearerTokenAuthenticationProvider(tokenProvider);
        return new GraphServiceClient(authProvider);
    }

    public async Task<IReadOnlyList<SecurityGroup>> GetSecurityGroupsAsync(string[] prefixes)
    {
        var client = CreateGraphClient();
        var groups = new List<SecurityGroup>();

        foreach (var prefix in prefixes)
        {
            var page = await client.Groups.GetAsync(config =>
            {
                config.QueryParameters.Filter =
                    $"securityEnabled eq true and startsWith(displayName,'{EscapeODataString(prefix)}')";
                config.QueryParameters.Select = new[]
                {
                    "id", "displayName", "description", "mail", "securityEnabled"
                };
                config.QueryParameters.Top = 999;
                config.Headers.Add("ConsistencyLevel", "eventual");
                config.QueryParameters.Count = true;
            });

            if (page?.Value == null)
                continue;

            var pageIterator = PageIterator<Group, GroupCollectionResponse>
                .CreatePageIterator(client, page, g =>
                {
                    groups.Add(new SecurityGroup
                    {
                        Id = g.Id ?? string.Empty,
                        DisplayName = g.DisplayName ?? string.Empty,
                        Description = g.Description,
                        Mail = g.Mail,
                        SecurityEnabled = g.SecurityEnabled
                    });
                    return true;
                });

            await pageIterator.IterateAsync();
        }

        return groups
            .OrderBy(g => g.DisplayName)
            .ToList();
    }

    private static string EscapeODataString(string value) =>
        value.Replace("'", "''");

    private sealed class TokenProvider : IAccessTokenProvider
    {
        private readonly IAuthenticationService _authService;

        public TokenProvider(IAuthenticationService authService)
        {
            _authService = authService;
        }

        public AllowedHostsValidator AllowedHostsValidator { get; } =
            new AllowedHostsValidator(new[] { "graph.microsoft.com" });

        public async Task<string> GetAuthorizationTokenAsync(
            Uri uri,
            Dictionary<string, object>? additionalAuthenticationContext = null,
            CancellationToken cancellationToken = default)
        {
            return await _authService.AcquireTokenAsync() ?? string.Empty;
        }
    }
}
