# 📓 APEIRON Mission Log: Technical Q&A Summary

This log captures the core technical questions and design decisions made during the initialization of Project APEIRON.

---

### 🧬 Agent Protocol & Context
**Q: Do subagents allow parallel work or individual unpolluted context?**
*   **A:** Cognitively, yes; technically, no. The models switch personas to maintain logical separation (e.g., Vision refuses to code). However, they share a conversation history. True parallelism requires separate agent instances.

**Q: Does Antigravity inherit GitHub credentials from VS Code?**
*   **A:** Yes. Since the terminal is spawned by the IDE, it inherits the session and any active Credential Helpers.

---

### 🛠 CLI & Tooling (The Forge)
**Q: Why Node.js for the Generator?**
*   **A:** Ecosystem richness (`commander`, `fs-extra`) and team relevance (React/TS developers can maintain the tool).
*   **Q: Why not use `git submodules`?**
*   **A:** Maintenance overhead. We prefer a clean "cloning/ejection" model for templates.

---

### 🛡 .NET Backend (The Skeleton)
**Q: Why .NET 10 over v9?**
*   **A:** .NET 10 is the latest LTS (Long Term Support) version (released Nov 2025), providing better performance and 3 years of support.

**Q: What is the mapping from N-Tier (BLL/DAL) to Clean Architecture?**
*   **A:** 
    *   **Controller** -> `Api`
    *   **Service (BLL)** -> `Application` (Use Cases)
    *   **Model** -> `Domain` (Pure Entities)
    *   **DAL / DbContext** -> `Infrastructure`

**Q: Does having pure Domain models cause redundancy in the DAL?**
*   **A:** No. We use EF Core's **Fluent API** in the Infrastructure layer to configure the Pure models from the Domain, avoiding duplication.

**Q: Why does Infrastructure depend on Application (Dependency Inversion)?**
*   **A:** To keep the logic pure. `Application` defines the **Interface**; `Infrastructure` implements it. Therefore, `Infrastructure` must reference `Application`.

**Q: Why use a `src` folder?**
*   **A:** Organization. It keeps the production code separate from `tests/`, `cli/`, and `devops/` assets.

---

### ⌨️ Terminal & OS Shortcuts
**Q: How to clear the current terminal line?**
*   **A:** `Ctrl + U` (Start to cursor), `Ctrl + K` (Cursor to end), or `Ctrl + C` to abort entirely.
**Q: What is `mkdir -p`?**
*   **A:** The `parents` flag. It creates any missing parent directories and suppresses errors if the directory already exists.

---

### 🏗 Build Physics
**Q: Why was the first build slow (18s)?**
*   **A:** The initial "First Date" tax: Full NuGet restore, Roslyn compiler warmup, and processing 4 separate projects for the first time.
*   **Q: Why are incremental builds faster?**
*   **A:** .NET only recompiles projects where source files have changed. Subsequent builds take ~2-5s.

---

### 📦 Dependencies & Data (The Organs)
**Q: Is Entity Framework (EF) by Microsoft?**
*   **A:** Yes. We use EF Core, the official, open-source ORM from Microsoft. It's the industry standard for .NET data access.

**Q: Why use `--prerelease` for HybridCache?**
*   **A:** Because we are using .NET 10. `HybridCache` is a new library designed for .NET 10+ that unifies L1 (In-Memory) and L2 (Redis) caching. Since we are on the cutting edge, we use the latest prerelease version optimized for the v10 SDK.

### 🔌 Wiring & Configuration
**Q: When do we configure `Program.cs`?**
*   **A:** At the very end. We adhere to strict "Layered Injection":
    1.  **Infrastructure** defines `AddInfrastructure(config)`.
    2.  **Application** defines `AddApplication()`.
    3.  **API** (`Program.cs`) simply calls these two methods. This keeps the entry point clean and agnostic of implementation details.

