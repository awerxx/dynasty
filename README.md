# wszystko-app

**Dynasty** is the application; **Carrington** is its set of finance modules. Each module is
independent today (monorepo) and is intended to become a separate microservice later, so
modules never reference each other.

## Modules

- **Blake** — Blazor (Server) admin UI: accounts management and other admin tasks.
- **Linda** — ASP.NET Core WebAPI backend for individual users.
- **Alexis** — Next.js UI ([src/Dynasty.Carrington.Alexis](src/Dynasty.Carrington.Alexis)) that talks to Linda (expenses etc.).

## Layout & conventions

Each .NET module follows clean architecture with its own layer projects:

```
Dynasty.Carrington.<Module>.Domain          ← entities, no dependencies (except Core)
Dynasty.Carrington.<Module>.Application     ← use cases, references Domain
Dynasty.Carrington.<Module>.Infrastructure  ← persistence/integrations, references Application
Dynasty.Carrington.<Module>.Web / .Api      ← host & composition root
```

`Dynasty.Carrington.Core` is a small shared kernel (base types/abstractions) referenced by the
Domain layers; when modules split into services it becomes a NuGet package. `Dynasty.Utils` is a
general-purpose utility library.

Common build settings (`net10.0`, nullable, implicit usings) live in `Directory.Build.props`.
Tests are xUnit projects under `tests/`, one per module. The solution is `Dynasty.slnx` at the
repo root.

```bash
dotnet build Dynasty.slnx
dotnet test
```
