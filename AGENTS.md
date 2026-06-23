# Agent Guide — Metadata Library System

This document is written for AI coding agents that need to understand, build, modify, or debug this project. It reflects the actual state of the repository as of the last exploration.

## 1. Project Overview

This is a backend system for managing a library's catalog, circulation, patrons, media, and administrative settings. It is implemented in C# / ASP.NET Core and follows a layered architecture inspired by Clean Architecture and CQRS.

The repository contains **two separate APIs**:

1. **LibrarySystem.API** — the main business API.
2. **Logging.API** — a small satellite microservice that stores operational request logs sent by the main API.

The project was created by HIAST students and is licensed under the MIT License (see `LICENSE`).

## 2. Technology Stack

- **Runtime / SDK:** .NET 8 (`net8.0`) — pinned by `global.json` to SDK `8.0.422`.
  - ⚠️ Note: Some documentation files mention a migration to .NET 9, but the actual `.csproj` files target `net8.0` and the build outputs `net8.0` assemblies. The CI workflow and Dockerfiles currently reference .NET 9 images / SDK, which is a known mismatch.
- **Web framework:** ASP.NET Core 8.
- **Database:** Microsoft SQL Server.
- **ORM:** Entity Framework Core 8 (`Microsoft.EntityFrameworkCore.*` 8.0.12).
- **Identity:** ASP.NET Core Identity with EF Core stores.
- **Authentication:** JWT Bearer tokens + optional Google OAuth login.
- **CQRS / Mediator:** MediatR 12.4.1.
- **Validation:** FluentValidation 11.11.0 with a MediatR pipeline behavior.
- **Object mapping:** AutoMapper 13.0.1.
- **Logging:** Serilog (console + configuration-driven) in the main API; custom `Logging.API` microservice for request logs.
- **Documentation:** Swagger / OpenAPI via Swashbuckle.AspNetCore 6.6.2.
- **Image processing:** SixLabors.ImageSharp 3.1.5.
- **Containerization:** Docker + Docker Compose.
- **CI:** GitHub Actions (`.github/workflows/backend-ci.yml`).

## 3. Solution Structure

Solution file: `LibrarySystem.sln`

```text
LibrarySystem.Domain          # Domain entities, enums, base contracts (no external deps)
LibrarySystem.Application     # CQRS commands/queries, DTOs, validators, mapping profiles, interfaces
LibrarySystem.DataAccess      # EF contexts, migrations, repositories, Identity services, HTTP clients
LibrarySystem.API             # ASP.NET Core host, controllers, middleware

Logging.Domain                # Log entity contracts
Logging.Application           # Log DTOs and service interfaces
Logging.Infrastructure        # EF context, repository, log service implementation
Logging.API                   # ASP.NET Core host for the logging microservice
```

Dependency direction (should be respected):

```text
API → Application → Domain
API → DataAccess → Application → Domain

Logging.API → Logging.Application → Logging.Domain
Logging.API → Logging.Infrastructure → Logging.Application
```

## 4. Build, Restore, and Run

### Prerequisites

- .NET 8 SDK (`8.0.422` recommended because of `global.json`).
- SQL Server (local, remote, or the Docker Compose SQL Server container).
- (Optional) Docker / Docker Compose.

### Common Commands

Restore and build the whole solution:

```bash
dotnet restore LibrarySystem.sln
dotnet build LibrarySystem.sln --configuration Release --no-restore
```

Run the main API locally:

```bash
dotnet run --project LibrarySystem.API/LibrarySystem.API.csproj --launch-profile http
```

Default main API URL: `http://localhost:5022`  
Swagger (development only): `http://localhost:5022/swagger`

Run the logging API locally:

```bash
dotnet run --project Logging.API/Logging.API.csproj --launch-profile http
```

Default logging API URL: `http://localhost:5113`

Run everything with Docker Compose:

```bash
docker compose up --build
```

Exposed ports:

- SQL Server: `1433`
- LibrarySystem.API: `5112` → mapped to container port `8080`
- Logging.API: `5113` → mapped to container port `8080`

The default SQL Server `sa` password is configured through the `MSSQL_SA_PASSWORD` environment variable and falls back to `LibrarySystem@12345`.

### Known Build Warnings

The solution currently builds with warnings caused by vulnerable NuGet packages:

- `AutoMapper` 13.0.1 — high severity vulnerability.
- `SixLabors.ImageSharp` 3.1.5 — high and moderate severity vulnerabilities.

These should be reviewed and upgraded when possible.

## 5. Code Organization and Conventions

### Layer Responsibilities

| Layer | Responsibility |
|-------|----------------|
| `LibrarySystem.Domain` | Plain entities (`Item`, `Media`, `Patron`, `BorrowRecord`, …), enums, and marker interfaces (`ISoftDelete`, `IAuditable`). |
| `LibrarySystem.Application` | CQRS command/query records, handlers, DTOs, FluentValidation validators, AutoMapper profiles, and service interfaces. |
| `LibrarySystem.DataAccess` | EF Core `DbContext` implementations, migrations, repository implementations, Identity integration, HTTP clients, and service implementations. |
| `LibrarySystem.API` | Controllers, middleware, DI wiring, `Program.cs`, and host configuration. |

