# Metadata Library System — Project Description

> Backend system for managing a library's catalog, circulation, patrons, media, and administrative settings.  
> Built with C# / ASP.NET Core, Entity Framework Core, and a Clean Architecture / CQRS approach.

---

## 1. Project Overview

The **Metadata Library System** is an ASP.NET Core Web API that supports both physical and digital library resources. It centers on a flexible metadata model: library items are described through **vocabularies**, **properties**, and **values** (similar to Dublin Core / RDF-style metadata). Each item is based on a **resource template** that defines which fields are required, their labels, and whether items of that type can be borrowed.

The repository contains **two APIs**:

| API | Purpose | Default Local URL |
|---|---|---|
| `LibrarySystem.API` | Main business API (catalog, circulation, users, settings) | `http://localhost:5022` |
| `Logging.API` | Satellite microservice that stores operational request logs | `http://localhost:5113` |

**Current target framework:** `net8.0` (pinned by `global.json` to SDK `8.0.422`).  
*Note: some CI/Docker/documentation references still mention .NET 9, but the project files target .NET 8.*

---

## 2. Architecture

### 2.1 Solution Projects

| Project | Responsibility |
|---|---|
| `LibrarySystem.Domain` | Domain entities, enums, and base contracts (`BaseEntity`, `ISoftDelete`, `IAuditable`). No external dependencies. |
| `LibrarySystem.Application` | CQRS commands/queries, handlers, DTOs, FluentValidation validators, AutoMapper profiles, and service interfaces. |
| `LibrarySystem.DataAccess` | EF Core contexts, migrations, persistence models, repositories, Unit of Work, Identity integration, and HTTP clients. |
| `LibrarySystem.API` | ASP.NET Core host, controllers, middleware, DI wiring, and `Program.cs`. |
| `Logging.Domain` / `Logging.Application` / `Logging.Infrastructure` / `Logging.API` | Logging microservice layers (entity contracts, DTOs, persistence, and host). |

### 2.2 Dependency Direction

```text
API → Application → Domain
API → DataAccess → Application → Domain

Logging.API → Logging.Application → Logging.Domain
Logging.API → Logging.Infrastructure → Logging.Application
```

### 2.3 Design Patterns

| Pattern | Where It Appears | Purpose |
|---|---|---|
| **Clean / Layered Architecture** | Solution structure (Domain → Application → DataAccess → API) | Enforces separation of concerns and dependency direction. |
| **CQRS (Command Query Responsibility Segregation)** | `Commands/*` and `Queries/*` folders in `LibrarySystem.Application` | Separates write operations (commands) from read operations (queries). |
| **Mediator Pattern** | MediatR registration in `ApplicationServiceRegistration` | Decouples controllers from handlers; routes commands/queries to handlers. |
| **Repository Pattern** | `IGenericRepository<T>` / `MappedRepository<TDomain, TPersistence>` | Abstracts data access behind a typed interface. |
| **Generic Repository Pattern** | `IGenericRepository<T>` with `GetByIdAsync`, `AddAsync`, `Update`, `Delete`, `GetAllAsync`, etc. | Provides reusable CRUD operations for any aggregate root. |
| **Mapped Repository Pattern** | `MappedRepository<TDomain, TPersistence>` and mapping profiles | Translates between domain entities and parallel persistence models. |
| **Unit of Work Pattern** | `IUnitOfWork` / `UnitOfWork` | Coordinates multiple repositories and commits changes in a single transaction. |
| **Pipeline Behavior Pattern** | `ValidationBehavior<TRequest, TResponse>` | Cross-cutting validation runs before any command/query handler. |
| **DTO (Data Transfer Object) Pattern** | `LibrarySystem.Application/DTOs/*` | Shapes data exposed by the API and decouples domain from contracts. |
| **Object-to-Object Mapping Pattern** | AutoMapper profiles in `Mappings/` | Automates mapping between domain, persistence, and DTO models. |
| **Fluent Validation Pattern** | `LibrarySystem.Application/Validators/*` | Encapsulates input validation rules per command/query. |
| **Soft Delete Pattern** | `ISoftDelete`, global EF query filters, overridden `SaveChanges` | Hides deleted rows instead of removing them from the database. |
| **Auditing Pattern** | `IAuditable`, `CreatedAt` / `ModifiedAt` stamping in `SaveChanges` | Tracks who/when created or modified an entity. |
| **Dependency Injection (DI)** | `ApplicationServiceRegistration`, `InfrastructureServiceRegistration`, `Program.cs` | Wires services, repositories, contexts, and behaviors at runtime. |
| **Middleware Pipeline Pattern** | `ExceptionHandlingMiddleware`, `RequestLoggingMiddleware` | Adds cross-cutting concerns (error handling, logging) to the HTTP pipeline. |
| **Global Query Filter Pattern** | `HasQueryFilter(e => !e.IsDeleted)` in `OnModelCreating` | Applies soft-delete filtering automatically to all queries. |
| **Database-per-Service / Satellite Microservice** | Separate `Logging.*` projects with own `LoggingDbContext` | Isolates operational logging into its own deployable service. |
| **Fire-and-Forget HTTP Client** | `ILoggingClient` / `LoggingClient` | Sends request logs to `Logging.API` without blocking the main request. |
| **Seeding Pattern** | `RoleSeeder`, `MetadataSeeder` | Populates default roles and development data at startup. |
| **Fallback Authorization Policy** | `Program.cs` `AddAuthorization` | Requires authentication by default; opt-out via `[AllowAnonymous]`. |
| **JWT Bearer Authentication** | `Program.cs` JWT configuration | Stateless token-based authentication for API clients. |
| **OAuth 2.0 / OpenID Connect (Google)** | `GoogleLogin` endpoint and `AuthService` | Allows login/registration via Google ID tokens. |
| **ASP.NET Core Identity Pattern** | `CustomIdentityDbContext`, `AppUserModel`, `AppRoleModel` | Manages users, roles, passwords, and tokens. |
| **Problem Details Pattern** | `ExceptionHandlingMiddleware` | Returns RFC 7808 `ProblemDetails` JSON for errors. |

