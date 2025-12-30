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
dotnet add src/Apeiron.Infrastructure package System.IdentityModel.Tokens.Jwt
dotnet add src/Apeiron.Api package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add src/Apeiron.Api package Asp.Versioning.Mvc
dotnet add src/Apeiron.Api package Asp.Versioning.Mvc.ApiExplorer
```

## 6. Phase 2.5: The DNA (Domain Entities)

We define the core entities in specific folders.

```bash
# Allow Domain to use IdentityUser type for future-proofing
dotnet add src/Apeiron.Domain package Microsoft.Extensions.Identity.Stores

# Infrastructure: Interceptors
Create `src/Apeiron.Infrastructure/Persistence/Interceptors/AuditingInterceptor.cs`.
It must inherit `SaveChangesInterceptor` and set `CreatedAt`/`ModifiedAt`.
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
It must also register `services.AddHybridCache()` and the `AuditingInterceptor`.
And `services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>()`.

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

## 11. The Voice (Controllers)

Create the RESTful entry points.

#### 1. The Base Controller
Create `src/Apeiron.Api/Controllers/BaseApiController.cs`. All other controllers should inherit from this to share common attributes.

#### 2. The Features
Implement the following in `src/Apeiron.Api/Controllers/`:
- `HealthController.cs`: Simple "Ok" status.
- `ProjectsController.cs`: CRUD operations using `IProjectService`.
- `UsersController.cs`: Registration and retrieval using `IUserService`.

## 12. Initializing DevOps (Docker)

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

## 13. Phase 2.5: The Shield (Universal Result Patterns)

Every professional API needs a consistent response envelope. We avoid throwing raw data or string errors.

#### 1. The Models
Create `src/Apeiron.Application/Common/Models/`:
- `Result.cs`: The success/failure wrapper.
- `PagedResult.cs`: The list/pagination wrapper.

#### 2. The Implementation (Result.cs)
```csharp
namespace Apeiron.Application.Common.Models;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    protected Result(bool isSuccess, Error error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, Error.None);
    public static Result Failure(Error error) => new(false, error);
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    protected internal Result(TValue? value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Failure result has no value.");

    public static Result<TValue> Success(TValue? value) => new(value, true, Error.None);
    public static new Result<TValue> Failure(Error error) => new(default, false, error);
}

public record Error(string Code, string Message)
{
    public static readonly Error None = new(string.Empty, string.Empty);
}
```

#### 3. The Implementation (PagedResult.cs)
```csharp
namespace Apeiron.Application.Common.Models;

public class PagedResult<TValue>
{
    public List<TValue> Items { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    public PagedResult(List<TValue> items, int pageNumber, int pageSize, int totalCount)
    {
        Items = items;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
    }
}
```

### 13.B The Public Promise (API Contracts)

We never expose Domain Entities directly. We use Contracts.

#### 1. The Folder Structure
```text
src/Apeiron.Application/
└── Contracts/
    ├── Projects/
    │   ├── ProjectCreateRequest.cs
    │   └── ProjectResponse.cs
    └── Users/
        ├── UserRegisterRequest.cs
        └── UserResponse.cs
```

#### 2. The Implementation (Example: Projects)

**ProjectCreateRequest.cs**
```csharp
namespace Apeiron.Application.Contracts.Projects;

public record ProjectCreateRequest(string Name, string? Description);
```

**ProjectResponse.cs**
```csharp
namespace Apeiron.Application.Contracts.Projects;

public record ProjectResponse(Guid Id, string Name, string? Description, DateTime CreatedAt);
```

#### 3. The Implementation (Example: Users)

**UserRegisterRequest.cs**
```csharp
namespace Apeiron.Application.Contracts.Users;

public record UserRegisterRequest(string Email, string Password, string FirstName, string LastName);
```

**UserResponse.cs**
```csharp
namespace Apeiron.Application.Contracts.Users;

public record UserResponse(Guid Id, string Email, string FirstName, string LastName, DateTime CreatedAt);
```

### 13.C The Choice (Validation)

#### Option A: The "Quick Ship" (Decorators)
Use this for early MVPs. Add attributes directly to your DTOs in `src/Apeiron.Application/Contracts/`.

```csharp
using System.ComponentModel.DataAnnotations;

public record ProjectCreateRequest(
    [Required][StringLength(100)] string Name,
    [StringLength(500)] string Description);
```

#### Option B: The "Industrial Grade" (FluentValidation)
Use this for production. Create separate validator classes.

```csharp
// (As defined in 13.C Step 1)
```

#### 13.D The Bouncer (Validation Behavior)
To make validation automatic, we create a **MediatR Behavior**.

1. Create `src/Apeiron.Application/Common/Behaviors/ValidationBehavior.cs`:

```csharp
using FluentValidation;
using MediatR;

namespace Apeiron.Application.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any()) return await next();

        var context = new ValidationContext<TRequest>(request);

        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(result => result.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count != 0)
        {
            throw new ValidationException(failures);
        }

        return await next();
    }
}
```

2. Create `src/Apeiron.Application/Contracts/Projects/ProjectCreateValidator.cs`:

```csharp
using FluentValidation;

namespace Apeiron.Application.Contracts.Projects;

public class ProjectCreateValidator : AbstractValidator<ProjectCreateRequest>
{
    public ProjectCreateValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must be less than 100 characters.");
            
        RuleFor(x => x.Description)
            .MaximumLength(500);
    }
}
```

3. Register in `DependencyInjection.cs` (Application Layer):
```csharp
services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});
```

**Note:** APEIRON favors Option B for the final generator, but you should use Option A to get your first endpoints live today.

## 14. Phase 2.6: The Safety Net (Global Exception Handling)

We never let raw stack traces leak to the user. We use the native .NET `IExceptionHandler`.

#### 1. The Middleware
Create `src/Apeiron.Api/Infrastructure/GlobalExceptionHandler.cs`.

```csharp
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Apeiron.Api.Infrastructure;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> _logger)
    {
        this._logger = _logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unhandled exception occurred: {Message}", exception.Message);

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Server Error",
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1"
        };

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
```

#### 2. The Registration
Update `Program.cs`:

```csharp
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// In the middleware pipeline (before MapControllers)
app.UseExceptionHandler();
```

## 16. Phase 2.7: The Grand Wiring (Controllers & Services)

Now we connect the briefcase (`Result`), the diamonds (`Contracts`), and the net (`Exception Handler`).

#### 1. Update the Service Interface
Update `src/Apeiron.Application/Interfaces/IProjectService.cs`:

```csharp
using Apeiron.Application.Common.Models;
using Apeiron.Application.Contracts.Projects;

namespace Apeiron.Application.Interfaces;

public interface IProjectService
{
    Task<Result<List<ProjectResponse>>> GetAllAsync();
    Task<Result<ProjectResponse>> GetByIdAsync(Guid id);
    Task<Result<ProjectResponse>> CreateAsync(ProjectCreateRequest request);
}
```

#### 2. Update the Controller
Update `src/Apeiron.Api/Controllers/ProjectsController.cs`:

```csharp
[HttpGet("{id}")]
public async Task<ActionResult<ProjectResponse>> Get(Guid id)
{
    var result = await _projectService.GetByIdAsync(id);
    
    return result.IsSuccess 
        ? Ok(result.Value) 
        : NotFound(result.Error); // Or your custom error handling
}

[HttpPost]
public async Task<ActionResult<ProjectResponse>> Create(ProjectCreateRequest request)
{
    var result = await _projectService.CreateAsync(request);
    
    return result.IsSuccess 
        ? CreatedAtAction(nameof(Get), new { id = result.Value.Id }, result.Value)
        : BadRequest(result.Error);
}
```

**Why this matters?**
The Controller now speaks **JSON (Contracts)** but the Service manages the **Safety (Result)**. No raw entities ever leave the building.

#### 3. Update the Service Implementation
Implement the logic to map `ProjectCreateRequest` -> `Project (Entity)` -> `ProjectResponse`.

---

### Phase 2.8: Testing the Net
Throw a `new Exception("Broke on purpose")` inside a service and hit the endpoint. You should see a clean JSON `ProblemDetails` response instead of a crash!

---

## 17. Phase 2.9: Structured Logging (Serilog)

We replace the default .NET console logger with **Serilog** for better formatting and "Structured Data" (JSON logs).

#### 1. Add NuGet Packages
```bash
dotnet add src/Apeiron.Api package Serilog.AspNetCore
dotnet add src/Apeiron.Api package Serilog.Sinks.Console
dotnet add src/Apeiron.Api package Serilog.Sinks.Seq
```

#### 2. Configure `appsettings.json`
Add the Serilog block:

```json
"Serilog": {
  "Using": [ "Serilog.Sinks.Console", "Serilog.Sinks.Seq" ],
  "MinimumLevel": {
    "Default": "Information",
    "Override": {
      "Microsoft": "Warning",
      "System": "Warning"
    }
  },
  "WriteTo": [
    { "Name": "Console" },
    {
      "Name": "Seq",
      "Args": { "serverUrl": "http://localhost:5341" }
    }
  ],
  "Enrich": [ "FromLogContext", "WithMachineName", "WithThreadId" ]
}
```

#### 3. Wire up `Program.cs`
Update the builder initialization:

```csharp
using Serilog;

// ... at the top
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try 
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    // ... rest of building logic
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
```

**Why Serilog?**
Standard logs are just strings. Serilog logs are **Objects**. If you log `_logger.Information("Processed {ProjectId}", id)`, Serilog stores the `id` as a searchable field in your log database (like Seq).

## 18. Phase 4: Final Polish (Config & Auth)

#### 1. API Versioning
Update `BaseApiController.cs`:
```csharp
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
```
Configure in `Program.cs` with `builder.Services.AddApiVersioning()`.

#### 2. Authentication (JWT)
1. Configure `Jwt` section in `appsettings.json`.
2. Implement `JwtTokenGenerator` in Infrastructure.
3. Add `builder.Services.AddAuthentication().AddJwtBearer()` in `Program.cs`.
4. Add `app.UseAuthentication()` before `app.UseAuthorization()`.

---
**Congratulation!** You have rebuilt the APEIRON backend.


