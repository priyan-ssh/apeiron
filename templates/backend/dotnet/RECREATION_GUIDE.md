# 🛡️ Apeiron: The Clean Architecture Re-Creation Guide

> **Objective:** Manually reconstruct the .NET 10 Clean Architecture Skeleton.
> **Status:** Learning Mode

Follow these steps exactly to rebuild the backend from scratch.

## 1. The Foundation (Solution)

Create the master container for the project.

```bash
# In templates/backend/dotnet
dotnet new sln -n Apeiron
```

## 2. The Core Projects (Layers)

We use a `src` folder to keep things clean.

```bash
mkdir src
```

### Layer 1: The Domain (Pure Logic)
*   **Role:** The "King". No external dependencies.
*   **Content:** Entities, Value Objects, Domain Exceptions.

```bash
dotnet new classlib -n Apeiron.Domain -o src/Apeiron.Domain
```

### Layer 2: The Application (Orchestration)
*   **Role:** The "Guard". Orchestrates the Domain.
*   **Content:** Interfaces (IUserRepository), Commands, Queries, Validations.

```bash
dotnet new classlib -n Apeiron.Application -o src/Apeiron.Application
```

### Layer 3: The Infrastructure (The Plumbing)
*   **Role:** The "Walls". Implements interfaces.
*   **Content:** DbContext (PostgreSQL), Repositories, External APIs.

```bash
dotnet new classlib -n Apeiron.Infrastructure -o src/Apeiron.Infrastructure
```

### Layer 4: The Api (The Gateway)
*   **Role:** The "Gate". HTTP Entry Point.
*   **Content:** Controllers, Middleware, Program.cs.

```bash
dotnet new webapi -n Apeiron.Api -o src/Apeiron.Api
```

## 3. The Gravity (Dependencies)

This is the most critical step. The dependency flow must be correct.

**Rule:** Dependencies point **INWARD** toward the Domain.

```bash
# 1. Application depends on Domain
dotnet add src/Apeiron.Application reference src/Apeiron.Domain

# 2. Infrastructure depends on Application AND Domain
dotnet add src/Apeiron.Infrastructure reference src/Apeiron.Application
dotnet add src/Apeiron.Infrastructure reference src/Apeiron.Domain

# 3. Api depends on Application AND Infrastructure
dotnet add src/Apeiron.Api reference src/Apeiron.Application
dotnet add src/Apeiron.Api reference src/Apeiron.Infrastructure
```

## 4. The Union (Update Solution)

Tell the `.sln` file that these projects exist.

```bash
dotnet sln add src/Apeiron.Domain
dotnet sln add src/Apeiron.Application
dotnet sln add src/Apeiron.Infrastructure
dotnet sln add src/Apeiron.Api
```

## 5. Phase 2.5: The Organs (NuGet Upgrades)

Install the necessary drivers and observability tools.

```bash
# Infrastructure: Database and Cache
dotnet add src/Apeiron.Infrastructure package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add src/Apeiron.Infrastructure package Microsoft.Extensions.Caching.Hybrid --prerelease

# API: Observability (OpenTelemetry)
dotnet add src/Apeiron.Api package OpenTelemetry.Extensions.Hosting
dotnet add src/Apeiron.Api package OpenTelemetry.Instrumentation.AspNetCore
dotnet add src/Apeiron.Api package OpenTelemetry.Instrumentation.Http
dotnet add src/Apeiron.Api package OpenTelemetry.Exporter.OpenTelemetryProtocol
```

## 6. Phase 2.5: The DNA (Domain Entities)

We define the core entities in specific folders.

```bash
# Allow Domain to use IdentityUser type for future-proofing
dotnet add src/Apeiron.Domain package Microsoft.Extensions.Identity.Stores
```

Create folders: `src/Apeiron.Domain/Entities`

Create classes inside `Entities/`:
*   `BaseEntity.cs` (Abstract, namespace `Apeiron.Domain.Entities`)
*   `Project.cs` (namespace `Apeiron.Domain.Entities`)
*   `User.cs` (Inherits `IdentityUser<Guid>`, namespace `Apeiron.Domain.Entities`)

## 7. Phase 2.5: The Contracts (Application Interfaces)

We define the rules of engagement.

Create folders: `src/Apeiron.Application/Interfaces`

Create interfaces inside `Interfaces/`:
*   `IProjectRepository.cs` (namespace `Apeiron.Application.Interfaces`)
*   `IUserRepository.cs` (namespace `Apeiron.Application.Interfaces`)
*   `IProjectService.cs` (namespace `Apeiron.Application.Interfaces`)
*   `IUserService.cs` (namespace `Apeiron.Application.Interfaces`)

## 8. Phase 2.5: The Plumbing (Infrastructure Implementation)

We implement the database access.

#### 1. The Database Context
Create `src/Apeiron.Infrastructure/Persistence/ApeironDbContext.cs`.
It MUST inherit from `IdentityDbContext<User, IdentityRole<Guid>, Guid>`, NOT just `DbContext`.
It MUST include `public DbSet<Project> Projects { get; set; }`.

#### 2. The Repositories
Create `src/Apeiron.Infrastructure/Repositories/`:
*   `ProjectRepository.cs` (Implements `IProjectRepository`)
*   `UserRepository.cs` (Implements `IUserRepository`)

#### 3. The Infrastructure Wire-Up
Create `src/Apeiron.Infrastructure/DependencyInjection.cs`:
This `AddInfrastructure` method registers the DbContext and Repositories.

## 9. Phase 3: The Nervous System (Application Services)

We implement the business logic.

#### 1. The Services
Create `src/Apeiron.Application/Services/`:
*   `ProjectService.cs` (Implements `IProjectService`)
*   `UserService.cs` (Implements `IUserService`)

#### 2. The Application Wire-Up
Create `src/Apeiron.Application/DependencyInjection.cs`:
This `AddApplication` method registers the Services implementing the Interfaces using `AddScoped`.

## 10. Phase 2.5: The Ignition (API Entry Point)

Final step: Wire the layers into `Program.cs`.

Modify `src/Apeiron.Api/Program.cs`:

```csharp
using Apeiron.Application;
using Apeiron.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// 1. Add Layers (The Clean Architecture Wiring)
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// 2. Add API Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 3. The Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

## 11. Initializing DevOps (Docker)

Create the local infrastructure container.

Create `templates/devops/docker-compose.yml`:
(Include Postgres, Redis, Seq).

## 12. The Database Birth (Migrations)

You need the EF Core tools installed.

```bash
dotnet tool install --global dotnet-ef
dotnet add src/Apeiron.Api package Microsoft.EntityFrameworkCore.Design
```

Generate the initial schema:

```bash
# From templates/backend/dotnet
dotnet ef migrations add InitialCreate -p src/Apeiron.Infrastructure -s src/Apeiron.Api
```

Apply to the running Docker database:

```bash
dotnet ef database update -p src/Apeiron.Infrastructure -s src/Apeiron.Api
```

## 13. Final Verification

```bash
dotnet build
```
