# Betrieb-App API — ASP.NET Core Backend

Clean Architecture Backend für die Betrieb-App, gebaut mit ASP.NET Core 8 und Entity Framework Core.

## Voraussetzungen

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
  - Auf Apple Silicon (M1/M2/M3/M4/M5): den **arm64**-Installer wählen, nicht x64
  - Prüfen mit `dotnet --version` (sollte `8.x.x` ausgeben)
  - Falls `dotnet` danach nicht gefunden wird, das SDK liegt i. d. R. unter `/usr/local/share/dotnet` — zum `PATH` hinzufügen (z. B. in `~/.zshrc`):
    ```bash
    export DOTNET_ROOT=/usr/local/share/dotnet
    export PATH="$DOTNET_ROOT:$PATH"
    ```
- [Docker Desktop](https://www.docker.com/products/docker-desktop) — startet PostgreSQL & Redis als Container (siehe `docker-compose.yml`)

## Setup

1. Datenbank und Redis starten:
   ```bash
   docker-compose up -d
   ```

2. API starten:
   ```bash
   dotnet run --project src/Betrieb.API
   ```

   Die API lauscht dann auf **`http://localhost:5228`** (siehe `src/Betrieb.API/Properties/launchSettings.json` — der `DefaultConnection`-Eintrag in `appsettings.json` nennt zwar Port 5000, das ist aber nur die Datenbankverbindung, nicht der API-Port).

   Beim ersten Start wird die Datenbank automatisch migriert und mit Demo-Daten befüllt (`DbSeeder.cs`). Alle Demo-Konten verwenden das Passwort `Demo123!`, z. B.:

   - `max.mueller@firma.de` / `Demo123!` (Admin)
   - `a.schmidt@firma.de` / `Demo123!` (Manager)
   - `t.wagner@firma.de` / `Demo123!` (Mitarbeiter)

## Swagger

Die API-Dokumentation ist unter [http://localhost:5228/swagger](http://localhost:5228/swagger) erreichbar.

## Projektstruktur

```
betrieb-api/
├── src/
│   ├── Betrieb.API/           # ASP.NET Core Web API (Controller, Program.cs)
│   ├── Betrieb.Application/   # Business Logic, MediatR Handlers, FluentValidation
│   ├── Betrieb.Domain/        # Entities, Domain Interfaces
│   └── Betrieb.Infrastructure/ # EF Core DbContext, Repositories, PostgreSQL
└── tests/
    ├── Betrieb.UnitTests/
    └── Betrieb.IntegrationTests/
```

## Architektur

Clean Architecture mit folgenden Abhängigkeiten:

- **API** → Application, Infrastructure
- **Application** → Domain
- **Infrastructure** → Application, Domain