**Q: Why inherit `IdentityUser<Guid>` in the Domain?**
*   **A:** To bridge our Domain `User` entity seamlessly with ASP.NET Core Identity. By doing this in the Domain (referencing `Microsoft.Extensions.Identity.Stores`), we avoid complex mapping layers while keeping the Domain relatively pure (no EF Core dependency).

### 🏗️ Architecture & Patterns
**Q: Why separate `Repositories` from `Persistence/DbContext`?**
*   **A:** For clarity. `Persistence` holds the layout (DbContext), while `Repositories` hold the implementation logic. Keeping them in separate root folders under Infrastructure keeps the project flat and navigable.

**Q: Why are Interfaces (`IProjectRepository`) in Application, not Infrastructure?**
*   **A:** **Dependency Inversion Principle.** The Application layer defines the *contract* it needs. The Infrastructure layer *fulfills* that contract. Dependencies must point INWARD (`Infra -> Application`). If interfaces were in Infra, the Application would depend on Infra, violating the core rule of Clean Architecture.

---
### 🐳 DevOps & Database Liftoff
**Q: how will swagger initialise if we haven't built any controllers?**
*   **A:** Swagger will initialize and show an empty document (or only built-in health endpoints). It is essentially a "Ghost Town" until we decorate it with Controllers. We should build a `ProjectController` soon to verify the plumbing.

**Q: Should migration files be in `.gitignore`?**
*   **A:** **Emphatically NO.** Migrations are source code. They represent the history of your schema. If you ignore them, your teammates (and your production server) won't know how to build the database. Always commit your migrations.

**Q: Why was `Microsoft.EntityFrameworkCore.Design` needed in the API?**
*   **A:** The `dotnet ef` CLI tool needs a "Design-time" bridge to look into the API's configuration (`appsettings.json`) to find the connection string. Without this package in the startup project, the tool is blind.

**Q: Should the test project be part of the template?**
*   **A:** **YES.** A professional template includes a testing framework by default. It signals that the generated project is ready for industrial use, not just a prototype. We include `tests/` for Unit, Integration, and Architecture (NetArchTest) tests.

