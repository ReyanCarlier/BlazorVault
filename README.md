# BlazorVault
### A strong password manager, linked to Microsoft Azure.

Implementation as part of a 6-month cybersecurity internship, 1 1/2 months of this internship were dedicated to BlazorVault development in solo.
BlazorVault is written in C#/ASP.NET & Blazor Server, and is accessible only if you have a Microsoft Azure account in the organization.
This repository has been curated of every confidential informations, thus, it might need you to do some configurations to make it work.

A database made with SQLite is used by BlazorVault to store users and their accounts.
An empty database is also provided and distributed here.

## Requirements

- .NET8 SDK
- Don't remove the footer and its content from `MainLayout.razor`

## Configuration

### BlazorVault.csproj
- Modify the tag `<UserSecretsId>` with your own.

### Modify appsettings.json

```
"AzureAd": {
    "Instance":     "https://login.microsoftonline.com/",
    "Domain"  :     "",     <--- Add your Azure AD domain
    "TenantId":     "",     <--- Add your Azure AD tenant ID
    "ClientId":     "",     <--- Add your Azure AD client ID
    "ClientSecret": "",     <--- Add your Azure AD client secret
    "CallbackPath": "/signin-oidc"
},
[...]
"ConnectionStrings": {
    "SQLite": "Data Source=Data/Database.db"   <--- Path to SQLite Database (default)
}
```

### Modify Program.cs
In Program.cs, a static variable named `AdminPassword` is declared, which is used as a key to encrypt Master Passwords.
In order for BlazorVault to function properly, you must modify this variable with a very strong password (>= 126 characters) from the ASCII table (spaces & such excluded).

```
public static string AdminPassword { get; private set; } = "<Put your very strong password/key here>";
```