### 2.4 Folder Structure by Layer

#### `LibrarySystem.Domain` (Domain Layer)

```text
LibrarySystem.Domain/
├── common/          # Shared contracts: BaseEntity, ISoftDelete, IAuditable
├── entities/        # Domain entities: Item, Media, Patron, BorrowRecord, etc.
└── enums/           # Domain enumerations: status codes, roles, etc.
```

#### `LibrarySystem.Application` (Application Layer)

```text
LibrarySystem.Application/
├── Behaviors/       # MediatR pipeline behaviors (e.g., ValidationBehavior)
├── Commands/        # CQRS write operations grouped by feature
│   ├── Bookmarks/
│   ├── Circulation/
│   ├── ItemCopies/
│   ├── ItemSets/
│   ├── Items/
│   ├── Media/
│   ├── Patrons/
│   ├── Properties/
│   ├── ResourceTemplates/
│   ├── Settings/
│   ├── Users/
│   └── Vocabularies/
├── DTOs/            # Data Transfer Objects grouped by feature
│   ├── Auth/
│   ├── Bookmarks/
│   ├── BorrowRecords/
│   ├── Circulation/
│   ├── ItemCopies/
│   ├── ItemSets/
│   ├── Items/
│   ├── Media/
│   ├── Patrons/
│   ├── Properties/
│   ├── ResourceTemplates/
│   ├── SystemSettings/
│   ├── Users/
│   ├── Values/
│   └── Vocabularies/
├── Interfaces/      # Abstractions: IUnitOfWork, IGenericRepository, IAuthService, etc.
├── Mappings/        # AutoMapper profiles (application-level)
├── Queries/         # CQRS read operations grouped by feature
│   ├── Bookmarks/
│   ├── Circulation/
│   ├── ItemCopies/
│   ├── ItemSets/
│   ├── Items/
│   ├── Media/
│   ├── Patrons/
│   ├── Properties/
│   ├── ResourceTemplates/
│   ├── Settings/
│   ├── Users/
│   └── Vocabularies/
└── Validators/      # FluentValidation validators grouped by feature
    ├── Circulation/
    ├── ItemCopies/
    ├── ItemSets/
    ├── Items/
    ├── Media/
    ├── Patrons/
    ├── Properties/
    ├── ResourceTemplates/
    ├── Users/
    └── Vocabularies/
```

#### `LibrarySystem.DataAccess` (Infrastructure Layer)

