# Library System Work Report

## Summary

The solution was stabilized and extended with a logging microservice based on the logging projects from `Amr-Namora/inrolment`.

The solution now contains these projects:

- `LibrarySystem.Domain`
- `LibrarySystem.Application`
- `LibrarySystem.DataAccess`
- `LibrarySystem.API`
- `Logging.Domain`
- `Logging.Application`
- `Logging.Infrastructure`
- `Logging.API`

## Build Fixes

The local machine has .NET 9 reference/runtime packs installed, but not .NET 8. The projects previously targeted `net8.0`, which caused build and restore confusion. All projects were retargeted to `net9.0`.

Updated projects:

- `LibrarySystem.Domain`
- `LibrarySystem.Application`
- `LibrarySystem.DataAccess`
- `LibrarySystem.API`
- `Logging.Domain`
- `Logging.Application`
- `Logging.Infrastructure`
- `Logging.API`

The solution now builds successfully:

```powershell
dotnet build LibrarySystem.sln -v:minimal --no-restore
```

## Package Fixes

`AutoMapper` 12/13 and older Microsoft 8.x dependencies were reported as vulnerable by NuGet. The package references were upgraded and normalized for the current `net9.0` solution.

Key package changes:

- `AutoMapper` upgraded to `16.1.1`.
- `Microsoft.AspNetCore.Authentication.JwtBearer` upgraded to `9.0.15`.
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore` upgraded to `9.0.15`.
- `Microsoft.EntityFrameworkCore.*` packages upgraded to `9.0.15`.
- `Serilog.AspNetCore` upgraded to `9.0.0`.
- `Swashbuckle.AspNetCore` upgraded to `10.2.1`.

Vulnerability check now passes:

```powershell
dotnet list LibrarySystem.sln package --vulnerable --include-transitive
```

## Soft Delete Fixes

`ApplicationDbContext` now applies soft-delete filtering to all models implementing `ISoftDelete`, not only `BasePersistenceModel`.

`TemplatePropertyModel` now implements `ISoftDelete` and has:

- `IsDeleted`
- `DeletedAt`

A migration was added:

- `20260609071000_AddSoftDeleteToTemplateProperties`

`CustomIdentityDbContext` now applies audit timestamps to auditable Identity entities.

## Logging Microservice

The logging microservice was imported from the referenced GitHub repo and adapted to this solution.

Added projects:

- `Logging.Domain`
- `Logging.Application`
- `Logging.Infrastructure`
- `Logging.API`

The microservice stores log records in its own database:

```json
"ConnectionStrings": {
  "Default": "Server=localhost;Database=LibrarySystem_LoggingDb;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;"
}
```

An initial logging migration was added:

- `20260609070000_InitialLoggingCreate`

`Logging.API` applies its migrations on startup.

## Main API Logging Integration

The main API now sends request logs to the logging microservice.

Added:

- `LibrarySystem.Application/Interfaces/ILoggingClient.cs`
- `LibrarySystem.DataAccess/Http/LoggingClient.cs`
- `LibrarySystem.API/Middleware/RequestLoggingMiddleware.cs`

Configured in `LibrarySystem.API/appsettings.json`:

```json
"LoggingService": {
  "BaseUrl": "http://localhost:5113"
}
```

Registered in `AddInfrastructureServices`:

```csharp
services.AddHttpClient<ILoggingClient, LoggingClient>(client =>
{
    var baseUrl = configuration["LoggingService:BaseUrl"] ?? "http://localhost:5113";
    client.BaseAddress = new Uri(baseUrl);
});
```

The middleware logs:

- HTTP method
- path
- response status code
- elapsed time
- authenticated username, or `anonymous`

If the logging microservice is down, the main API does not fail; it writes a warning to local application logs.

## Next Phase Work Completed

The next phase focused on API hardening, persistence performance, and deployment scaffolding.

### Repository Query Optimization

`MappedRepository.FindAsync()` no longer defaults to loading full tables before filtering. It now:

- Builds an EF `IQueryable`.
- Applies supported includes before querying.
- Translates simple domain predicates to persistence predicates by matching property names.
- Runs translated predicates in SQL.
- Falls back to in-memory filtering only when a predicate cannot be translated.
- Uses `AsNoTracking()` for read operations.

This directly fixes the earlier scalability issue for common calls such as:

- `i => i.Id == request.Id`
- `m => m.ItemId == request.ItemId`
- `p => p.VocabularyId == request.VocabularyId`
- `tp => tp.TemplateId == request.TemplateId`

### Persistence Relationship Fixes

Persistence models now include navigation properties required by application queries:

- `ResourceModel.Values`
- `VocabularyModel.Properties`
- `ItemModel.Template`
- `ItemModel.Medias`
- `MediaModel.Item`
- media metadata fields: `MimeType`, `FileSize`, `AltText`

`ApplicationDbContext` now explicitly configures these relationships:

- `PropertyModel` to `VocabularyModel`
- `ItemModel` to `ResourceTemplateModel`
- `MediaModel` to `ItemModel`
- `ValueModel` to `ResourceModel`
- `ValueModel` to `PropertyModel`

Migration added:

- `20260609072000_AddRepositoryNavigationMappings`

### API Hardening

Added:

- `LibrarySystem.API/Middleware/ExceptionHandlingMiddleware.cs`

`Program.cs` now registers a fallback authorization policy. Controller endpoints require authentication by default unless explicitly marked `[AllowAnonymous]`.

`AuthController` now explicitly allows anonymous access for:

- `POST /api/auth/register`
- `POST /api/auth/login`
- `POST /api/auth/login-google`

`POST /api/auth/logout` remains protected.

### Startup Migration Fix

The API failed during role seeding when the Identity database did not yet contain the audit columns expected by `AppRoleModel`.

Fixes added:

- `20260609041905_AddAuditColumnsToIdentityRoles`
- Startup migration for `ApplicationDbContext`
- Startup migration for `CustomIdentityDbContext`
- Connection string fallback from the corrected `IdentityConnection` key to the existing `IdentifyConnection` key

`Program.cs` now applies database migrations before calling `RoleSeeder.SeedRolesAsync()`.

### CI And Container Setup

Added:

- `.github/workflows/backend-ci.yml`
- `docker-compose.yml`
- `Logging.API/Dockerfile`

Updated:

- `LibrarySystem.API/Dockerfile`

The Dockerfiles now target .NET 9 and copy referenced project files before restore.

`docker-compose.yml` defines:

- SQL Server
- `Logging.API`
- `LibrarySystem.API`

Docker validation could not be run on this machine because the `docker` command is not installed.

## Remaining Technical Debt

- Persistence models and domain entities are duplicated, which increases mapping and repository complexity.
- `SystemUser.Roles` belongs to the Identity database, but `SystemUserModel` in the application database does not model that relationship yet.
- Some filenames contain typos or trailing spaces:
  - `MdeiaModel.cs`
  - `UpdateMediaComman.cs`
  - `UnitOfWork .cs`
  - `IUnitOfWork .cs`
- Unit/integration tests are still missing.
- Logging microservice currently uses HTTP fire-and-forget style from the API middleware. For high traffic, a message queue would be better.
