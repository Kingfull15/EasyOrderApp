# EasyOrderApp

This app now uses Entity Framework Core + SQLite for persistent data.

## What is persisted

- Menu items (`MenuItems` table)
- Submitted table orders (`Orders` table)
- Order line items (`OrderItems` table)

Database file: `restaurant.db` (project root)

## Run locally

```powershell
dotnet restore
dotnet ef database update
dotnet run
```

## Add new DB-backed features later

1. Add a new model in `Models/`.
2. Add `DbSet<YourModel>` to `Data/AppDbContext.cs`.
3. Add relationship/configuration in `OnModelCreating` if needed.
4. Create a migration:

```powershell
dotnet ef migrations add AddYourFeature
```

5. Apply it:

```powershell
dotnet ef database update
```

## Notes

- `MenuController` and `DashboardController` now use `IDataService` via dependency injection.
- `Program.cs` applies pending migrations automatically at startup.