```text
LibrarySystem.DataAccess/
├── Http/            # HTTP clients (ILoggingClient / LoggingClient)
├── Mappings/        # Persistence mapping profiles (domain ↔ persistence models)
├── Migrations/      # EF Core migrations
│   └── CustomIdentityDb/
├── Persistence/
│   ├── Seeds/       # Role and metadata seeders
│   ├── contexts/    # EF Core DbContext classes (ApplicationDbContext, CustomIdentityDbContext)
│   └── models/      # Parallel persistence models (e.g., ItemModel, BorrowRecordModel)
├── Properties/      # Assembly metadata
├── Repositories/    # Repository implementations (GenericRepository, MappedRepository)
├── Services/        # Infrastructure service implementations (AuthService, IdentityService)
└── UnitOfWork/      # UnitOfWork implementation and interfaces
```

#### `LibrarySystem.API` (Presentation Layer)

```text
LibrarySystem.API/
├── Controllers/     # ASP.NET Core API controllers (Auth, Items, Circulation, ...)
├── Middleware/      # Custom middleware (ExceptionHandling, RequestLogging)
├── Properties/      # Launch profiles and assembly metadata
├── uploads/         # Runtime uploaded files (general and item subfolders)
│   ├── general/
│   └── items/
├── wwwroot/         # Static web assets
│   └── uploads/
├── Dockerfile       # Container image definition
├── Program.cs       # Host setup, DI, middleware pipeline
└── appsettings*.json# Configuration files
```

#### Logging Microservice Projects

```text
Logging.Domain/
├── Common/          # Shared contracts
│   └── IRepositories/# Repository abstractions
└── Entities/        # Log entity contracts

Logging.Application/
├── DTOs/            # Log data transfer objects
└── IServices/       # Service interfaces

Logging.Infrastructure/
├── Migrations/      # EF Core migrations for LoggingDbContext
├── Persistence/
│   └── Contexts/    # LoggingDbContext
├── Repositories/    # Log repository implementations
└── Services/        # Log service implementations

Logging.API/
├── Controllers/     # LogsController
├── Properties/      # Launch profiles
├── Dockerfile       # Container image definition
├── Program.cs       # Host setup
└── appsettings*.json# Configuration files
```

### 2.5 Authentication & Authorization

- **JWT Bearer** tokens with symmetric key.
- **Google OAuth** login via ID-token verification.
- Fallback authorization policy requires authentication on all endpoints by default.
- Public endpoints are explicitly marked `[AllowAnonymous]` (register, login, Google login).
- Roles: `Admin`, `Librarian`, `User`, `Guest`.

### 2.6 Middleware

- `ExceptionHandlingMiddleware`: returns `ProblemDetails` JSON; maps `ValidationException` → 400, `InvalidOperationException` → 400.
- `RequestLoggingMiddleware`: captures HTTP method, path, status code, elapsed time, and authenticated user; forwards logs to `Logging.API`.

---

## 3. Database

### 3.1 Databases

| Context | Database Name (local) | Purpose |
|---|---|---|
| `ApplicationDbContext` | `LibrarySystemDataBas` | Main library catalog, circulation, templates, vocabularies, patrons, settings. |
| `CustomIdentityDbContext` | `IdentifyConnectionDataBase` | ASP.NET Core Identity users and roles. |
| `LoggingDbContext` | `LibrarySystem_LoggingDb` | Request/operation logs (logging microservice). |

### 3.2 Main Domain Entities

| Entity | Description |
|---|---|
| `Resource` | Abstract base for catalog resources. Has an owner (`SystemUser`) and a collection of `Values`. |
| `Item` | A catalog resource. Linked to a `ResourceTemplate`, has `Copies`, `Medias`, and belongs to `ItemSets`. |
| `Media` | A digital file resource linked to an `Item` (storage path, file name, MIME type, size, alt text). |
| `ItemSet` | A curated collection of items with title, description, and visibility. |
| `ItemCopy` | A physical or digital copy of an item with a unique barcode and circulation status. |
| `Patron` | Library member with full name, national ID, phone, and email. |
| `BorrowRecord` | Circulation record linking an `ItemCopy` to a `Patron` with borrow/return/due dates and status. |
| `ResourceTemplate` | Template that defines borrowability, default borrow days, and required properties. |
| `TemplateProperty` | Join between a template and a property (required flag, display order, alternate label). |
| `Vocabulary` | Metadata namespace/prefix (e.g., Dublin Core). |
| `Property` | A field defined inside a vocabulary (label, term URI, searchable flag). |
| `Value` | A metadata value assigned to a resource for a specific property. |
| `SystemUser` | Application user profile linked to an Identity user. |
| `Role` | Application-level role entity. |
| `Bookmark` | User-saved favorite item. |
| `SystemSetting` | Key/value system configuration (e.g., `GlobalBorrowDays`). |

