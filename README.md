# CloudNet

## 1) Project Overview
CloudNet is a multi-project ASP.NET Core solution built for scalable backend systems. It applies Clean Architecture and CQRS (via MediatR) to keep the domain and application logic independent from delivery and infrastructure concerns, enabling maintainable and testable services.

## 2) Architecture Overview
CloudNet follows Clean Architecture, splitting responsibilities into distinct layers:

- **Domain**: Core business entities, value objects, and rules. No dependencies on other layers.
- **Application**: Use cases implemented as CQRS commands and queries, plus DTOs and validation.
- **Infrastructure**: External concerns such as persistence (EF Core) and identity/authentication.
- **API**: Thin HTTP layer that maps requests to application use cases.

**Dependency direction:**
- Dependencies always point inward (API → Application → Domain). Infrastructure implements application interfaces and is wired from the outer layers.

**CQRS rationale:**
- Separates reads and writes for clearer models, simpler validation, and better scalability.

## 3) Solution Structure
```
CloudNet
├── CloudNet.Api
├── CloudNet.Api.Abstractions
├── CloudNet.Application
├── CloudNet.Domain
├── CloudNet.Infrastructure.Identity
├── CloudNet.Infrastructure.Persistence
└── CloudNet.Web
```

- **CloudNet.Api**: Thin Web API layer (controllers only).
- **CloudNet.Api.Abstractions**: API contracts, DTOs, filters, Swagger configuration, and shared API concerns.
- **CloudNet.Application**: CQRS handlers, application DTOs, and validators.
- **CloudNet.Domain**: Domain entities, value objects, and business rules.
- **CloudNet.Infrastructure.Identity**: ASP.NET Core Identity, JWT auth, refresh tokens.
- **CloudNet.Infrastructure.Persistence**: EF Core DbContext, repositories, and database access.
- **CloudNet.Web**: UI layer or future frontend integration.

## 4) Technology Stack
- **.NET**: `net9.0`
- **ASP.NET Core**: Web API hosting and middleware
- **Entity Framework Core**: Data access and migrations
- **MediatR**: CQRS implementation
- **ASP.NET Core Identity + JWT**: Authentication and authorization
- **Swagger / OpenAPI**: API documentation
- **GitHub Actions**: Continuous integration

## 5) Getting Started

### Prerequisites
- .NET SDK 9.0+
- A supported database server (e.g., PostgreSQL)

### Clone the repository
```
git clone <repository-url>
cd CloudNet
```

### Restore and build
```
dotnet restore

dotnet build
```

### Run the API
```
dotnet run --project src/CloudNet.Api
```

### Run migrations (if applicable)
```
dotnet ef database update \
  --project src/CloudNet.Infrastructure.Persistence \
  --startup-project src/CloudNet.Api
```

## 6) Configuration
Configuration is managed via `appsettings.json` and environment-specific overrides.

Typical settings include:
- **ConnectionStrings:DefaultConnection**: Database connection string
- **Jwt**: Settings for issuer, audience, signing key, and token lifetime
- **Logging**: Logging levels and providers

Environment variables can override any configuration key using standard ASP.NET Core conventions.

## 7) Authentication & Security
- **JWT Authentication**: Access tokens are issued upon successful login.
- **Refresh Token Rotation**: Refresh tokens are rotated to prevent reuse and improve security.
- **Authorization**: Role- and claim-based policies can be applied at controller or action level.

## 8) Development Guidelines
- **Clean Architecture rules**: Inner layers must not depend on outer layers.
- **Feature placement**:
  - Domain rules and entities → `CloudNet.Domain`
  - Use cases and CQRS handlers → `CloudNet.Application`
  - Persistence and external services → `CloudNet.Infrastructure.*`
  - HTTP endpoints → `CloudNet.Api`
- **Naming conventions**:
  - Commands: `CreateXCommand`, `UpdateXCommand`
  - Queries: `GetXQuery`, `ListXQuery`
  - Handlers: `CreateXCommandHandler`, `GetXQueryHandler`

### Adding a new CQRS feature
1. Define the command/query and response DTO in `CloudNet.Application`.
2. Implement the handler and validation.
3. Add controller endpoints in `CloudNet.Api`.
4. Register any required services in the composition root.

## 9) CI / Build
- **GitHub Actions** is used to validate builds and tests.
- **Central package management** (`Directory.Packages.props`) ensures consistent dependency versions.

## 10) Future Improvements / Roadmap
- Response caching
- Rate limiting
- Auditing and soft deletes
- Observability (tracing, metrics)
- Frontend integration in `CloudNet.Web`

## 11) License / Contribution
- **License**: TBD
- **Contributions**: PRs are welcome. Please open an issue to discuss major changes.
