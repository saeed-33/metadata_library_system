# Next Phase Plan For 4 Developers

## Goal

Stabilize the backend as a production-ready API system with reliable persistence, security, tests, and observability.

## Status After Implementation

The next phase was partially implemented in the current branch:

- CI workflow added for restore, build, and vulnerable package scan.
- Dockerfiles and Docker Compose added for the main API, logging API, and SQL Server.
- Microsoft package versions normalized to .NET 9 servicing packages.
- `MappedRepository.FindAsync()` optimized to push common predicates to EF/database queries.
- Missing persistence navigation properties and relationships added.
- Main API exception handling middleware added.
- Default authentication requirement added through a fallback authorization policy.
- Auth endpoints that must remain public are marked with `[AllowAnonymous]`.
- Main solution build passes.
- Vulnerability scan passes with transitive packages included.

Docker Compose validation is still pending because Docker is not installed on this machine.

## Developer 1: Build, Infrastructure, DevOps

Responsibilities:

- Keep the solution building cleanly.
- Add CI pipeline for restore, build, test, and vulnerability checks.
- Add Docker Compose for:
  - `LibrarySystem.API`
  - `Logging.API`
  - SQL Server
- Normalize package versions.
- Add environment-specific configuration.
- Document local setup.

Deliverables:

- CI build passes.
- Docker Compose starts all required services.
- `dotnet list package --vulnerable` is clean.

## Developer 2: DataAccess And Persistence

Responsibilities:

- Replace in-memory repository filtering with database-side queries.
- Review all EF mappings and relationships.
- Add migrations for current model state.
- Fix file names with typos/trailing spaces.
- Decide whether to keep separate persistence models or simplify to EF-mapped domain entities.

Delivered:

- Repository queries scale correctly.
- Migrations match the current model.

Still pending:

- DataAccess naming is clean.

## Developer 3: Application Layer

Responsibilities:

- Review all commands and queries.
- Ensure handlers return consistent results.
- Ensure validators are registered and tested.
- Add unit tests for CQRS handlers.
- Review AutoMapper profiles for missing or risky mappings.

Still pending:

- Main command/query flows are tested.
- Validation failures are predictable.
- Mapping configuration is verified.

## Developer 4: API, Security, Integration Testing

Responsibilities:

- Add authorization policies and `[Authorize]` attributes.
- Standardize API error responses.
- Add integration tests for core endpoints.
- Test JWT auth and Google login flows.
- Verify request logging to `Logging.API`.

Delivered:

- Protected endpoints are secured.
- Swagger is available in development.

Still pending:

- Swagger is accurate.
- Integration tests cover core user flows.

## Suggested Sprint Order

1. Add unit tests for application handlers and mapping profiles.
2. Add integration tests for auth, protected endpoints, and request logging.
3. Clean file names with typos/trailing spaces.
4. Model user roles consistently between Identity and application user responses.
5. Replace direct HTTP logging with a durable queue when traffic grows.