### 3.3 Key Relationships

- `Item` → `ResourceTemplate`: many-to-one.
- `Item` → `ItemCopy`: one-to-many (cascade delete).
- `Item` → `Media`: one-to-many.
- `Item` ↔ `ItemSet`: many-to-many via `ItemSetItems`.
- `ResourceTemplate` ↔ `Property`: many-to-many via `TemplateProperty`.
- `Vocabulary` → `Property`: one-to-many.
- `Resource` → `Value`: one-to-many (cascade delete).
- `BorrowRecord` → `ItemCopy` / `Patron`: many-to-one with restricted delete.
- `SystemUser` → `Resource`: one-to-many (owned resources).
- Identity `AppUserModel` ↔ `SystemUserModel`: optional one-to-one.

### 3.4 Parallel Persistence Models

The project currently maintains a separate set of persistence models under `LibrarySystem.DataAccess/Persistence/models/` (e.g., `ItemModel`, `BorrowRecordModel`). `MappedRepository` translates between domain and persistence models using AutoMapper. This is a known source of technical debt.

---

## 4. Main Features

### 4.1 Authentication (`/api/auth`)

- `POST /api/auth/register` — local registration, creates Identity user + `SystemUser`, assigns `User` role.
- `POST /api/auth/login` — local login, returns JWT.
- `POST /api/auth/login-google` — Google ID-token login/registration.
- `POST /api/auth/logout` — sign out.

### 4.2 Items (`/api/items`)

- Full CRUD with soft delete and restore (`Undelet`).
- List items, list with deleted (admin), get by ID, get with copies.
- Create item with metadata values based on its template.

### 4.3 Item Copies (`/api/item-copies`)

- Manage physical/digital copies by barcode.
- Copy status is set automatically based on template borrowability.
- Soft delete and restore support.

### 4.4 Media (`/api/media`)

- CRUD for media resources.
- Upload file with metadata (`POST /api/media/upload-with-metadata`).
- File upload controller (`/api/files/upload`) generates thumbnails for images using ImageSharp.

### 4.5 Resource Templates (`/api/resource-templates`)

- CRUD plus property management.
- Defines whether items are borrowable and the default borrow duration.
- Soft delete blocked if the template is still in use.

### 4.6 Vocabularies & Properties (`/api/vocabularies`, `/api/properties`)

- Metadata dictionary management.
- Properties can be marked searchable.

### 4.7 Item Sets (`/api/item-sets`)

- Curated collections of items.
- Add/remove items from a set.

### 4.8 Patrons (`/api/patrons`)

- Library member CRUD with search and soft-delete restore.

### 4.9 Users (`/api/users`)

- Admin-only user listing, creation, deletion, and role updates.

### 4.10 Bookmarks (`/api/bookmarks`)

- Authenticated users can save and remove favorite items.

### 4.11 System Settings (`/api/system-settings`)

- Key/value settings such as `GlobalBorrowDays` (default: 14 days).

### 4.12 Circulation (`/api/circulation`)

See Section 5 for detailed borrow/return workflows.

---

## 5. Borrow Operations

### 5.1 Item Copy Status Codes

| Status | Meaning |
|---|---|
| `0` | `Available` |
| `1` | `Borrowed` |
| `2` | `ReferenceOnly` |

### 5.2 Borrow Record Statuses

- `Active`
- `Returned`
- `Overdue`

### 5.3 Checkout Flow

**Endpoint:** `POST /api/circulation/checkout`  
**Command:** `CheckoutCommand(string Barcode, int PatronId, DateTime? CustomDueDate)`

1. Find the `ItemCopy` by barcode.
2. Validate that the copy status is `Available`.
3. Load the parent `Item` and its `ResourceTemplate`.
4. Validate that the template exists and `IsBorrowable == true`.
5. Calculate the due date:
   - Use `CustomDueDate` if provided.
   - Otherwise use `ResourceTemplate.DefaultBorrowDays`.
   - Otherwise fall back to the `GlobalBorrowDays` system setting (default 14).
