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

---
*Log generated by Friday Protocol.*



