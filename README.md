# 🚀 APEIRON

**APEIRON** (Greek: *The Infinite / Boundless*) is a production-ready template generator that spawns **React 19 + .NET 10** stack designed for the long haul.

## 🛡️ The "Day 2" Guarantee

Most boilerplates are designed for **Day 1**: getting "Hello World" on the screen. They fail on **Day 2**, when you need to add a real feature, debug a production crash, or handle scale.

**APEIRON** is engineered to solve the headaches that usually hit you two weeks into development.

### 1. Architecture That Doesn't Rot
**The Trap:** Putting business logic in Controllers or mixing DB calls with UI code. It works for a todo list, but breaks at enterprise scale.

**The Apeiron Fix:** Enforced Clean Architecture.
*   **Domain:** Pure C# logic. No dependencies.
*   **Application:** Use Cases (CQRS via MediatR).
*   **Infrastructure:** EF Core, Redis, External Services (pluggable).
*   **API:** Thin presentation layer.

### 2. "It Works on My Machine" is Dead
**The Trap:** You spend 4 hours debugging why the local DB connection string doesn't work in the container.

**The Apeiron Fix:** Zero-Config Infrastructure.
*   Includes a battle-tested `docker-compose.yml`.
*   Spins up **PostgreSQL**, **Redis**, and **Seq/Jaeger** (Observability) with correct networking and environment variables pre-wired.
*   `npm run up` is all you need.

### 3. Observability Built-In, Not Bolted On
**The Trap:** The app crashes in production and you have to grep through text files to find out why.

**The Apeiron Fix:** Structured Logging & Tracing standard.
*   **Serilog** pre-configured for structured JSON logs.
*   **OpenTelemetry** hooks ready for distributed tracing.
*   Correlation IDs flow from **React → API → Database** automatically.

### 4. Enterprise Rigor
*   **Validation:** FluentValidation pipeline behaviors wired up automatically.
*   **Error Handling:** Global Exception Middleware that returns standard **RFC 7807 Problem Details** (no more 500 Internal Server Error with no context).
*   **Testing:** xUnit + Moq projects created alongside source code, preventing the "I'll add tests later" excuse.

---

## ⚙️ How it Operates

The APEIRON system works by using a recursive template engine to "eject" a perfectly configured monorepo based on your requirements.

1.  **Selection:** You choose your stack (React + .NET by default).
2.  **Scaffolding:** The CLI recursively renames namespaces, projects, and configurations to match your project name.
3.  **Modular Setup:** Feature flags enable or disable specific modules like Authentication, Caching, and Observability.
4.  **Zero-Day Ready:** You get a `docker-compose.yml` that stands up the entire grid (App, DB, Redis, Logs) immediately.

---

## 📦 Installation & Usage

```bash
git clone https://github.com/priyan-ssh/apeiron.git
cd apeiron
npm install
npm link
apeiron                 # Launch Main Menu
apeiron init            # Launch Wizard immediately
```

## 🔍 Detailed Specification

For a full breakdown of the architecture, tech stack, and operational features, see [APEIRON_SPEC.md](./APEIRON_SPEC.md).
