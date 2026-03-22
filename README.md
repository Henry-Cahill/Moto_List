# Moto_List

Stay organized and ensure you have everything you need for your motocross adventure by keeping a detailed checklist of all the essential items.

## Architecture (Three-Tier)

```
┌─────────────────┐     ┌─────────────────┐     ┌─────────────────┐
│  Presentation   │     │   Business/API   │     │    Database     │
│  (Razor Pages)  │────▶│  (ASP.NET Core)  │────▶│  (SQL Server)   │
│  :7200          │     │  :7100           │     │  192.168.1.202  │
└─────────────────┘     └─────────────────┘     └─────────────────┘
```

| Project | Description |
|---------|-------------|
| `src/Moto_List.Shared` | Shared DTOs used by API and Web |
| `src/Moto_List.API` | ASP.NET Core Web API with JWT auth, EF Core |
| `src/Moto_List.Web` | Razor Pages frontend |

## Getting Started

### Prerequisites
- .NET 10 SDK
- SQL Server (accessible at `192.168.1.202:1433`)

### Run the API (Terminal 1)
```bash
dotnet run --project src/Moto_List.API
```
API runs at `https://localhost:7100` — Swagger UI at `/swagger`

### Run the Web App (Terminal 2)
```bash
dotnet run --project src/Moto_List.Web
```
Web app runs at `https://localhost:7200`

### Database
The database is auto-migrated on first API startup (Development mode).
To manually apply migrations:
```bash
dotnet ef database update --project src/Moto_List.API
```