### CQRS Pattern

- Commands live in `LibrarySystem.Application/Commands/<Feature>/`.
- Queries live in `LibrarySystem.Application/Queries/<Feature>/`.
- Handlers are co-located with the command/query record they handle.
- Validators live in `LibrarySystem.Application/Validators/<Feature>/`.
- DTOs live in `LibrarySystem.Application/DTOs/<Feature>/`.
- Mapping profiles live in `LibrarySystem.Application/Mappings/` and `LibrarySystem.DataAccess/Mappings/`.

Example file layout for the `Items` feature:

```text
Commands/Items/CreateItemCommand.cs
Commands/Items/DeleteItemCommand.cs
Commands/Items/UpdateItemCommand.cs
Queries/Items/GetItemByIdQuery.cs
Queries/Items/GetAllItemsQuery.cs
Validators/Items/CreateItemCommandValidator.cs
Validators/Items/UpdateItemCommandValidator.cs
DTOs/Items/ItemResponse.cs
Mappings/ItemMappingProfile.cs
```

### Domain Model vs Persistence Model

The project currently maintains **two parallel object models**:

- Domain entities in `LibrarySystem.Domain/entities/` (e.g. `Item`).
- Persistence models in `LibrarySystem.DataAccess/Persistence/models/` (e.g. `ItemModel`).

`MappedRepository<TDomain, TPersistence>` maps between them using AutoMapper. This adds complexity and is called out as technical debt in the project documentation.

### Soft Delete and Auditing

- `ISoftDelete` provides `IsDeleted` and `DeletedAt`.
- `IAuditable` provides `CreatedAt`, `CreatedBy`, `ModifiedAt`, `ModifiedBy`.
- `ApplicationDbContext` and `CustomIdentityDbContext` automatically:
  - Apply a global query filter that hides soft-deleted rows.
  - Convert `Delete` operations into soft deletes.
  - Stamp audit timestamps on save.

Queries that need deleted rows use `IgnoreQueryFilters()` (e.g. `FindWithDeletedAsync`).

### Validation

FluentValidation validators are registered by assembly scan in `ApplicationServiceRegistration.cs`. Validation runs automatically through `ValidationBehavior<TRequest, TResponse>` before the command/query handler executes.

### Dependency Injection Registration

- `ApplicationServiceRegistration.AddApplicationServices()` registers AutoMapper, MediatR, FluentValidation, and the validation pipeline behavior.
- `InfrastructureServiceRegistration.AddInfrastructureServices()` registers EF Core contexts, Identity, repositories, unit of work, auth services, and the logging HTTP client.

## 6. Databases and Migrations

The main API uses **two SQL Server databases**:

1. **Application database** — configured with `ConnectionStrings:DefaultConnection` and used by `ApplicationDbContext`.
2. **Identity database** — configured with `ConnectionStrings:IdentityConnection` (with a fallback to the misspelled `IdentifyConnection` key for backward compatibility) and used by `CustomIdentityDbContext`.

The logging API uses one database configured with `ConnectionStrings:Default` and `LoggingDbContext`.

Migrations are applied automatically at startup (`MigrateAsync`). This is convenient for local development but should normally be moved to a deployment step for production.

### Adding Migrations

For the main application context:

```bash
dotnet ef migrations add <MigrationName> --project LibrarySystem.DataAccess --startup-project LibrarySystem.API --context ApplicationDbContext
```

For the Identity context:

```bash
dotnet ef migrations add <MigrationName> --project LibrarySystem.DataAccess --startup-project LibrarySystem.API --context CustomIdentityDbContext
```

For the logging context:

```bash
dotnet ef migrations add <MigrationName> --project Logging.Infrastructure --startup-project Logging.API --context LoggingDbContext
```

## 7. Testing

There are **no test projects** in the solution at this time. Adding unit tests for application handlers / validators / mappers and integration tests for controllers is an open work item.

If you add tests, follow the existing folder conventions and place them in a project such as `LibrarySystem.Tests` at the solution root.

## 8. Security Considerations

- **JWT configuration:** `Jwt:Key`, `Jwt:Issuer`, and `Jwt:Audience` are read from `appsettings.json`. The committed key is a placeholder and must be replaced with a strong secret in production (at least 32 bytes).
- **Fallback authorization:** `Program.cs` registers a fallback policy that requires an authenticated user for all endpoints by default. Public endpoints must be explicitly marked `[AllowAnonymous]`.
- **Role-based authorization:** Admin-only endpoints use `[Authorize(Roles = SystemRoles.Admin)]`.
- **Google OAuth:** Google client credentials are present in `appsettings.json`. These should be treated as secrets and rotated.
- **Connection strings:** Database passwords are visible in `appsettings.json` and `docker-compose.yml` defaults. Use user secrets, environment variables, or a secret manager in production.
- **CORS:** The API allows `http://localhost:5173` (the React Vite dev server). Adjust this for production.
- **Swagger:** Swagger UI is enabled only in the Development environment.
- **HTTPS redirection:** Enabled; ensure certificates are configured for non-development environments.