6. Create a `BorrowRecord` with `Status = "Active"`.
7. Set `ItemCopy.Status = Borrowed`.
8. Save changes.

### 5.4 Return Flow

**Endpoint:** `POST /api/circulation/return`  
**Command:** `ReturnCommand(string Barcode)`

1. Find the `ItemCopy` by barcode.
2. Find the active `BorrowRecord` for that copy where `ReturnDate` is null.
3. Set `ReturnDate = DateTime.UtcNow` and `Status = "Returned"`.
4. Set `ItemCopy.Status = Available`.
5. Save changes.

### 5.5 Circulation Queries

| Endpoint | Description |
|---|---|
| `GET /api/circulation/active` | List all active loans with computed `IsOverdue`. |
| `GET /api/circulation/history` | Borrow history, filterable by item or patron. |
| `GET /api/circulation/overdue` | List overdue loans. |

### 5.6 Validators

- `CheckoutCommandValidator`
- `ReturnCommandValidator`

---

## 6. Folder Responsibilities Explained

This section explains what each folder in the solution is responsible for.

### Domain Layer (`LibrarySystem.Domain`)

| Folder | Responsibility |
|---|---|
| `common/` | Contains base contracts such as `BaseEntity`, `ISoftDelete`, and `IAuditable` used by all entities. |
| `entities/` | Contains plain domain entities (`Item`, `Media`, `Patron`, `BorrowRecord`, `Value`, etc.) with properties and relationships. |
| `enums/` | Contains domain enumerations (e.g., copy status codes). |

### Application Layer (`LibrarySystem.Application`)

| Folder | Responsibility |
|---|---|
| `Behaviors/` | Contains MediatR pipeline behaviors. `ValidationBehavior` runs FluentValidation before handlers execute. |
| `Commands/` | Contains CQRS command records and their handlers, organized by feature. Commands represent write operations. |
| `DTOs/` | Contains Data Transfer Objects used as request/response contracts, organized by feature. |
| `Interfaces/` | Contains abstractions such as `IGenericRepository<T>`, `IUnitOfWork`, `IAuthService`, and `IIdentityService`. |
| `Mappings/` | Contains AutoMapper profiles that map domain entities to DTOs and vice versa. |
| `Queries/` | Contains CQRS query records and their handlers, organized by feature. Queries represent read operations. |
| `Validators/` | Contains FluentValidation validators, organized by feature, used to validate commands and queries. |

### Infrastructure Layer (`LibrarySystem.DataAccess`)

| Folder | Responsibility |
|---|---|
| `Http/` | Contains HTTP clients (e.g., `LoggingClient`) used to call external services such as `Logging.API`. |
| `Mappings/` | Contains AutoMapper profiles that map domain entities to persistence models and back. |
| `Migrations/` | Contains EF Core migration files for the application and identity databases. |
| `Persistence/Seeds/` | Contains seeders (`RoleSeeder`, `MetadataSeeder`) that populate default data at startup. |
| `Persistence/contexts/` | Contains EF Core `DbContext` implementations (`ApplicationDbContext`, `CustomIdentityDbContext`). |
| `Persistence/models/` | Contains parallel persistence models (e.g., `ItemModel`, `BorrowRecordModel`) used by EF Core. |
| `Repositories/` | Contains repository implementations, including `GenericRepository<T>` and `MappedRepository<TDomain, TPersistence>`. |
| `Services/` | Contains infrastructure service implementations such as `AuthService` and `IdentityService`. |
| `UnitOfWork/` | Contains the `UnitOfWork` implementation that aggregates repositories and commits changes. |

### Presentation Layer (`LibrarySystem.API`)

| Folder | Responsibility |
|---|---|
| `Controllers/` | Contains ASP.NET Core controllers that expose HTTP endpoints and delegate work to MediatR. |
| `Middleware/` | Contains custom middleware for exception handling and request logging. |
| `uploads/` | Runtime folder for uploaded files (general uploads and item-specific uploads). |
| `wwwroot/` | Static web assets served by the host. |

### Logging Microservice (`Logging.*`)

