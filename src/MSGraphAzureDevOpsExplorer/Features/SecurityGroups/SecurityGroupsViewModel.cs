using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MSGraphAzureDevOpsExplorer.Models;
using MSGraphAzureDevOpsExplorer.Services;
using System.Collections.ObjectModel;

namespace MSGraphAzureDevOpsExplorer.Features.SecurityGroups;

public partial class SecurityGroupsViewModel : ObservableObject
{
    private static readonly string[] GroupPrefixes = { "AP_AzDO", "AP_VSTS" };

    private readonly IGraphService _graphService;
    private readonly IAuthenticationService _authService;

    private List<SecurityGroup> _allGroups = new();

    [ObservableProperty]
    private ObservableCollection<SecurityGroup> groups = new();

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string? errorMessage;

    [ObservableProperty]
    private string statusMessage = "Please sign in to load security groups.";

    [ObservableProperty]
    private int totalGroupCount;

    [ObservableProperty]
    private int filteredGroupCount;

    public SecurityGroupsViewModel(IGraphService graphService, IAuthenticationService authService)
    {
        _graphService = graphService;
        _authService = authService;
        _authService.AuthenticationStateChanged += OnAuthenticationStateChanged;
    }

    private void OnAuthenticationStateChanged(object? sender, string? displayName)
    {
        if (!string.IsNullOrEmpty(displayName))
        {
            StatusMessage = $"Signed in as {displayName}. Click Refresh to load groups.";
        }
        else
        {
            StatusMessage = "Please sign in to load security groups.";
            _allGroups.Clear();
            Groups.Clear();
            TotalGroupCount = 0;
            FilteredGroupCount = 0;
        }
    }

    partial void OnSearchTextChanged(string value)
    {
        ApplyFilter();
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        if (!_authService.IsSignedIn)
        {
            StatusMessage = "Please sign in first.";
            return;
        }

        IsLoading = true;
        ErrorMessage = null;
        StatusMessage = "Loading security groups...";

        try
        {
            _allGroups = (await _graphService.GetSecurityGroupsAsync(GroupPrefixes)).ToList();
            TotalGroupCount = _allGroups.Count;
            ApplyFilter();
            StatusMessage = $"Loaded {TotalGroupCount} security groups.";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load groups: {ex.Message}";
            StatusMessage = "An error occurred while loading groups.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void ClearSearch()
    {
        SearchText = string.Empty;
    }

    private void ApplyFilter()
    {
        var filtered = string.IsNullOrWhiteSpace(SearchText)
            ? _allGroups
            : _allGroups.Where(g =>
                g.DisplayName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                (g.Description?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false))
              .ToList();

        Groups = new ObservableCollection<SecurityGroup>(filtered);
        FilteredGroupCount = filtered.Count;
    }
}