**Q: Why xUnit and not NUnit/MSTest?**
*   **A:** **Isolation & Modernity.**
    *   **Instance per Test**: xUnit creates a *new instance* of the test class for every single test method. This guarantees zero "State Leakage" (variables from Test A won't mess up Test B). NUnit reuses the class instance.
    *   **Idiomatic C#**: xUnit uses Constructors for `Setup` and `IDisposable` for `Teardown`, rather than magic attributes (`[SetUp]`). It feels more like writing real code.
    *   **Parallelism**: It is designed for parallel execution by default, making suites run faster.

**Q: Why `public partial class Program { }`?**
*   **A:** **Integration Testing.** Top-level statements (standard in .NET 6+) compile to an internal, hidden `Program` class. To run Integration Tests using `WebApplicationFactory<Program>`, the test project needs to "see" this class. Adding this one line exposes the hidden class to the outside world, effectively making it `public`.

**Q: Why reference `Application.Models` in `Program.cs`?**
*   **A:** **Strongly Typed Config.** We store `FeatureFlags` (EnableAuth, EnableOTEL) in `appsettings.json`. To read them safely, we deserialize the JSON into the C# `FeatureFlags` class, which lives in `Application/Common/Models`. This prevents "Magic String" errors when toggling features.
    
**Q: Will I lose my database data if I do `docker compose down`?**
*   **A:** **NO.** We use **Docker Volumes**. In `docker-compose.yml`, the `postgres_data` volume maps the internal database files to your host machine's disk. Even if the container is deleted, the data lives on. Only `docker compose down -v` (with the `-v` flag) will wipe the data.

---

## 🛑 Retrospective: Did we Overengineer it?
**Verdict:** **No.** We built a **Platform**, not a Prototype.

### 1. The "Day 2" Defense
Most templates are optimized for "Day 1" (File -> New Project -> Run). They look simple but fall apart on "Day 2":
*   *"Where do I put this validation logic?"* -> Bloated Controllers.
*   *"Why is the API slow?"* -> No Caching or Tracing.
*   *"The app crashed in production!"* -> No Global Exception Handling or Structured Logs.

**APEIRON** solves Day 2 problems on Day 1.

### 2. Complexity on Demand (Feature Flags)
We added complexity (HybridCache, OpenTelemetry, Auth), but we **tamed it** with Feature Flags.
*   **Junior Dev Mode:** Set flags to `false`. You have a simple CRUD API.
*   **Senior Dev Mode:** Set flags to `true`. You have a distributed, observable, secured system.

### 3. Clean Architecture Costs
*   **Cost:** More files (DTOs, Interfaces).
*   **Gain:** You can change the Database, the Auth Provider, or the Cache without rewriting your business logic.

This is **Industrial Grade**. It feels "heavy" because it carries the weight of reliability.


**Q: Why don't we put all 'Models' in the Domain?**
*   **A:** **The Hierarchy of Truth.** 
    1. **Domain Entities** (`Project.cs`): These are the "Truth." They own the state and the rules of the business.
    2. **Contracts / DTOs** (`ProjectResponse.cs`): These are the "Promise." They are what we show the outside world. Never expose the Truth directly to the JSON; it creates tight coupling.
    3. **Application Models** (`Result.cs`): These are the "Envelope." They help us carry the Truth safely through the plumbing.

**Q: Is an 'API Contract' just a document for the frontend?**
*   **A:** **Partially.** Swagger/OpenAPI is the "Document." But the **Code Contract** (the DTO classes) is the "Manual" that the computer uses to enforce that document. If the document says "I return a Name," the Code Contract ensures the JSON actually has a `string Name`.

**Q: What is the difference between a Contract and a DTO?**
*   **A:** **Naming & Intent.**
    *   **DTO (Data Transfer Object)** is the general category. Any object that just carries data is a DTO.
    *   **Contract** is the specific name for a DTO that we show the **public**. It is our "Signed Promise" to the frontend. All Contracts are DTOs, but we use the term 'Contract' to highlight that these files define the API's face.

**Q: Do we need both 'API Contracts' and 'Application DTOs'?**
*   **A:** **The 'One-Mirror' Rule.** For APEIRON, No. We only need **one** layer of DTOs (which we call **Contracts**) to mirror the Domain Entities for the public. Creating *another* set of DTOs just for the Application layer to talk to the API layer would be "Over-Engineering." We keep it pragmatic: Domain for state, Contracts for the window.

**Q: Should I use decorators (`[Required]`) or a middleware for validation?**
*   **A:** **Neither and Both.** We use **FluentValidation** (in the Application layer) plus a **Middleware/Behavior**. 
    *   **Anti-Pattern**: Using decorators on Contracts (`[Required]`). It’s inflexible and makes your DTOs look like a Christmas tree.
    *   **Pro-Pattern**: Dedicated Validator classes. It keeps your DTOs clean, allows for complex logic (like checking if a name already exists in the DB), and can be unit tested easily.

**Q: Gemini recommended decorators for type safety. Why is Friday Audit so against them?**
*   **A:** **The 'Pure vs. Easy' Debate.** 
    *   **The Easy Way (Decorators)**: `[Required]` is great for small apps. It’s "Easy Type Safety."
    *   **The APEIRON Way (FluentValidation)**: In .NET 10, we use `required` keywords and `nullable` types for **actual** type safety. We use FluentValidation for **validation logic**. 
    *   **Why?** Keeping decorators off your DTOs means you can reuse them in different contexts (like a gRPC service or a background worker) without dragging along the ASP.NET Core validation baggage. If you want the "Gemini Hybrid" approach, you *can* use `[EmailAddress]` for quick checks, but our mission is **Industrial Scale**, which demands decoupled logic.

**Q: I want to ship it quick. Which one helps?**
*   **A:** **Decorators.** If your goal is "MVP in 10 minutes", use `[Required]` and `[EmailAddress]` on your DTOs. It’s built-in, and you don’t have to create extra files. Friday Tech will mock you for it, but Friday PM will love the delivery speed. We’ll offer both in the template: "Quick Start" with decorators, "Pro Mode" with Fluent.

**Q: Is it time-consuming to set up validators? (I had a bad time with Joi).**
*   **A:** **The Joi Comparison.** Joi (Node.js) can be verbose. In .NET, **FluentValidation** is significantly more streamlined. 
    *   **The Setup**: You write **one** "Behavior" (the Bouncer) for the whole app. It takes 2 minutes.
    *   **The Maintenance**: After that, every new validator is just 5-10 lines of code. 
    *   **The Middleware**: With decorators, you *still* need separate middleware for complex logic. With Fluent, your "middleware" (the Behavior) is universal—it just works for every request automatically. Faster shipping, better security.

**Q: So we make both DTOs... for request and for response?**
*   **A:** **YES.** It’s about **Symmetry and Privacy.**
    *   **The Request** (`CreateUserRequest`): Only contains what the user *can* send (e.g., `Password`). It never contains an `Id` because the server hasn't made it yet.
    *   **The Response** (`UserResponse`): contains what the user *should* see (e.g., `Id`, `CreatedAt`). It **never** contains the `Password`. 
    *   If you only use one DTO for both, you either accidentally ask the user for an `Id` they don't have, or you accidentally leak their `Password` back to the UI. Don't be that developer.

**Q: Where does 'Contract' fit in the DTO structure?**
*   **A:** **The Container.** "Contracts" is the name of the **folder** and **namespace**. 
    *   Folder: `src/Apeiron.Application/Contracts/`
    *   Content: Inside that folder, you have your `...Request` and `...Response` DTOs.
    *   Think of it like this: The **Contract** is the whole document; the **Requests** and **Responses** are the specific clauses inside it.

**Q: Is `Result.cs` our Response DTO?**
*   **A:** **The Envelope vs. The Letter.**
    *   **The DTO** (`ProjectResponse`): This is the **Letter**. It contains the actual data.
    *   **The Result** (`Result<ProjectResponse>`): This is the **Envelope**. It tells you if the letter arrived successfully or if there was an error. 
    *   In your API, you return the `Result` object. The frontend sees `{ isSuccess: true, value: { name: "Project A" } }`. The `value` is your DTO.

**Q: Why is `Error.None` static?**
*   **A:** **The Singleton of Silence.** 
    *   `Error.None` represents the state of "Nothing went wrong." 
    *   Instead of creating a new "Empty Error" object every single time a request succeeds (which wastes memory), we create **one** instance and mark it `static readonly`. 
    *   It’s a global reference that every success result can point to. It’s faster, cleaner, and uses less memory.

**Q: Isn't a static `Error.None` a race condition waiting to happen?**
*   **A:** **No, because of Immutability.** 
    *   **The Guard**: We use `static readonly`. This means the reference can never be changed after the app starts.
    *   **The Body**: We use a `record`. In C#, records are immutable by default. You can't change the `Code` or `Message` after creation.
    *   **The Result**: Since no thread can ever *write* to `Error.None`, and everyone is only *reading* the same data, there is zero risk of a race condition. It’s "Read-Only" for everyone, forever.

**Q: Why don't I need to define `Code` and `Message` properties in a record?**
*   **A:** **Magic of the Record.** When you write `public record Error(string Code, string Message)`, C# automatically creates:
    1. A constructor that takes `Code` and `Message`.
    2. Two public, read-only properties named `Code` and `Message`.
    3. Proper equality checks (two errors with the same code are considered equal).
    It’s the ultimate "Ship It Quick" feature for C# developers. Adding them manually is like putting a belt on someone who's already wearing suspenders.

**Q: Why are `Success` and `Failure` methods static?**
*   **A:** **The Factory Pattern.** We use static methods as "factories." Instead of the developer having to remember every constructor parameter, they just call `Result.Success()`. It makes the code read like a sentence.

**Q: What is a `record` exactly?**
*   **A:** **A Class for Data.** A `record` is a special C# type that is "Immutable" (cannot be changed after creation) and has "Value Equality" (if the data matches, the objects match). It’s perfect for DTOs and Errors where you only care about the values.

**Q: Why use `new` instead of `override` for static methods?**
*   **A:** **Static Shadowing.** You cannot `override` static methods in C# because they belong to the Class, not the Object. When `Result<T>` has its own `Failure` method, we use the `new` keyword to tell the compiler: "Hide the base version; when people use this typed class, use this version instead."

**Q: Why is only `Failure` using the `new` keyword? Why not `Success`?**
*   **A:** **Method Overloading vs. Hiding.** 
    *   **Success**: In the base class, it's `Success()`. In the generic class, it's `Success(T value)`. Because the inputs are different, C# treats them as **Overloads**. They can coexist peacefully.
    *   **Failure**: Both the base and generic classes have `Failure(Error error)`. Because the inputs are **identical**, the compiler sees a collision. The `new` keyword tells the compiler: "I know they have the same signature; use this one for the generic class."

**Q: Why `ProjectCreateRequest` instead of `CreateProjectRequest`?**
*   **A:** **Alphabetical Sanity.** If you have 50 models, you want `ProjectCreate`, `ProjectUpdate`, and `ProjectDelete` sitting together in the file explorer. If you start with verbs, your project files are scattered like trash in a windstorm. 10x devs don't hunt for files; they group them.

**Q: Seriously, why records?**
*   **A:** **Because writing Boilerplate is for Interns.** 
    *   A `class` needs: Properties, Private backing fields, A constructor, `Equals()` overrides, and `GetHashCode()`. That's 40 lines of noise.
    *   A `record` needs: **1 line.** 
    *   If you enjoy typing redundant code, go build a 1x CRUD app. In APEIRON, we use the compiler to do the grunt work.

**Q: Why is the method called `TryHandleAsync` and not something like `CatchAsync`?**
*   **A:** **The Chain of Responsibility.** 
    *   In modern .NET, you can have multiple exception handlers. 
    *   `TryHandleAsync` returns a `bool`. 
    *   If it returns `true`, it tells the system: "I handled this! Stop looking."
    *   If it returns `false`, it tells the system: "I don't know this error, let the next handler try."
    *   Naming it "Catch" would imply it's the end of the road. "TryHandle" acknowledges it's part of a sophisticated sequence.

**Q: What is `IQueryable<Project> Query()` and why is it in my Repository?**
*   **A:** **The "Double-Edged Sword".**
    *   **What it is**: It’s a pointer to the database that hasn’t run yet. It allows the Service to say `Query().Where(p => p.Name == "Apeiron")` and the database only fetches that one project.
    *   **The Benefit**: It's incredibly powerful for Paging and Sorting.
    *   **The Risk (The Leak)**: In strict Clean Architecture, `IQueryable` is a leak. It forces your Service to know about LINQ-to-SQL details and can cause "Lazy Loading" crashes if you aren't careful.
    *   **The Friday Rule**: Use `IQueryable` for complex Paging/Filtering, but ALWAYS materialize it (`.ToListAsync()`) before it leaves the Service. Never let an `IQueryable` reach the Controller.

**Q: What is the difference between a Service and a Repository?**
*   **A:** **The Chef vs. The Pantry.**
    *   **Repository (The Pantry)**: Its only job is to **get or store stuff**. It doesn't know *why*. It just knows how to talk to PostgreSQL. It has methods like `GetById`, `Add`, `Save`. No business logic allowed.
    *   **Service (The Chef)**: This is the **Brain**. It decides *what* to do. It checks if the user is allowed to delete a project, it hashes passwords, it sends emails, and it maps data to DTOs. It uses the Repository to get the raw ingredients.

**Q: Why do we return `IQueryable` from Infra instead of keeping the queries there?**
*   **A:** **Flexibility vs. Purity.** 
    *   If you put every single query in the Repository (e.g., `GetActiveProjectsWithNameStartingWithA`), your Repository will become a 5,000-line mess of specialized methods.
    *   By returning `IQueryable`, the Repository says: "Here is the raw data stream. You (the Service) decide how to filter it."
    *   **The Rule**: The **Infra** provides the *capability* to query, but the **Service** defines the *intent* of the query.

**Q: How does the "Grand Wiring" flow actually work?**
*   **A:** **The Handshake.** 
    1.  **Controller**: Receives a `ProjectCreateRequest`. Passes it to the Service.
    2.  **Service**: Maps the Request to a `Project` (Entity). Saves it via Repo.
    3.  **Service**: Maps the saved `Project` to a `ProjectResponse`. Returns `Result.Success(response)`.
    4.  **Controller**: Checks `result.IsSuccess`. If yes, returns `Ok(result.Value)`.
    *   **The Win**: The Controller never sees the Entity. The Service never sees the HTTP context. Everyone stays in their lane. 

**Q: So we don't have to wrap our call to service in `TryHandleAsync`?**
*   **A:** **NO.** It’s Automatic. 
    *   **The Result Pattern (Explicit)**: You use `if (result.IsSuccess)` for things you *expect* to happen (e.g., 404 Not Found, 400 Validation Error).
    *   **The Exception Handler (Implicit)**: If your database explodes, or a piece of code tries to divide by zero, that throws a raw `Exception`. You don't "catch" this in the controller. You let it "bubble up." 
    *   **The Middleware**: ASP.NET Core sees the uncaught exception, says "Oh no!", and automatically sends it to your `GlobalExceptionHandler.TryHandleAsync`. 
    *   **The Win**: Your controller remains clean of `try/catch` blocks. It only handles business results.

**Q: Why does the IDE show yellow lines for `user.FirstName` even after I check `if (user is null)`?**
*   **A:** **The Overprotective Compiler.** 
    *   In modern .NET, **Nullable Reference Types (NRTs)** are turned on. 
    *   If your `User` entity has `string? FirstName`, the compiler sees that '?' and worries that even if the `user` object exists, the `FirstName` field inside it might be null.
    *   **The Solution**: If you *know* that a field is required in your database (e.g., `FirstName` is never null for a registered user), use the **Null-Forgiving Operator** (`!`). 
    *   `user.FirstName!` tells the compiler: "Shh, I've checked the database schema, this will never be null here. Stop the warnings."

**Q: What does the Serilog configuration block actually do?**
*   **A:** **The "Brain" of your Logs.**
    *   **"Using"**: Tells Serilog which "Sinks" (Destinations) are active.
    *   **"MinimumLevel"**:
        *   `Default`: Sets the global noise level.
        *   `Override`: This is key. It tells Microsoft and System to **shut up** (only show Warnings) so your own Information logs aren't buried in framework junk.
    *   **"WriteTo"**:
        *   `Console`: Prints to terminal.
        *   `Seq`: Sends logs to your local Seq server for searching/graphing.
    *   **"Enrich"**: This turns a "String" into an **"Object"**. It automatically attaches the MachineName and ThreadId to every single log entry without you typing it.

**Q: We used `WriteTo.Console()` in Program.cs. Do we need to add `.WriteTo.Seq()` there too?**
*   **A:** **No, if you use "ReadFrom.Configuration".**
    *   **The Bootstrap Logger**: The code at the very top of `Program.cs` is just a temporary safety measure. It writes to the Console so you can see if the app crashes during the very first second of startup.
    *   **The Host Logger**: Once the `builder` is created, we tell Serilog to `ReadFrom.Configuration(context.Configuration)`. 
    *   **The Swap**: This command tells Serilog to **ignore** the hardcoded C# settings and instead look at your `appsettings.json`. Since you have both Console and Seq in your JSON, Serilog will automatically start writing to both. 
    *   **The Rule**: Keep the C# code minimal. Let the JSON do the heavy lifting.

**Q: Where is Dependency Injection set up and what is the Scope?**
*   **A:** **The Extension Method Pattern.**
    *   **The Setup**: We don't clutter `Program.cs`. Instead, each layer has a `DependencyInjection.cs` file.
        *   `Apeiron.Application` registers Services.
        *   `Apeiron.Infrastructure` registers Repositories and Database.
    *   **The Scope**: We use **`AddScoped`**.
        *   **Why?**: "Scoped" means "One instance per HTTP Request".
        *   This ensures that if you start a transaction in the Repository, the Service shares the exact same DB connection. It creates a "Bubble of State" for that one user's request, then destroys it when the request finishes.

**Q: What does the `this` keyword mean in `(this IServiceCollection services)`?**
*   **A:** **The Extension Method (The Glue).**
    *   **The Syntax**: Putting `this` before the first parameter of a `static` method tells the C# compiler: "Pretend this method belongs to `IServiceCollection`."
    *   **The Magic**: unique syntax allows you to write:
        *   `services.AddApplication()` (Fluent/Clean)
        *   Instead of: `DependencyInjection.AddApplication(services)` (Clumsy/Ugly).
    *   **The Goal**: It allows us to extend Microsoft's builtin types with our own APEIRON logic without hacking their source code.

**Q: What does `where TRequest : IRequest<TResponse>` mean in the ValidationBehavior?**
*   **A:** **The VIP List (Generic Constraints).**
    *   **The Syntax**: This line tells C#: "This Behavior implementation is **only** for classes that implement `IRequest`."
    *   **The Why**: We don't want to try and validate a random `string` or `int`. We only want to intercept **MediatR Requests**.
    *   **The `IPipelineBehavior`**: This interface is the "Middleman". It says "I sit between the Controller and the Handler."
    *   It allows us to run code **before** (Validation) and **after** (Logging) the main logic, without touching the Handler itself.

### 🔐 Security & Observability (Phase 2.5)
**Q: How do we update/change API Versioning?**
*   **A:** **The "V" Strategy.**
    *   **Config**: In `Program.cs`, we set `DefaultApiVersion` to 1.0.
    *   **Usage**: In `BaseApiController`, we use `[ApiVersion("1.0")]` and `[Route("api/v{version:apiVersion}/[controller]")]`.
    *   **To Change**: To allow `v2`, you simply add `[ApiVersion("2.0")]` to a new Controller. The system handles the routing automatically.

**Q: What is the `AuditingInterceptor`?**
*   **A:** **The Automatic Scribe.**
    *   **The Problem**: Manually setting `CreatedAt = DateTime.UtcNow` in every single Service method is tedious and error-prone.
    *   **The Solution**: An EF Core **Interceptor** that sits between your app and the database.
    *   **How it works**: When you call `SaveChanges()`, the Interceptor pauses the execution, scans every entity being saved, checks if it inherits from `BaseEntity`, and automatically injects the current UTC time into `CreatedAt` (for new rows) or `ModifiedAt` (for updates). It guarantees audit trails can never be forgotten.

**Q: How does the `JwtTokenGenerator` work?**
*   **A:** **The Digital Passport Printer.**
    *   **Symmetric Security**: It uses a secret key (from `appsettings.json`) that only the server knows. This key is like the "Government Seal."
    *   **The Claims**: It takes the User's ID and Email and stamps them into the token payload (Claims).
    *   **The Signature**: It hashes the data + the secret key using `HmacSha256`. 
    *   **The Result**: If a user tries to tamper with the token (e.g., change "User" to "Admin"), the hash won't match, and the API will reject it. It allows stateless auth—we don't need to check the DB for every request, we just check the signature.

**Q: Can we use Feature Flags for Caching/Auth?**
*   **A:** **Yes ("The Kill Switch").** 
    *   It is a good idea for local development speed.
    *   **How to**: Add `EnableAuth: false` in `appsettings.Development.json`. In `Program.cs`, wrap `app.UseAuthorization()` in an `if (config["EnableAuth"])` block.
    *   **Warning**: If you disable Auth locally, you risk shipping bugs where protected routes fail in production. Use with caution.

---
*Log generated by Friday Protocol.*



