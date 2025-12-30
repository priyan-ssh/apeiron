# 🚀 APEIRON

APEIRON (Greek: The Infinite/Boundless) is an industrial-grade "Construction Kit" that spawns production-ready stacks (React 19 + .NET 10 Clean Architecture) instantly.

## ⚙️ How it Operates

The APEIRON system works by using a recursive template engine to "eject" a perfectly configured monorepo based on your requirements.

1.  **Selection:** You choose your stack (React + .NET by default).
2.  **DNA Injection:** The CLI recursively renames namespaces, projects, and configurations to match your project name.
3.  **Modular Setup:** Feature flags enable or disable specific modules like Authentication, Caching, and Observability.
4.  **Zero-Day Ready:** You get a `docker-compose.yml` that stands up the entire grid (App, DB, Redis, Logs) immediately.


## 🛑 The "Day 2" Problem

Simple templates are great for **Day 1** (getting something on screen).
They fail on **Day 2** when you need to debug a crash, speed up a query, or secure an endpoint.

**APEIRON** solves these "Day 2" problems **before you even start**.
You won't have to refactor in a month when your app grows.

## 🔍 Detailed Specification

For a full breakdown of the architecture, tech stack, and operational features, see [APEIRON_SPEC.md](./APEIRON_SPEC.md).

## 📦 Installation & Usage

```bash
git clone https://github.com/priyan-ssh/apeiron.git
cd apeiron
npm install
node cli/index.js my-app
```