| Project / Folder | Responsibility |
|---|---|
| `Logging.Domain/Common/` | Shared contracts and repository abstractions. |
| `Logging.Domain/Entities/` | `Log` entity definition. |
| `Logging.Application/DTOs/` | Log DTOs. |
| `Logging.Application/IServices/` | Log service interfaces. |
| `Logging.Infrastructure/Persistence/Contexts/` | `LoggingDbContext` for the logging database. |
| `Logging.Infrastructure/Repositories/` | Log repository implementation. |
| `Logging.Infrastructure/Services/` | Log service implementation. |
| `Logging.API/Controllers/` | `LogsController` exposing log endpoints. |

## 7. Project Preferences & Conventions

### 7.1 Coding Style

- C# with implicit usings and nullable reference types enabled.
- CQRS commands/queries are declared as `record` types where appropriate.
- Async methods use the `Async` suffix (`GetByIdAsync`, `SaveChangesAsync`).
- Arabic comments appear in some controllers alongside English code.

### 7.2 File / Naming Notes

A few filenames contain typos or trailing spaces that should be preserved when referencing them:

- `LibrarySystem.DataAccess/Persistence/models/MdeiaModel.cs`
- `LibrarySystem.DataAccess/Persistence/models/BookmarkModel .cs`
- `LibrarySystem.Application/Commands/Media/UpdateMediaComman.cs`
- `LibrarySystem.DataAccess/UnitOfWork/UnitOfWork .cs`
- `LibrarySystem.Application/Interfaces/IUnitOfWork .cs`
- Several controllers expose `Undelet` instead of `Undelete` routes.

### 7.3 Dependency Injection

- `ApplicationServiceRegistration.AddApplicationServices()` registers AutoMapper, MediatR, FluentValidation, and the validation pipeline behavior.
- `InfrastructureServiceRegistration.AddInfrastructureServices()` registers EF Core contexts, Identity, repositories, unit of work, auth services, and the logging HTTP client.

### 7.4 Configuration

- `appsettings.json` uses local SQL Server with Windows authentication (`Trusted_Connection=True`).
- Connection string keys: `DefaultConnection`, `IdentityConnection` (with fallback to legacy `IdentifyConnection`).
- JWT and Google OAuth credentials are stored in `appsettings.json` — replace with secrets in production.
- `LoggingService:BaseUrl` defaults to `http://localhost:5113`.

### 7.5 Development

- **CORS:** configured for the React Vite dev server at `http://localhost:5173`.
- **Swagger:** enabled only in Development with Bearer security definition.
- **Serilog:** console logging.
- **Migrations:** applied automatically at startup for both main and Identity contexts.
- **Seeding:** roles are seeded on startup; development seeder creates sample vocabularies, properties, templates, users, items, media, and item sets.

### 7.6 Deployment

- **Docker Compose:** orchestrates SQL Server, `Logging.API`, and `LibrarySystem.API`.
- **Ports:** SQL Server `1433`, main API `5112`, logging API `5113`.
- **CI:** GitHub Actions builds the solution and runs a vulnerable package scan.

### 7.7 Logging Microservice

- Stores `Log` entities with message, creator, and timestamp.
- Endpoints: `POST /api/logs`, `GET /api/logs`, `GET /api/logs/test`.
- If `Logging.API` is unavailable, the main API logs a warning locally and continues.

### 7.8 Known Technical Debt

- Domain entities and persistence models are duplicated, increasing mapping complexity.
- Several filename typos/trailing spaces remain.
- No unit or integration tests yet.
- HTTP fire-and-forget logging; durable message queue recommended for high traffic.
- Vulnerable package warnings on `AutoMapper` and `SixLabors.ImageSharp`.

---

## 8. Technology Stack

| Category | Technology |
|---|---|
| Framework | ASP.NET Core 8 |
| ORM | Entity Framework Core 8 (SQL Server) |
| Mediator | MediatR 12 |
| Mapping | AutoMapper 13 |
| Validation | FluentValidation 11 |
| Identity | ASP.NET Core Identity |
| Auth | JWT Bearer + Google OAuth |
| Logging | Serilog + custom `Logging.API` microservice |
| Image Processing | SixLabors.ImageSharp |
| Containerization | Docker / Docker Compose |
| CI/CD | GitHub Actions |

---

*Document generated for AI/agent context. Last updated: 2026-06-22.*
