# CLAUDE.md

# Approval Policy

Before making any modification to the codebase:

- Never apply changes automatically, even if they seem trivial.
- Always present a plan before making changes.
- Wait for an explicit "Apply", "Go ahead", or equivalent confirmation from me.

---

Behavioral guidelines to reduce common LLM coding mistakes. Merge with project-specific instructions as needed.

**Tradeoff:** These guidelines bias toward caution over speed. For trivial tasks, use judgment.

## 1. Think Before Coding

**Don't assume. Don't hide confusion. Surface tradeoffs.**

Before implementing:
- State your assumptions explicitly. If uncertain, ask.
- If multiple interpretations exist, present them - don't pick silently.
- If a simpler approach exists, say so. Push back when warranted.
- If something is unclear, stop. Name what's confusing. Ask.

## 2. Simplicity First

**Minimum code that solves the problem. Nothing speculative.**

- No features beyond what was asked.
- No abstractions for single-use code.
- No "flexibility" or "configurability" that wasn't requested.
- No error handling for impossible scenarios.
- If you write 200 lines and it could be 50, rewrite it.

Ask yourself: "Would a senior engineer say this is overcomplicated?" If yes, simplify.

## 3. Surgical Changes

**Touch only what you must. Clean up only your own mess.**

When editing existing code:
- Don't "improve" adjacent code, comments, or formatting.
- Don't refactor things that aren't broken.
- Match existing style, even if you'd do it differently.
- If you notice unrelated dead code, mention it - don't delete it.

When your changes create orphans:
- Remove imports/variables/functions that YOUR changes made unused.
- Don't remove pre-existing dead code unless asked.

The test: Every changed line should trace directly to the user's request.

## 4. Goal-Driven Execution

**Define success criteria. Loop until verified.**

Transform tasks into verifiable goals:
- "Add validation" → "Write tests for invalid inputs, then make them pass"
- "Fix the bug" → "Write a test that reproduces it, then make it pass"
- "Refactor X" → "Ensure tests pass before and after"

For multi-step tasks, state a brief plan:
```
1. [Step] → verify: [check]
2. [Step] → verify: [check]
3. [Step] → verify: [check]
```

Strong success criteria let you loop independently. Weak criteria ("make it work") require constant clarification.

---

**These guidelines are working if:** fewer unnecessary changes in diffs, fewer rewrites due to overcomplication, and clarifying questions come before implementation rather than after mistakes.


---

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

VillaAgency is a .NET 8 ASP.NET Core MVC real estate application backed by MongoDB, built with N-Tier architecture (Entity → DataAccess → Business → Dto → WebUI) and the Repository pattern. It has a public-facing villa/product catalog site and an isolated Admin area (ASP.NET Core Areas) for content management, protected by ASP.NET Core Identity (MongoDB-backed).

## Commands

The solution file is `VillaAgency.UI/VillaAgency.slnx` (there is no `.sln` at the repo root).

```
dotnet build VillaAgency.UI/VillaAgency.slnx
dotnet run --project VillaAgency.UI/VillaAgency.WebUI.csproj
```

There is no test project in this repository — do not assume a test command exists.

MongoDB connection settings live in `VillaAgency.UI/appsettings.json` under `MongoDB:ConnectionString` / `MongoDB:DatabaseName`. Note the checked-in `appsettings.json` currently contains live-looking Atlas credentials; treat any change to this file with care and avoid re-committing real secrets.

Default seeded admin login (created at startup in `Program.cs` if missing): `admin@villaagency.com` / `admin00`.

## Architecture

### Layer flow and dependency direction

```
VillaAgency.Entity  (no project deps)
VillaAgency.Dto     (no project deps)
VillaAgency.DataAccess  → Dto, Entity
VillaAgency.Business    → DataAccess, Dto
VillaAgency.UI (WebUI)  → Business only
```

Each layer only registers its own DI services via an `Add*Services(IServiceCollection, IConfiguration)` extension method, and chains downward: `Program.cs` calls `AddBusinessServices`, which calls `AddDataAccessServices` + `AddIdentityServices` internally (`VillaAgency.Business/Extensions/BusinessServiceExtension.cs`). Controllers never reference `DataAccess` or `Entity` types directly except where an entity type is needed for a `Expression<Func<TEntity,bool>>` filter predicate passed into a service method — DTOs are otherwise the only cross-layer contract exposed to `WebUI`.

### Repository pattern: generic vs. dedicated DAL

`BaseEntity` (`VillaAgency.Entity/Common/BaseEntity.cs`) stores `Id` as a `string` (`[BsonRepresentation(BsonType.ObjectId)]` over a Mongo `ObjectId`) — do not change entities to use `ObjectId` directly, DTOs/views expect `string`.

