# Betrieb-App API — ASP.NET Core Backend

Clean Architecture Backend für die Betrieb-App, gebaut mit ASP.NET Core 8 und Entity Framework Core.

## Voraussetzungen

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/products/docker-desktop)

## Setup

1. Datenbank und Redis starten:
   ```bash
   docker-compose up -d
   ```

2. API starten:
   ```bash
   dotnet run --project src/Betrieb.API
   ```

## Swagger

Die API-Dokumentation ist unter [http://localhost:5000/swagger](http://localhost:5000/swagger) erreichbar.

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
