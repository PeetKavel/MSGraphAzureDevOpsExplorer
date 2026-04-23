# MSGraphAzureDevOpsExplorer

A WinUI3 desktop application that queries Microsoft Graph API on behalf of the signed-in user to explore Entra ID (Azure Active Directory) security groups related to Azure DevOps.

## Features

The application is organized into tabs, with each tab representing a feature:

### Security Groups Tab
- Lists all Entra ID security groups whose names start with **AP_AzDO** or **AP_VSTS**
- Real-time search/filter by group name or description
- Live status bar showing loaded/filtered group counts
- Refresh button to reload data from Microsoft Graph

### Settings Tab
- Configure the **Entra ID Tenant ID** (e.g. `contoso.onmicrosoft.com` or `common`)
- Configure the **Azure App Registration Client ID**
- Settings are persisted locally between sessions

### Authentication (header)
- Sign in with a Microsoft account (delegated permissions)
- **Switch User** – sign in as a different user without restarting
- **Sign Out** – removes cached tokens

## Prerequisites

1. **.NET 8 SDK** with Windows App SDK workload
2. A registered **Azure App Registration** in your Entra ID tenant:
   - Supported account types: Single tenant or multi-tenant
   - Redirect URI (mobile & desktop): `ms-appx-web://microsoft.aad.brokerplugin/{ClientId}`
   - Delegated API permissions:
     - `User.Read`
     - `GroupMember.Read.All`

## Setup

1. Clone the repository
2. Open `MSGraphAzureDevOpsExplorer.sln` in Visual Studio 2022 or later
3. Run the application
4. Navigate to the **Settings** tab and enter:
   - Your **Tenant ID** (or `common` for multi-tenant)
   - Your **Client ID** from the Azure App Registration
5. Click **Save Settings**
6. Click **Sign In** in the header
7. Navigate to the **Security Groups** tab and click **Refresh**

## Project Structure

```
src/MSGraphAzureDevOpsExplorer/
├── App.xaml / App.xaml.cs              # Application entry point & DI setup
├── MainWindow.xaml / .cs               # Main window with tab view & auth header
├── Features/
│   ├── SecurityGroups/                 # Security Groups feature tab
│   │   ├── SecurityGroupsPage.xaml     # UI for listing and filtering groups
│   │   └── SecurityGroupsViewModel.cs  # Logic and state for security groups
│   └── Settings/                       # Settings feature tab
│       ├── SettingsPage.xaml           # UI for configuring tenant & client ID
│       └── SettingsViewModel.cs        # Logic and state for settings
├── Services/
│   ├── AuthenticationService.cs        # MSAL-based authentication (sign in/out/switch)
│   ├── GraphService.cs                 # Microsoft Graph API calls
│   └── SettingsService.cs              # Persists application settings
├── Models/
│   └── SecurityGroup.cs               # Security group data model
└── Converters/
    └── ValueConverters.cs             # XAML value converters
```

## Dependencies

| Package | Purpose |
|---------|---------|
| `Microsoft.WindowsAppSDK` | WinUI3 runtime |
| `Microsoft.Identity.Client` | MSAL – authentication against Entra ID |
| `Microsoft.Graph` | Microsoft Graph SDK – querying groups |
| `CommunityToolkit.Mvvm` | MVVM source generators & helpers |
| `Microsoft.Extensions.DependencyInjection` | Dependency injection container |