`GenericRepository<T>` (`VillaAgency.DataAccess/Concrete/MongoDb.Driver/Common/GenericRepository.cs`) implements `IGenericDal<T>` (CRUD + `GetFilteredListAsync` with optional paging) against a Mongo collection named by pluralizing `typeof(T).Name` (via `MongoDbContext.GetCollection<T>()`).

Two patterns coexist depending on whether an entity needs behavior beyond generic CRUD:
- **Generic-only** entities (e.g. Banner, Contact, Feature, Question) are injected directly as `IGenericDal<TEntity>` into their Business manager.
- **Entity-specific DAL** entities (Product, Message, Video, Counter) get their own `I<Entity>Dal : IGenericDal<TEntity>` interface plus a `<Entity>Dal : GenericRepository<TEntity>, I<Entity>Dal` concrete class exposing extra queries (e.g. `IProductDal.GetUniqueCategoriesAsync`, `ChangeStatusAsync`, `GetRandomProductPerCategoryAsync` using Mongo aggregation pipelines). Both interface and concrete class must be registered in `DataAccessServiceExtension.cs`.

When adding a new entity, default to the generic-only pattern unless you need queries the generic repository can't express.

### Business layer

Each entity has an `I<Entity>Service` (`VillaAgency.Business/Abstract`) exposing only DTO-typed methods (`T`-prefixed: `TCreateAsync`, `TUpdateAsync`, `TDeleteAsync`, `TGetListAsync`, `TGetByIdAsync`, `TGetFilteredListAsync`, ...), implemented by a `<Entity>Manager` (`VillaAgency.Business/Concrete`). Managers:
- Map DTO ↔ Entity with **Mapster** (`dto.Adapt<Entity>()` / `entity.Adapt<Dto>()`); `TypeAdapterConfig.GlobalSettings.Default.IgnoreNullValues(true)` is set globally in `Program.cs` so partial update DTOs don't null out existing fields.
- Guard against null args with `ArgumentNullException` and throw `KeyNotFoundException` when an update/delete target doesn't exist.
- Log via injected `ILogger<T>` (Serilog is the provider, configured in `Program.cs`/`appsettings.json`, writing to console + rolling files under `VillaAgency.UI/Logs/`).
- Register new managers/services in `BusinessServiceExtension.cs`.

Validation is done with **FluentValidation** (`VillaAgency.Business/Validators/<Entity>Validators/`), auto-registered via `AddValidatorsFromAssemblyContaining<CreateBannerValidator>()` and wired into MVC via `AddFluentValidationAutoValidation()` in `Program.cs`. Controllers check `ModelState.IsValid` after posting, following the FluentValidation-generated errors.

`ICacheService`/`CacheManager` (`VillaAgency.Business/Abstract/Concrete`) is an in-memory cache abstraction (`Microsoft.Extensions.Caching.Memory`) with keys centralized in `CacheKeys` — but its DI registration is currently commented out in `BusinessServiceExtension.cs`, so it is not active. Don't assume caching is live without checking that registration.

`ExceptionMiddleware`/`ErrorDetails` (`VillaAgency.Business/Middleware_Reference/`) is a reference-only global exception handler for a future Web API split; the running MVC app uses `app.UseExceptionHandler("/Home/Error")` instead, so this middleware is not wired into the pipeline.

### Identity / Auth

ASP.NET Core Identity is backed by MongoDB via `AspNetCore.Identity.MongoDbCore` (`AppUser : MongoIdentityUser<string>`, `AppRole`, roles in `VillaAgency.Entity/Identity/Constants/Roles.cs`), configured in `IdentityServiceExtension.cs` (`VillaAgency.DataAccess/Extensions`). `IAuthService`/`AuthManager` wraps `SignInManager`/`UserManager` for login/logout, called from the non-Area `AccountController`. Cookie paths (`/Account/Login`, `/Account/AccessDenied`) are set in `Program.cs`.

### Admin Area

`VillaAgency.UI/Areas/Admin/Controllers/AdminBaseController.cs` is decorated `[Area("Admin")] [Authorize]`; all other Admin controllers (Product, Banner, Message, Contact, Video, Question, Feature, User, Dashboard) inherit from it to get area routing + auth enforced in one place rather than repeating attributes. Admin `Index` actions read live from the database (no caching) so admins always see current state; write actions (Create/Update/Delete/ChangeStatus) call the manager then redirect back to `Index`, generally preserving filter state (category/status/page) via route values.

### Public UI

Public-facing pages use `ViewComponent`s under `VillaAgency.UI/ViewComponents/UILayout/` (e.g. `_DefaultProducts`, `_DefaultBanner`, `_DefaultFeature`) to compose the homepage from independent, service-backed sections, separate from the `Controllers/Default*Controller.cs` classes that handle catalog listing/detail pages.