## 9. Deployment

### Docker

Each API has its own Dockerfile:

- `LibrarySystem.API/Dockerfile`
- `Logging.API/Dockerfile`

Both Dockerfiles currently use `mcr.microsoft.com/dotnet/aspnet:9.0` and `mcr.microsoft.com/dotnet/sdk:9.0` base images even though the projects target `net8.0`. This works because .NET 9 SDK can build .NET 8 projects, but it is inconsistent and should be aligned.

`docker-compose.yml` orchestrates SQL Server, `Logging.API`, and `LibrarySystem.API`. The main API depends on the logging API only for startup (`service_started`), so the main API remains available even if logging fails.

### CI / CD

`.github/workflows/backend-ci.yml` runs on pushes to `main`/`master` and on pull requests:

1. Checks out the code.
2. Sets up .NET 9.x SDK.
3. Restores packages.
4. Builds the solution in Release.
5. Runs `dotnet list` for a vulnerable package scan.

The workflow does not run tests because none exist yet.

## 10. Logging and Observability

- The main API uses Serilog for structured logging to the console.
- A custom `RequestLoggingMiddleware` captures method, path, status code, elapsed time, and authenticated user.
- Request logs are sent via HTTP to `Logging.API` (`LoggingService:BaseUrl`, default `http://localhost:5113`).
- If `Logging.API` is unavailable, the main API logs a warning locally and continues processing.
- For high-traffic scenarios, the documentation recommends replacing the HTTP fire-and-forget integration with a durable message queue.

## 11. Important Files and Gotchas

### Files to Know

- `LibrarySystem.sln` — solution file.
- `global.json` — pins .NET SDK version.
- `LibrarySystem.API/Program.cs` — main host setup, auth, CORS, middleware ordering, and startup seeding.
- `LibrarySystem.API/appsettings.json` — main configuration.
- `LibrarySystem.DataAccess/Persistence/contexts/ApplicationDbContext.cs` — main EF context.
- `LibrarySystem.DataAccess/Persistence/contexts/CustomIdentityDbContext.cs` — Identity EF context.
- `LibrarySystem.DataAccess/InfrastructureServiceRegistration.cs` — DI registration for infrastructure.
- `LibrarySystem.Application/ApplicationServiceRegistration.cs` — DI registration for application services.
- `docker-compose.yml` — local container orchestration.

### Known Naming / Structural Issues

A few files have typos or trailing spaces in their names. Be careful when referencing them:

- `LibrarySystem.DataAccess/Persistence/models/MdeiaModel.cs`
- `LibrarySystem.DataAccess/Persistence/models/BookmarkModel .cs`
- `LibrarySystem.Application/Commands/Media/UpdateMediaComman.cs`
- `LibrarySystem.DataAccess/UnitOfWork/UnitOfWork .cs`
- `LibrarySystem.Application/Interfaces/IUnitOfWork .cs`

### React Frontend

The backend CORS policy expects the frontend at `http://localhost:5173`, which is the default Vite dev server URL. The frontend project is not included in this repository.

### .NET Version Mismatch

The following .NET 9 references exist while the projects target .NET 8:

- Dockerfiles use `dotnet/aspnet:9.0` and `dotnet/sdk:9.0`.
- The GitHub Actions workflow installs `dotnet-version: 9.0.x`.
- Some documentation states the projects were upgraded to .NET 9.

The local build with the .NET 8 SDK succeeds, but the version alignment should be cleaned up.

## 12. Useful References

- Project documentation: `docs/`
  - `docs/LOGGING_MICROSERVICE.md`
  - `docs/WORK_REPORT.md`
  - `docs/NEXT_PHASE_PLAN_4_DEVELOPERS.md`
  - `docs/THIRD_PARTY_NOTICES.md`
- HTTP request files: `LibrarySystem.API/LibrarySystem.API.http`, `Logging.API/Logging.API.http`
- PDF report generator: `generate_changes_pdf.py` (Python + ReportLab, requires Arabic fonts on Windows)

## 13. Quick Checklist When Modifying Code

- Build the full solution: `dotnet build LibrarySystem.sln`.
- Check for vulnerable packages: `dotnet list LibrarySystem.sln package --vulnerable --include-transitive`.
- If you change entities or persistence models, add/update EF migrations.
- Add or update FluentValidation validators for new commands.
- Add or update AutoMapper profiles for new DTOs.
- Ensure controllers have the correct `[Authorize]` / `[AllowAnonymous]` attributes.
- Keep domain/persistence model mapping in sync if both models are affected.
- If you introduce new configuration, prefer environment variables or user secrets over committing secrets to `appsettings.json`.
