# Logging Microservice

## Purpose

`Logging.API` is a separate microservice responsible for storing operational logs sent by the main `LibrarySystem.API`.

It was adapted from the logging projects in:

```text
https://github.com/Amr-Namora/inrolment.git
```

## Projects

```text
Logging.Domain
Logging.Application
Logging.Infrastructure
Logging.API
```

## Data Model

The logging database contains one main entity:

```csharp
public class Log
{
    public int Id { get; set; }
    public string? Message { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
```

## Endpoints

Base route:

```text
/api/logs
```

Endpoints:

```text
POST /api/logs
GET  /api/logs
GET  /api/logs/test
```

Example POST body:

```json
{
  "message": "GET /api/vocabularies -> 200 in 14ms",
  "createdBy": "anonymous"
}
```

## Running Locally

Start the logging service first:

```powershell
dotnet run --project Logging.API\Logging.API.csproj --launch-profile http
```

Default HTTP URL:

```text
http://localhost:5113
```

Then start the main API:

```powershell
dotnet run --project LibrarySystem.API\LibrarySystem.API.csproj --launch-profile http
```

The main API sends request logs to:

```text
http://localhost:5113/api/logs
```

## Running With Docker Compose

The solution now includes a root `docker-compose.yml` with:

- SQL Server
- `Logging.API`
- `LibrarySystem.API`

Command:

```powershell
docker compose up --build
```

Expected URLs:

```text
LibrarySystem.API: http://localhost:5112
Logging.API:       http://localhost:5113
```

This could not be validated in the current environment because Docker is not installed.

## Database

The logging service uses:

```text
LibrarySystem_LoggingDb
```

The service applies migrations during startup:

```csharp
await dbContext.Database.MigrateAsync();
```

This is practical for local development. For production, migration execution should usually be moved into a deployment step.

## Operational Notes

- If `Logging.API` is unavailable, `LibrarySystem.API` continues working.
- Failed log delivery is written as a warning through `ILogger`.
- The current integration uses HTTP. A queue-based integration is recommended later for durability.
- `Logging.API` has its own Dockerfile and can be deployed separately from the main API.
