# VillaAgency — Architecture and Design Decisions

This document explains VillaAgency's layer architecture, data access strategy, and the rationale behind the design decisions made during development, in detail. For the project's general introduction and feature list, see the [README](../README.md) file at the repository root.

Every heading here describes a concrete choice that has a counterpart in the code; it contains no hypothetical or unimplemented feature.

## Table of Contents

- [VillaAgency — Architecture and Design Decisions](#villaagency--architecture-and-design-decisions)
  - [Table of Contents](#table-of-contents)
  - [Architectural Approach](#architectural-approach)
  - [Dependency Inventory (NuGet Packages)](#dependency-inventory-nuget-packages)
  - [Data Access Layer: Combining Generic and Entity-Specific Repositories](#data-access-layer-combining-generic-and-entity-specific-repositories)
  - [Business Layer: A Deliberate Move Away from the Generic Manager](#business-layer-a-deliberate-move-away-from-the-generic-manager)
  - [Validation: FluentValidation](#validation-fluentvalidation)
  - [Logging: Structured Logging with Serilog](#logging-structured-logging-with-serilog)
  - [Centralized Error Handling](#centralized-error-handling)
  - [Authentication and Role-Based Authorization](#authentication-and-role-based-authorization)
  - [The Pagination Choice: Why Pagination Instead of Caching?](#the-pagination-choice-why-pagination-instead-of-caching)
  - [Scalability and Maintainability](#scalability-and-maintainability)
  - [Known Limitations](#known-limitations)

---

## Architectural Approach

The project is split into five separate class libraries, and dependency always flows in one direction:

```
VillaAgency.Entity        (no dependencies)
VillaAgency.Dto           (no dependencies)
VillaAgency.DataAccess    → Entity, Dto
VillaAgency.Business      → DataAccess, Dto
VillaAgency.UI (WebUI)    → Business
```

`WebUI` never references `DataAccess` or `Entity` projects directly; only service interfaces from the `Business` layer (`IProductService`, `IBannerService`, etc.) are injected into Controllers. The one exception is where filter predicates (`Expression<Func<TEntity, bool>>`) need to be passed as parameters to service methods — in that case the Entity type is visible inside the Controller, but the Entity is never passed to the View; everything that reaches a View is a DTO.

DI registrations are chained in a way that mirrors this layering: each layer registers its own services through its own `Add*Services()` extension method, and calls the registration method of a lower layer from within its own.

```
Program.cs
  └─ AddBusinessServices(config)
       ├─ AddDataAccessServices(config)   // repository + MongoContext registrations
       ├─ AddIdentityServices(config)     // Identity + Mongo store registrations
       └─ (Business services + FluentValidation registrations)
```

What this achieves is simple: `Program.cs` sets up the entire dependency tree with a single top-level call (`AddBusinessServices`); each layer carries its own dependency-registration responsibility within its own project. If a `VillaAgency.Api` project is ever added, it only needs to call its own `Add*Services()` method without touching the existing chain.

---

## Dependency Inventory (NuGet Packages)

Each layer references only the packages it needs for its own responsibility; there is no unnecessary dependency leaking into a layer above it.

| Layer Name | Dependency / NuGet Package | Version | Purpose |
| :--- | :--- | :--- | :--- |
| **General** | .NET 8 SDK | v8.0 | The overall runtime and infrastructure the project runs on. |
| **VillaAgency.Entity** | MongoDB.Bson | v3.9.0 | Makes model classes compliant with MongoDB conventions (`BsonId`, `BsonRepresentation`). |
| **VillaAgency.Entity** | AspNetCore.Identity.MongoDbCore | v7.0.0 | Lets `AppUser`/`AppRole` Identity models be defined directly in this layer in a MongoDB-compatible way. |
| **VillaAgency.Dto** | MongoDB.Bson | v3.9.0 | Allows `ObjectId`/Bson types to be carried in DTOs where needed. |
| **VillaAgency.DataAccess** | MongoDB.Driver | v3.9.0 | Database interaction, aggregation pipelines, and asynchronous CRUD operations. |
| **VillaAgency.DataAccess** | AspNetCore.Identity.MongoDbCore | v7.0.0 | Lets the ASP.NET Core Identity infrastructure run natively on MongoDB (without requiring EF Core). |
| **VillaAgency.DataAccess** | Humanizer.Core | v3.0.10 | Produces readable strings in collection/name conversions (e.g. singular→plural). |
| **VillaAgency.DataAccess** | Microsoft.Extensions.Configuration | v10.0.8 | Reading and managing configuration data from `appsettings.json`. |
| **VillaAgency.DataAccess** | Microsoft.Extensions.Configuration.Binder | v10.0.8 | Binds raw configuration values to strongly-typed (`MongoDbSettings`) C# objects. |
| **VillaAgency.DataAccess** | Microsoft.Extensions.DependencyInjection | v10.0.8 | Registering Repository and Context classes into the application's DI pool. |
| **VillaAgency.Business** | Mapster | v10.0.8 | Low-cost object mapping between the Entity and DTO layers. |
| **VillaAgency.Business** | FluentValidation | v12.1.1 | Data integrity and validation rules in the business logic layer. |
| **VillaAgency.Business** | FluentValidation.DependencyInjectionExtensions | v12.1.1 | Automatic registration of module-based validator classes into the IoC container. |
| **VillaAgency.Business** | Microsoft.AspNetCore.Http.Abstractions | v2.3.11 | Lets the Business layer work with HTTP-context-bound operations (e.g. file uploads) without being tightly coupled to MVC. |
| **VillaAgency.Business** | Microsoft.Extensions.Caching.Abstractions | v10.0.9 | The infrastructure behind the `ICacheService` abstraction (see [The Pagination Choice](#the-pagination-choice-why-pagination-instead-of-caching); currently inactive). |
| **VillaAgency.WebUI** | FluentValidation.AspNetCore | v11.3.1 | Automatic integration of FluentValidation rules with `ModelState`. |
| **VillaAgency.WebUI** | Serilog.AspNetCore | v10.0.0 | Structured logging and centralized error-tracing infrastructure. |
| **VillaAgency.WebUI** | Serilog.Sinks.Console | v6.1.1 | Writing logs to the console in the development environment. |
| **VillaAgency.WebUI** | Serilog.Sinks.File | v7.0.0 | Writing logs to daily rolling physical files (`Logs/log-.txt`). |
| **VillaAgency.WebUI** | Microsoft.VisualStudio.Web.CodeGeneration.Design | v8.0.23 | Automatic generation of MVC Controller/View scaffolding. |

**Notes:**

- **Isolation:** The `VillaAgency.WebUI` project never references the `DataAccess` or `Entity` layers directly. The UI layer is completely abstracted away from database details.
- **DTO Strategy:** The `MongoDB.Bson` dependency in the `VillaAgency.Dto` layer exists solely to preserve database identifiers (`ObjectId`).

---

## Data Access Layer: Combining Generic and Entity-Specific Repositories

`BaseEntity`, the common ancestor of all entities, deliberately keeps the `Id` field as `string` rather than MongoDB's native `ObjectId` type:

```csharp
public class BaseEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
}
```

The reasoning is this: `ObjectId` is a type that depends on MongoDB.Bson. Letting that type leak into the Business and UI layers (route parameters, form fields, DTOs) would mean those layers become dependent on MongoDB. Using `string` together with `[BsonRepresentation(BsonType.ObjectId)]` performs this conversion automatically at the database level, keeping the upper layers unaware of the infrastructure detail.

`GenericRepository<T>` implements the `IGenericDal<T>` contract, consolidating CRUD operations (`CreateAsync`, `UpdateAsync`, `DeleteAsync`, `GetListAsync`, `GetByIdAsync`, and the paginated `GetFilteredListAsync`) in one place. The collection name is derived automatically via `typeof(T).Name.Pluralize()` (Humanizer); the collection name is never defined manually when a new entity is added.

However, not every entity has the same needs, so two strategies coexist in the project:

- **Entities that are purely generic** (Banner, Contact, Feature, Question) get no separate repository written for them; the Business layer injects `IGenericDal<TEntity>` directly.
- **Entities with an entity-specific DAL** (Product, Message, Video, Counter) define their own interface (`I<Entity>Dal : IGenericDal<TEntity>`) for queries the generic repository cannot satisfy. For example:
  - `IProductDal.GetRandomProductPerCategoryAsync()` — a per-category random selection for the homepage's featured listings, via a `$sample` → `$group` → `$slice` aggregation pipeline
  - `IProductDal.ChangeStatusAsync(id, status)` — a partial update that changes only the `Status` and `UpdatedDate` fields rather than the entire document
  - `IMessageDal.MarkAsReadAsync / MarkAsDeletedAsync` — changing a message's state via a single-field update
  - `ICounterDal.GetProductCountsByCategoryAsync()` — category-based counting for the dashboard via a `$group` aggregation

The default approach when adding a new entity is generic-only; an entity-specific DAL is only opened up once a genuine need arises. This avoids creating an unnecessary interface/class pair for every entity.

---

## Business Layer: A Deliberate Move Away from the Generic Manager

Once the generic repository pattern proved itself in the DataAccess layer, the natural next step that came to mind was carrying the same approach into the Business layer — `IGenericService<TEntity, TResultDto, TCreateDto, TUpdateDto>` and a corresponding `GenericManager<...>`. This approach was tried, but abandoned for three concrete reasons:

1. **Domain leakage.** When a generic service is injected into a Controller, the entity type (`IGenericService<Banner, ...>`) inevitably becomes visible in the Controller's signature. This makes the presentation layer directly aware of database entities, breaking the isolation between layers.
2. **Risk of Single Responsibility violation.** The Business layer doesn't just move data — it also holds business rules. When an entity-specific behavior needs to be added (e.g. setting `IsRead = false` and `MessageDate = DateTime.UtcNow` when a new message is created), a generic structure is forced to get cluttered with `if/else` blocks.
3. **Generic type bloat.** An interface carrying four different generic parameters makes both DI registrations and method signatures harder to read.

A hybrid model was applied instead: while the generic repository is kept in DataAccess, each entity in the Business layer has its own interface (`IBannerService`) and its own manager class (`BannerManager`). These classes perform the DTO ↔ Entity conversion with Mapster, throw `ArgumentNullException` for `null` parameters and `KeyNotFoundException` for records that aren't found, and log operations with `ILogger<T>`. The amount of code written increases somewhat compared to the generic approach, but in return the presentation layer stays fully isolated from the database, and each entity has an extensible foundation for its own rules.

Mapster is configured globally via `TypeAdapterConfig.GlobalSettings.Default.IgnoreNullValues(true)` (`Program.cs`). The purpose of this setting is to prevent fields left `null` (unfilled) in an Update DTO, in partial-update scenarios, from overwriting the existing valid data on the target entity — when only a few fields are changed in a form, the other fields are thereby prevented from unintentionally dropping to `null`.

---

## Validation: FluentValidation

Validation rules are collected under `Business/Validators/<Entity>Validators/` and registered automatically via assembly scanning (`AddValidatorsFromAssemblyContaining<CreateBannerValidator>()`); no manual DI registration is needed when a new validator is added. It's wired into the MVC pipeline via `AddFluentValidationAutoValidation()`, and Controllers only perform the standard `ModelState.IsValid` check.

Rather than writing separate rules for Create and Update DTOs, common rules are collected in a `BaseProductValidator<T> : AbstractValidator<T> where T : BaseProductDto`; `CreateProductValidator` and `UpdateProductValidator` derive from this class and add only their own specific extra rules. Conditional validations are also present among the rules — for example, the `Area` field must be either `0` (unspecified) or greater than `10` (`.Must(area => area == 0 || area >= 10)`), which guarantees that optional numeric fields fall within a meaningful range only when they are actually filled in.

The `SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true` setting in `Program.cs` disables the implicit `[Required]` validation that .NET automatically applies to non-nullable reference types; this way validation responsibility is fully consolidated in FluentValidation, and a clash between two different validation sources is avoided.

---

## Logging: Structured Logging with Serilog

Serilog was preferred over .NET's built-in logging infrastructure for two reasons: configuration can be managed entirely through `appsettings.json` (`ReadFrom.Configuration()`), and messages (`"Banner created successfully. Id: {Id}"`) can be logged as structured fields rather than plain text — this makes it possible to filter by fields like `{Id}` or `{Title}` if a log management tool (Seq, Elasticsearch, etc.) is adopted later.

Logs are written simultaneously to the Console and to daily rolling files (`Logs/log-.txt`, 30-day retention). Framework logs coming from `Microsoft`/`System` are restricted to the `Warning` level — this only silences low-level noise; an `Error` entry is always logged regardless of its source. `app.UseSerilogRequestLogging()` automatically logs every HTTP request (path, status code, duration); `Log.CloseAndFlush`, hooked into the `app.Lifetime.ApplicationStopping` event, prevents logs still buffered in memory from being lost when the application shuts down.

Manager classes in the Business layer log successful operations with `LogInformation`, and unexpected-but-not-an-error situations (such as `entity is null`) with `LogWarning`. The `LogError` level is **deliberately never produced** in the Business layer: errors propagate upward without being swallowed by a try-catch, and are logged exactly once, only at the centralized error-handling point. Otherwise, the same error would be logged twice — once in the Business layer and once in the central handler — which bloats log files and makes incident analysis harder.

---

## Centralized Error Handling

The application runs with `app.UseExceptionHandler("/Home/Error")`: any exception left uncaught at any point in the pipeline redirects the request to the `HomeController.Error` action. This action retrieves the original exception and its path via `IExceptionHandlerPathFeature`, logs it — stack trace included — from a single point, and shows the user a fixed, safe message (`ErrorViewModel.UserMessage`) without leaking technical details.

`Error.cshtml` is deliberately rendered as a standalone `<html>` document with `Layout = null`; this way, even if the site's overall template is in a broken state, the user always encounters a consistent error screen. The page offers a different return link depending on session state: "Back to Dashboard" for a logged-in user, "Back to Home" for an anonymous visitor.

The mechanism is fed from a single centralized point, but the user experience differs depending on the type of request. When an error occurs in the admin panel, the browser is redirected to the `/Home/Error` page. On the public side, however, in forms submitted via AJAX — such as the contact form — a server error returns with a 500 status code, jQuery routes this to the `error` callback, the page never reloads, and the user is shown only an inline warning (SweetAlert) — the error is communicated without interrupting the form-filling flow.

The project also contains a complete `ExceptionMiddleware` class under the `Middleware_Reference` folder that returns errors in JSON format based on exception type (`FluentValidation.ValidationException` → 400, others → 500), but this class is not registered with DI and has no effect at runtime. The reason is simple: this is an MVC application, and the user should get a View, not JSON; `UseExceptionHandler` already covers that need. The class is kept as a reference in case a Web API layer is added to the project in the future — in that scenario, a JSON-returning error middleware would be directly useful.

---

## Authentication and Role-Based Authorization

Authentication is built on MongoDB via `AspNetCore.Identity.MongoDbCore` (`AppUser : MongoIdentityUser<string>`, `AppRole`). The reasoning is simple: the project already runs on MongoDB end-to-end, so opening up a separate relational database just for user management would be an unnecessary dependency. The session cookie is customized via `ConfigureApplicationCookie`: an unauthenticated request is redirected to `/Account/Login`, an unauthorized request to `/Account/AccessDenied`; the cookie is valid for 7 days and, with `SlidingExpiration = true`, that period is renewed on every request.

Rather than a single generic "admin user," the panel is built on a two-tier permission model resembling a real corporate structure: an **Admin** can open **Manager** accounts with limited privileges beneath them.

| Area | Admin | Manager |
|---|---|---|
| Dashboard | View | View |
| Product Management | Full CRUD + status change | Full CRUD + status change |
| Message Management | Full CRUD + mark as read/deleted | Full CRUD + mark as read/deleted |
| Banner | Full CRUD | No access |
| Contact Information | Full CRUD | No access |
| Feature Section (Feature/FAQ) | Full CRUD | No access |
| Question & Answer | Full CRUD | No access |
| Video | Full CRUD | No access |
| User Management | Full CRUD | No access |

This separation is implemented by adding `[Authorize(Roles = Roles.Admin)]` to Admin-only controllers (`BannerController`, `ContactController`, `FeatureController`, `QuestionController`, `VideoController`, `UserController`), on top of `AdminBaseController` (`[Area("Admin")] [Authorize]`), the common ancestor of all Admin controllers in the code. `ProductController`, `MessageController`, and `DashboardController`, on the other hand, carry only the base authorization; these three form the operational part of the panel and let a Manager do their daily work (entering listings, replying to messages) without needing an Admin.

One detail that reinforces this hierarchy: `UserController.Create` always registers new accounts opened through the panel with a fixed `Roles.Manager`. A new Admin account cannot be opened from the panel — Manager accounts are dependent accounts that can only be created by an existing Admin.

In the login flow, `AuthManager.LoginAsync` distinguishes username from email using the same field (by checking for an `@` character), and enables Identity's `MaxFailedAccessAttempts = 5` / `10-minute` lockout policy via `lockoutOnFailure: true`. The `user.IsActive` field is also checked; a deactivated account cannot log in even with the correct password — this lets an Admin temporarily suspend a Manager's access without deleting the account. When the application first starts up, the roles and a first Admin account are automatically created using the information defined in the `AdminUser` section of `appsettings.json`; no manual database entry is needed when deploying to a new environment. Having the seed information (username, email, password, full name) read from configuration instead of being hardcoded in `Program.cs` allows a different initial admin account to be defined across different environments (Development/Staging/Production) without changing code.

---

## The Pagination Choice: Why Pagination Instead of Caching?

Early in development, an `ICacheService`/`CacheManager` abstraction wrapping `IMemoryCache` had been set up for product listing. As the project progressed, the product listing need was solved with server-side pagination and filtering (`GetFilteredListAsync(predicate, page, pageSize)` → MongoDB `Skip`/`Limit`) instead. With this change, every request now fetches only a small subset belonging to the relevant page from the database, rather than the entire collection.

At this point, the value of caching was questioned: since the data volume returned per query was already small, the performance gain caching would provide didn't offset the extra overhead of cache invalidation complexity — especially since the need to keep the cache in sync during the frequent create/update/delete/status-change operations on the Admin side loses its meaning once the data is paginated. For this reason, the caching approach was deliberately abandoned in favor of the simpler pagination architecture that better fits the current use case.

The `ICacheService` and `CacheManager` classes were not deleted after this decision. They are zero-cost at runtime since they are not registered in the DI container and are called from nowhere, but there's a reason they remain in the code: the services were already designed to depend on the `ICacheService` interface rather than `IMemoryCache`. If traffic ever grows to a size that requires caching again, the system can scale without changing a single line on the Controller/Service side, simply by turning on `CacheManager`'s DI registration (or replacing it with a distributed `RedisCacheManager`). The commented-out registration line in `BusinessServiceExtension.cs` shows that this code is not forgotten but is a deliberately kept-in-waiting architectural choice.

Pagination works with the same contract (`TGetFilteredListAsync(predicate, page, pageSize)`) on both the Admin and Public sides; the total page count is calculated with a separate counting query via `ICounterService`. In the message inbox, this principle is taken one step further: each of the "All / Unread / Deleted" tabs keeps its own page number independently, and only the data of the currently active tab is fetched from the database — the three lists are not needlessly queried on every request.

---

## Scalability and Maintainability

Since the Business and WebUI layers depend only on the `IGenericDal<T>` / `I<Entity>Dal` interfaces, if a move away from MongoDB to another data source is ever needed, that change only requires rewriting the implementations in the DataAccess layer; the Business and WebUI code doesn't change. Pagination ensures that list pages perform a fixed-cost read regardless of data size. As data volume grows, defining a compound index on the MongoDB side for the fields `GetFilteredListAsync` filters on (`Status`, `Category`, `IsDeleted`) is the next step that will come up — there is currently no explicit index on these fields.

On the maintainability side, every entity follows the same folder pattern (`Dto/<Entity>Dtos`, `Business/Validators/<Entity>Validators`, `Business/Abstract`+`Concrete`) and the same naming convention (`Create<Entity>Dto`, `I<Entity>Service`, `<Entity>Manager`); this lets a developer who is new to the project understand the others in the same way once they've worked out one entity's structure. Role names (`Roles.Admin`, `Roles.Manager`) are kept in a single constants class and are not repeated as magic strings in controllers. The steps required when adding a new entity are clear and not scattered: a repository registration in DataAccess, a service registration in Business; validator registration is automatic thanks to assembly scanning.

---

## Known Limitations

This section exists to transparently lay out the cost of the choices made, rather than to make the project look better than it is:

- **There is no test project.** Validation has so far been done manually; the lack of automated protection for business rules against regressions could pose a risk in a growing codebase down the line.
- **`ICacheService` is not active.** As justified above, this is a deliberate choice, but it hasn't yet been verified that it would work smoothly under real conditions once activated.
- **`ExceptionMiddleware` is for reference only**, and has not been tested under real conditions before a Web API layer is added.

---

This document has been prepared staying strictly faithful to the project's current codebase; no behavior or decision described here is based on an assumption that has no counterpart in the code.