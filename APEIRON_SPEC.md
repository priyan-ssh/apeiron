# 📜 APEIRON: THE EVERYTHING MANIFESTO

This document defines the technical scope, features, and architecture of Project APEIRON.

## 🛡️ Backend (.NET 10) - The "Engine Room"
### Core & Architecture
- **Pattern:** Clean Architecture (`Api` -> `Application` -> `Infrastructure` -> `Domain`).
- **Startup:** Clean `Program.cs` using ServiceCollection extensions.
- **Dependency Injection:** Strict Lifetime management (Scoped for Services/Db, Singleton for Caching).

### Database & Data Access
- **ORM:** EF Core 10 with `IQueryable` pattern.
- **Migrations:** Auto-migration on startup (configurable via flag).
- **Seeding:** `DataSeeder` for Roles/Users/Lookups.
- **Auditing:** `SaveChangesInterceptor` to auto-fill `CreatedAt`, `ModifiedAt`, `CreatedBy`.

### API Surface
- **Documentation:** Swagger / OpenAPI with JWT support.
- **Versioning:** API Versioning enabled (e.g., `/api/v1/...`).
- **Response Wrapper:** Standard `Result<T>` envelope for consistent JSON responses.
- **Exception Handling:** Global `IExceptionHandler` implementation (ProblemDetails standard).

### Operational Essentials
- **Health Checks:** `/health` (UI), `/health/ready` (DB+Redis), `/health/live`.
- **Logging:** Serilog (Console, Rolling File, OpenTelemetry).
- **Testing:** xUnit + FluentAssertions + Testcontainers (Postgres).

### Security
- **Auth:** JWT Bearer Auth + Refresh Tokens.
- **Identity:** ASP.NET Core Identity (Headless).
- **Rate Limiting:** Built-in .NET middleware.

## 💎 Frontend (React 19) - The "Control Panel"
### Build & DevEx
- **Vite 6:** Optimized build, path aliases.
- **Env:** Strict Typing via Zod.
- **Linting:** ESLint + Prettier + Oxlint + Husky hooks.

### Networking & State
- **Axios:** Global error handling, Auth injection, Silent Refresh logic.
- **TanStack Query:** Global config, DevTools.

### UI Components
- **Layouts:** Auth (Center Card), Dashboard (Sidebar + Topbar).
- **Theme:** Dark/Light mode (LocalStorage persistent).
- **Feedback:** Sonner/React-Hot-Toast, Skeleton screens.

## 🏗️ Infrastructure & DevOps - The "Glue"
### Docker Strategy
- **Backend:** Multi-stage builds using **.NET Chiseled Ubuntu** images.
- **Frontend:** Node build -> Nginx Alpine serve (SPA Fallback).

### Orchestration
- **Services:** API (.NET), Web (React), Postgres (Persistent), Redis (HybridCache), Seq (Logs).
- **MailDev:** Local email catching.

### Generator Logic
- **DNA Replacement:** Recursive find/replace of namespaces and project names.
- **Feature Flags:** Toggle modules (Auth, Caching, Logging) on/off during generation.

---

## 🗺 Architecture Map

```text
/apeiron-generator           <-- ROOT
├── /cli                     <-- Node.js Generator Logic
│   └── index.js             <-- The Forge script
│
├── /templates               <-- The Blueprints
│   ├── /frontend            <-- UI Layer
│   │   └── /react           <-- React 19 + Vite 6
│   │       ├── vite.config.ts
│   │       ├── tailwind.config.ts
│   │       └── src/
│   │           ├── components/
│   │           ├── routes/
│   │           └── hooks/
│   │
│   ├── /backend             <-- Data Layer
│   │   └── /dotnet          <-- .NET 10 Clean Architecture
│   │       ├── Apeiron.sln
│   │       └── src/
│   │           ├── Apeiron.Api
│   │           ├── Apeiron.Application         <-- Business Logic
│   │           ├── Apeiron.Domain              <-- Core Entities
│   │           └── Apeiron.Infrastructure      <-- DB/Ext Services
│   │
│   └── /devops              <-- Infrastructure
│       ├── docker-compose.yml
│       ├── Dockerfile.backend
│       └── Dockerfile.frontend
```

