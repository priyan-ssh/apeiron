# 🚀 APEIRON: The Monorepo Generator

> **Status:** Phase 1 (CLI Active)

APEIRON (Greek: The Infinite/Boundless) is an industrial-grade "Construction Kit" that spawns production-ready stacks (React + .NET Clean Architecture) instantly.

**The Directive:** Kill redundancy. Config is pain. We do not waste time setting up Webpack/Vite or Docker manually. The generated code is zero-day ready and deployable immediately.

## 🛠 Tech Stack (Mission Assets)

### The "Generator" (The Engine)
*   **Runtime:** Node.js (CLI Script).
*   **Arsenal:** `commander`, `prompts`, `fs-extra`, `picocolors`.
*   **Logic:** Recursive directory copying, Dockerfile injection, git history sanitization.

### Template A: The Frontend (The Interface)
*   **Framework:** React 19 (Cutting Edge).
*   **Build:** Vite 6.
*   **Language:** TypeScript (Strict Mode).
*   **Routing:** TanStack Router (Mandatory. File-based, Type-safe).
*   **State:** TanStack Query.
*   **Styling:** Tailwind CSS v4 (CSS-first config).
*   **QA:** Vitest + React Testing Library.

### Template B: The Backend (The Core)
*   **Framework:** .NET 9.
*   **Architecture:** Clean Architecture (Simplified). API → Application → Infrastructure → Domain.
*   **Database:** PostgreSQL (via EF Core 9 Code First).
*   **Logic Rule:** Use `IQueryable` in Services. No Stored Procedures.
*   **Caching:** HybridCache (Redis + In-Memory L1/L2).
*   **Observability:** OpenTelemetry + Aspire Dashboard.

### Infrastructure (The Grid)
*   **Docker:** Multi-stage builds.
*   **Orchestration:** `docker-compose.yml` linking App, DB, Redis, and Aspire.

## 🗺 Architecture Map

```text
/apeiron-generator           <-- ROOT
├── /cli                     <-- The Generator Logic
│   └── index.js             <-- The "Create-Apeiron" Script
│
├── /templates               <-- The "Blueprints"
│   ├── /frontend            <-- UI LAYER
│   │   └── /react           <-- 💎 REACT 19 STACK
│   │       ├── vite.config.ts
│   │       ├── tailwind.config.ts
│   │       └── src/
│   │
│   ├── /backend             <-- DATA LAYER
│   │   └── /dotnet          <-- 🛡️ .NET 9 STACK
│   │       ├── Apeiron.sln
│   │       └── src/
│   │           ├── Apeiron.Api
│   │           ├── Apeiron.Application
│   │           ├── Apeiron.Domain
│   │           └── Apeiron.Infrastructure
│   │
│   └── /devops              <-- 🏗️ INFRA ASSETS
│       ├── dotnet-compose.yml
│       └── Dockerfile.postgres
```

## 📦 Installation & Usage

```bash
# Clone the repo
git clone <your-repo-url>
cd apeiron

# Install dependencies
npm install

# Run the generator locally
node cli/index.js my-new-app
```

## 🧪 Testing

```bash
npm test
```
