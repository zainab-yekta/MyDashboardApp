# MyDashboardApp

A small sales dashboard built with ASP.NET Core 8. Admins add monthly sales, and every open dashboard updates instantly through SignalR, with no page refresh.

> **Note:** I first built this project in January 2025 as a technical interview task for a company, then cleaned it up and finished it for my portfolio.

![Admin adds a sale on the left, the user's dashboard updates on the right](mockup/preview.gif)

## What it does

- Sign in with ASP.NET Core Identity. There are two roles, Admin and User.
- The dashboard shows total sales, the best month, the monthly average, a line chart and the five latest entries.
- Only admins see the Add Sale form. The server checks the role as well, so a normal user can't post data even by calling the endpoint directly.
- A new sale is pushed to all connected browsers over SignalR, and the chart and cards update in place.

## Tech stack

- C# and JavaScript
- ASP.NET Core 8 MVC, with Razor Pages for the Identity UI
- Entity Framework Core 8 with SQL Server, using code first migrations and seeding on startup
- ASP.NET Core Identity with role based authorization
- SignalR for real time updates
- A hosted background service for the daily demo data reset
- Chart.js and Bootstrap 5 on the front end
- GitHub Actions for continuous integration

## Screenshots

| Admin view | User view |
| --- | --- |
| ![Admin dashboard](mockup/dashboard-admin.png) | ![User dashboard](mockup/dashboard-user.png) |

| Login | Mobile |
| --- | --- |
| ![Login page with demo accounts](mockup/login.png) | ![Dashboard on a phone](mockup/dashboard-mobile.png) |

## Getting started

You need the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) and SQL Server. LocalDB, which comes with Visual Studio, is enough.

```bash
git clone https://github.com/zainab-yekta/MyDashboardApp.git
cd MyDashboardApp
dotnet run
```

Open http://localhost:5048. On the first run the app creates the database, applies the migration and adds a year of sample sales, so there is no manual setup.

The default connection string points to LocalDB. To use a full SQL Server instance, change `DefaultConnection` in `appsettings.json`, for example:

```
Server=localhost;Database=MyDashboardApp;Trusted_Connection=True;TrustServerCertificate=True
```

### Accounts

A read-only account is created on startup, so anyone can look around:

| Email | Password |
| --- | --- |
| user@demo.com | User@123 |

The admin account is not public. To try the admin view locally, create an `appsettings.Local.json` file next to `appsettings.json` (it is ignored by git) and restart the app:

```json
{
  "DemoAccounts": {
    "AdminEmail": "admin@demo.com",
    "AdminPassword": "Choose-a-strong-password1"
  }
}
```

Then sign in as admin in one browser and as the read-only user in a private window, and add a sale to see the live update.

## Project layout

```
Controllers/HomeController.cs   dashboard page and the AddSale endpoint
Data/                           DbContext, startup seeding and the daily demo reset
Hubs/ChartHub.cs                SignalR hub the dashboards listen to
Models/                         SalesData entity and the dashboard view model
Views/Home/Index.cshtml         the dashboard
wwwroot/js/dashboard.js         chart, SignalR client and the add sale form
Areas/Identity/                 customized login page
mockup/                         screenshots and the preview GIF
```

## Deployment

The GitHub Actions workflow builds every push. It can also publish to Azure App Service with an Azure SQL database:

1. Create the web app (.NET 8) and the database in Azure.
2. In the web app's configuration, add the connection string `DefaultConnection` and these app settings: `DemoAccounts__AdminEmail`, `DemoAccounts__AdminPassword` and `DemoData__ResetDaily` set to `true`.
3. In the GitHub repository settings, add the variable `AZURE_WEBAPP_NAME` and the secret `AZURE_WEBAPP_PUBLISH_PROFILE` (downloaded from the web app's overview page).

After that, each push to `main` is deployed. The migration runs on startup, and with `ResetDaily` on, the sample data is restored when the app starts and every 24 hours after that.

## Notes

- Every POST is protected with an antiforgery token through a global filter.
- Failed logins count towards account lockout.
- Chart data is written into the page with `Json.Serialize`, which escapes HTML characters.
