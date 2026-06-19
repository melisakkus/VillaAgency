# 📑 PROJECT LOG: VillaAgency Project (.NET 8 & MongoDB)

Language Options: [🇬🇧 English](README.md) | [🇹🇷 Türkçe](README.tr.md)

---

## 📌 Project Overview & Goals
* **Purpose:** To develop a comprehensive Real Estate project by integrating a MongoDB database into a .NET 8 MVC application using N-Tier Architecture and the Repository Design Pattern.
* **Theme:** A villa-purchasing focused real estate theme integrated with an administrative management panel.
* **Core Paradigm:** Dependency Inversion Principle (DIP), strongly-typed configurations, and loose coupling.

---

## 🛠 Used Technologies & NuGet Packages

The project isolates dependencies within the layered architecture based on their specific responsibilities. The package manifest and its architectural purposes are listed below:

| Layer Name | Dependency / NuGet Package | Version | Purpose |
| :--- | :--- | :--- | :--- |
| **All Layers** | `.NET 8 SDK / runtime` | v8.0 | Core execution runtime infrastructure for the entire suite. |
| **VillaAgency.Entity** | `MongoDB.Bson` | v10.0.8 | Light structural library handling native MongoDB Binary JSON types, unique identity generation (`ObjectId`), and data metadata attributes. |
| **VillaAgency.DataAccess** | `MongoDB.Driver` | v3.9.0 | Main official driver to orchestrate connections, build queries, and run asynchronous CRUD operations. |
| **VillaAgency.DataAccess** | `Microsoft.Extensions.Configuration` | v10.0.8 | Infrastructure to read data settings from structural configuration files like `appsettings.json`. |
| **VillaAgency.DataAccess** | `Microsoft.Extensions.Configuration.Binder` | v10.0.8 | Object graph mapping framework to bind raw configuration keys directly into C# objects (`MongoDbSettings`). |
| **VillaAgency.DataAccess** | `Microsoft.Extensions.DependencyInjection` | v10.0.8 | IoC container extensions to register DataAccess services (Context, Repositories) into the runtime scope. |
| **VillaAgency.Business** | `Mapster` | v3.9.0 | Lightweight object mapping library used to transform DTO ↔ Entity models in the Business layer, ensuring clean separation between presentation contracts and domain entities. |
| **VillaAgency.WebUI** | `Microsoft.AspNetCore.Mvc` | Native | Core web infrastructure bringing the web interface, routing, administrative views, and Controllers to life. |

---

## 🌟 VALUE PROPOSITIONS & ARCHITECTURAL ADVANTAGES (Why This Design?)

This project is built from the ground up prioritizing architectural scalability and enterprise SOLID patterns over simple monolithic scripting. Reviewers should note the following advantages:

### 1. SUSTAINABLE AND SECURE CONNECTION LIFECYCLE
* **Advantage:** Database credentials, secrets, or cluster endpoints are never hardcoded inside compilation source files.
* **How It Works:** Connection metadata is safely retrieved out of `appsettings.json` and cleanly bound to a strongly-typed `MongoDbSettings` POCO class. This guarantees compile-time type safety and isolates connection lifecycle monitoring to a single operational configuration spot.

### 2. THE POWER OF CLEAN ARCHITECTURE: DATABASE AGNOSTICISM (Loose Coupling)
* **Advantage:** Although the stack starts on **MongoDB (NoSQL)**, the core application can seamlessly pivot to **PostgreSQL or MS SQL (Relational RDBMS)** tomorrow without breaking or changing a single line of business logic!
* **Architectural Blueprint (SOLID - Dependency Inversion):**
    * The `WebUI` (Presentation) and `Business` (Logic) layers remain completely oblivious to underlying MongoDB implementations. They only reference the abstract `IRepository<T>` contract.
    * If a database migration occurs, the `Business` and `WebUI` source code remains completely untouched. Engineers only write a brand new `EfRepository` under the `DataAccess` library and re-bind the container target mapped in `DataAccessServiceExtension.cs`.
    * This stands as a true textbook implementation of the **Dependency Inversion Principle (DIP)** (The **D** in SOLID) — the software ecosystem locks itself into abstract components, not concrete volatile data drivers.
 
### 3. ABSTRACTED PERFORMANCE CACHING & INVALIDATION (Infrastructure Decoupling)
* **Microsecond-Level Latency Reduction:** The highly trafficked villa/product listing (Index) page is served directly from the server's memory store via an optimized caching tier instead of querying the database on every single request. This maximizes page rendering speeds while mitigating the I/O and Read overhead on the MongoDB cluster to near-zero.
* **Technology-Agnostic Abstraction (SOLID Open/Closed Principle):** Instead of tightly coupling controllers to .NET's native IMemoryCache engine, the architecture introduces a custom ICacheService abstraction layer. This isolates the presentation tier from concrete infrastructure implementation. If system scaling demands a shift from volatile server RAM (In-Memory) to a distributed cluster like Redis, not a single line of controller code will change; engineers only need to inject a new runtime manager within the IoC container.
* **Defensive Cache Lookup & Strict Token Governance:** A defensive control workflow is enforced utilizing custom service wrapper methods. If the requested DTO collection exists within the memory store, the incoming request is resolved instantly at the Controller level. In the event of a cache miss, data is asynchronously fetched and committed using centrally managed configuration policies (30-minute absolute / 10-minute sliding expiration). Furthermore, inline "magic strings" are eliminated by organizing all cache tokens inside a single, centralized CacheKeys registry hub (DRY Principle).
* **Proactive State Invalidation & CQRS Real-Time Balance:** To guarantee absolute data consistency, a proactive state-purging strategy is implemented. While public UI components leverage read-only cache streams, administrative data manipulation operations (Create, Update, Delete) systematically trigger cache eviction pipelines via _cacheService.Remove(). This eliminates the risk of "dirty reads" and ensures 100% real-time synchronization between the persistent database and the cache layer.

---

## 📁 Project Directory Structural Blueprint (Solution Explorer)

```plaintext
VillaAgency (Solution)
├── 📄 global.json (Optional - SDK version locking)
│
├── 💻 VillaAgency.Entity (Class Library)
│       ├── 📁 Common
│       │   └── 📄 BaseEntity.cs (Shared identity property layout)
│       └── 📁 Entities
│           └── 📄 Banner.cs (City, Title, Image metadata model)
│
├── 💻 VillaAgency.DataAccess (Class Library)
│       ├── 📁 Abstract
│       │   └── 📄 IGenericDal.cs (Generic Repository Contract interface)
│       └── 📁 Concrete
│       │   └── 📁 MongoDb.Driver
│       │         └── 📄 GenericRepository.cs (Concrete MongoDB implementation)
│       ├── 📁 Configurations
│       │   └── 📄 MongoDbSettings.cs (Strongly-typed configuration mapper)
│       ├── 📁 Context
│       │   └── 📄 MongoDbContext.cs (Cluster Connection & Collection hub)
│       ├── 📁 Extensions
│       │   └── 📄 DataAccessServiceExtension.cs (Data Layer DI Container Manifest)

│
├── 💻 VillaAgency.Dto (Class Library)
│       └── 📁 BannerDtos (Data Transfer Object structural layouts)
│       │   └── 📄 CreateBannerDto.cs
│       │   └── 📄 ResultBannerDto.cs
│       │   └── 📄 UpdateBannerDto.cs
│
├── 💻 VillaAgency.Business (Class Library)
│       ├── 📁 Abstract (Business Domain Services - e.g., IBannerService)
│       │   └── 📄 IBannerService.cs
│       ├── 📁 Concrete (Domain implementation Logic Managers - e.g., BannerManager)
│           └── 📄 BannerManager.cs
│       └── 📁 Extensions
│           └── 📄 BusinessServiceExtension.cs (Layered DI chaining adapter)
│
└── 💻 VillaAgency.WebUI (ASP.NET Core MVC)
│       ├── 📁 Areas
│       │   └── 📁 Admin
│       │       └── 📁 Controllers
│       │           └── 📄 BannerController.cs
│       │       └── 📁 Data
│       │       └── 📁 Models
│       │       └── 📁 Views
│       │           └── 📁 Shared
│       │               └── 📄 _AdminLayout.cshtml
│       ├── 📁 Controllers
│       ├── 📁 Views
│       ├── 📄 appsettings.json (System configuration registry)
│       └── 📄 Program.cs (Application composition bootstrap file)

```

# 🚀 Chronological Walkthrough Lifecycle (Step-by-Step Development)
## 🟢 STEP 0 — Foundations (Entity & Dto Architecture Layers)
Development began on the bottom-most layer with zero outward dependencies: the Entity project.

- BaseEntity.cs Built: A parent structural blueprint initialized to enforce dry-coding principles across model records. Houses the global database driver identifier tracking layout (ObjectId).

```C#
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class BaseEntity
{
   public ObjectId Id { get; set; }
}
```

- Banner.cs Built: Created the inaugural storage-mapped data entity class by subclassing BaseEntity.

```C#
public class Banner : BaseEntity
{
    public string City { get; set; }
    public string Title { get; set; }
    public string Image { get; set; }
}
```

## 🔵 STEP 1 — Strong Configuration Scaffolding (DataAccess / MongoDbSettings.cs)
Constructed strongly-typed target classes to ingest raw configuration strings directly from JSON manifests, removing brittle hardcoded keys throughout system runtimes.

appsettings.json Content Layout:

```JSON
{
  "MongoDB": {
    "ConnectionString": "-",
    "DatabaseName": "VillaAgencyDb"
  }
}
```

MongoDbSettings.cs Class Layout:

```C#
public class MongoDbSettings
{
    public string ConnectionString { get; set; } ;
    public string DatabaseName { get; set; } ;
}
```

## 🔵 STEP 2 — Unified Cluster Gateway Context (DataAccess / MongoDbContext.cs)
Acting as the conceptual counterpart to Entity Framework Core's DbContext, this engine orchestrates pool channels, connections, and automated collection parsing inside one wrapper class.

``` C#
using Humanizer;
using MongoDB.Driver;
using VillaAgency.DataAccess.Configurations;

namespace VillaAgency.DataAccess.Context
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;
        public MongoDbContext(MongoDbSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            _database = client.GetDatabase(settings.DatabaseName);
        }

        public IMongoCollection<T> GetCollection<T>()
        {
            var collectionName = typeof(T).Name.Pluralize();
            return _database.GetCollection<T>(collectionName);
        }
    }
}
```

## 🔵 STEP 3 — The Abstract Service Contract (DataAccess / IGenericDal.cs)
To maintain pure system decoupling across persistent layers, a common transactional contract interface was drafted using the Dependency Inversion Principle (DIP). The upstream business tier binds exclusively to this abstract schema.

```C#
using MongoDB.Bson;
using System.Linq.Expressions;
using VillaAgency.Entity.Common;

namespace VillaAgency.DataAccess.Abstract
{
    public interface IGenericDal<T> where T : BaseEntity
    {
        Task CreateAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(ObjectId id);

        Task<List<T>> GetListAsync();
        Task<T> GetByIdAsync(ObjectId id);

        Task<int> CountAsync();

        Task<List<T>> GetFilteredListAsync(Expression<Func<T,bool>> predicate);
    }
}

```

## 🔵 STEP 4 — Concrete Persistence Logic (DataAccess / MongoRepository.cs)
The architectural bridge where the generic data contract interface is materialized utilizing official native MongoDB asynchronous driver extensions.

```C#
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;
using VillaAgency.DataAccess.Abstract;
using VillaAgency.DataAccess.Context;
using VillaAgency.Entity.Common;

namespace VillaAgency.DataAccess.Concrete.MongoDb.Driver
{
    public class GenericRepository<T> : IGenericDal<T> where T : BaseEntity
    {
        private readonly IMongoCollection<T> _collection;
        public GenericRepository(MongoDbContext context)
        {
            _collection = context.GetCollection<T>();
        }

        public async Task<int> CountAsync()
        {
            return (int)await _collection.CountDocumentsAsync(Builders<T>.Filter.Empty);
        }

        public async Task CreateAsync(T entity)
        {
            await _collection.InsertOneAsync(entity);
        }

        public async Task DeleteAsync(ObjectId id)
        {
            await _collection.DeleteOneAsync(x => x.Id == id);
        }

        public async Task<T> GetByIdAsync(ObjectId id)
        {
            var filter = Builders<T>.Filter.Eq("_id",id);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<List<T>> GetFilteredListAsync(Expression<Func<T, bool>> predicate)
        {
            return await _collection.Find(predicate).ToListAsync();
        }

        public async Task<List<T>> GetListAsync()
        {
            return await _collection.Find(Builders<T>.Filter.Empty).ToListAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            var id = entity.Id;
            await _collection.ReplaceOneAsync(x => x.Id == id, entity);
        }
    }
}
```

## 🔵 STEP 5 — Automated Registries (DataAccess / DataAccessServiceExtension.cs)
Encapsulated container composition graphs within tailored Extension Methods to retain cleanliness inside the core startup files, ensuring each layer retains ownership over its assembly needs.

``` C#
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VillaAgency.DataAccess.Abstract;
using VillaAgency.DataAccess.Configurations;
using VillaAgency.DataAccess.Context;
using VillaAgency.DataAccess.Repositories;

namespace VillaAgency.DataAccess.Extension
{
    public static class DataAccessServiceExtension
    {
        public static IServiceCollection AddDataAccessServices(
            this IServiceCollection services, IConfiguration config)
        {
            var mongoSettings = config.GetSection("MongoDB").Get<MongoDbSettings>();

            services.AddSingleton(mongoSettings);
            services.AddSingleton<MongoDbContext>();
            services.AddScoped(typeof(IGenericDal<>), typeof(GenericRepository<>));

            return services;
        }
    }
}
```

## 🔵 STEP 6 — Chained Dependency Wiring (Business / BusinessServiceExtension.cs)
Enforces layered dependency boundary protection rules: the presentation web views never establish direct reference channels onto persistence libraries. Presentation calls Business components, and the Business tier safely pipelines registration requirements down to DataAccess modules.

```C#
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VillaAgency.DataAccess.Extension;

namespace VillaAgency.Business.Extension
{
    public static class BusinessServiceExtension
    {
        public static IServiceCollection AddBusinessServices(
            this IServiceCollection services, IConfiguration config)
        {
            // 1. Chaining the persistent data layer registrations automatically
            services.AddDataAccessServices(config);

            // 2. Business domain managers register comfortably within this scope below
            services.AddScoped<IBannerService, BannerManager>();

            return services;
        }
    }
}
```

## 🔵 STEP 7 — Core Root Bootstrapping (WebUI / Program.cs)
The ultimate execution compilation entry point where a single declarative invocation instantly hooks the complete multi-tier composition map straight into the active application engine scope.

```C#
// Inside the WebUI Presentation root Program.cs file:
builder.Services.AddBusinessServices(builder.Configuration);

// Enlisting the framework runtime memory caching capability
builder.Services.AddMemoryCache();
```

## 🔵 STEP 8 — Entity-Specific Business Contract (Business / IBannerService.cs)
As the project evolved, the Business layer was redesigned to expose entity-specific service contracts rather than a fully generic abstraction. This approach enables the Business layer to work directly with DTOs, encapsulate validation rules, and prevent presentation layers from interacting with persistence entities.

The IBannerService contract exposes only DTO-based operations to external consumers while internally coordinating entity transformations.

```C#
using MongoDB.Bson;
using System.Linq.Expressions;
using VillaAgency.Dto.Banner;
using VillaAgency.Dto.BannerDtos;
using VillaAgency.Entity.Entities;

namespace VillaAgency.Business.Abstract
{
    public interface IBannerService
    {
        Task TCreateAsync(CreateBannerDto dto);
        Task TUpdateAsync(UpdateBannerDto dto);
        Task TDeleteAsync(ObjectId id);

        Task<List<ResultBannerDto>> TGetListAsync();
        Task<ResultBannerDto> TGetByIdAsync(string id);

        Task<int> TCountAsync();

        Task<List<ResultBannerDto>> TGetFilteredListAsync(Expression<Func<Banner, bool>> predicate);
    }
}

```
### 🎯 Why Entity-Specific Services?

* **Public Contract:** DTOs become the public contract of the Business layer.
* **Domain Isolation:** Entities remain isolated inside the domain and persistence layers.
* **Encapsulated Rules:** Validation and business rules can be introduced without affecting Controllers.
* **Independent Evolution:** Different entities can evolve independently without forcing generic abstractions everywhere.
* **Clean Architecture:** Supports Clean Architecture principles by preventing UI $\rightarrow$ Entity coupling.


## 🔵 STEP 9 — Business Logic Implementation & DTO Mapping (Business / BannerManager.cs)

### ⚙️ BannerManager & Business Workflow

The `BannerManager` class represents the real business workflow implementation. Unlike the previous generic manager approach, this class now handles the complete lifecycle of a request:

1. **Receive:** Accepts DTOs directly from the Presentation Layer.
2. **Validate:** Performs strict validation and safety guard checks.
3. **Map:** Converts DTOs $\leftrightarrow$ Entities seamlessly using **Mapster**.
4. **Persist:** Communicates safely with the DataAccess layer through abstractions.
5. **Return:** Sends clean DTOs back to the Presentation layer.

> 🔒 **Key Benefit:** This strict lifecycle ensures that persistence models never leak outside the Business layer.


```C#
using Mapster;
using MongoDB.Bson;
using System.Linq.Expressions;
using VillaAgency.Business.Abstract;
using VillaAgency.DataAccess.Abstract;
using VillaAgency.Dto.Banner;
using VillaAgency.Dto.BannerDtos;
using VillaAgency.Entity.Entities;

namespace VillaAgency.Business.Concrete
{
    public class BannerManager : IBannerService
    {
        private readonly IGenericDal<Banner> _genericDal;

        public BannerManager(IGenericDal<Banner> genericDal)
        {
            _genericDal = genericDal
                ?? throw new ArgumentNullException(nameof(genericDal));
        }

        public async Task TCreateAsync(CreateBannerDto dto)
        {
            if (dto is null)
                throw new ArgumentNullException(nameof(dto));

            var entity = dto.Adapt<Banner>();

            await _genericDal.CreateAsync(entity);
        }

        public async Task<List<ResultBannerDto>> TGetListAsync()
        {
            var entities = await _genericDal.GetListAsync();

            return entities.Adapt<List<ResultBannerDto>>();
        }

        public async Task<ResultBannerDto> TGetByIdAsync(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                throw new FormatException("Invalid ObjectId format.");

            var entity = await _genericDal.GetByIdAsync(objectId);

            return entity.Adapt<ResultBannerDto>();
        }

        // Other methods omitted for brevity...
    }
}
```
## 🔵 STEP 10 — Registering Entity-Specific Services (Business / BusinessServiceExtension.cs)
With the introduction of entity-specific service contracts, the Dependency Injection container is updated to register concrete business managers individually.
```C#
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VillaAgency.Business.Abstract;
using VillaAgency.Business.Concrete;
using VillaAgency.DataAccess.Extension;

namespace VillaAgency.Business.Extension
{
    public static class BusinessServiceExtension
    {
        public static IServiceCollection AddBusinessServices(
            this IServiceCollection services,
            IConfiguration config)
        {
            services.AddDataAccessServices(config);

            services.AddScoped<IBannerService, BannerManager>();

            return services;
        }
    }
}
```
## 🔵 STEP 11 — UI Layer Implementation (WebUI)
### 🔄 Parallel Development Model
The UI layer has been developed simultaneously with the DataAccess and Business layers. Instead of waiting for all backend layers to be fully completed, a modular (Feature-Driven) development approach has been followed.

**Example Process (Banner Module):**
* **DataAccess (DAL):** Database entities and abstractions for the module were created.
* **Business (BL):** Business logic, validations, and DTO structures were implemented.
* **UI / Presentation:** At the same time, listing and view structures were designed and developed.

> 💡 **Continuous Feedback:** During this process, layers were not static; they were continuously improved, tested, and evolved in a flexible feedback loop, supporting each other.

## 🔵 STEP 12 — Areas Structure Implementation (Admin Panel)
In the project, the user interface (Frontend) and administration panel (Admin) are separated using the ASP.NET Core Areas structure. This design ensures that the Admin section is isolated and implemented as a modular subsystem independent from the main application.
### 🎯 Architectural Goals
The main purpose of this structure is to achieve a sustainable and scalable architecture:
* **Separation of Concerns:** Clear separation between the user interface and the admin panel
* **Modularity & Scalability:** A modular structure that allows easy integration of new features as the project grows
* **Independent Administration:** The admin panel is isolated from the main application and managed as a standalone subsystem
### 📁 Implementations
* Areas/Admin structure was created
* Controllers and Views for the Admin panel were set up
* A shared layout file _AdminLayout.cshtml was created
* A consistent UI structure was established for admin pages

## 🔵 STEP 13 — Banner Controller (Admin Panel)
The BannerController was created as the foundation of the first CRUD structure within the admin panel.
```c#
using Microsoft.AspNetCore.Mvc;
using VillaAgency.Business.Abstract;

namespace VillaAgency.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BannerController : Controller
    {
        private readonly IBannerService _bannerService;

        public BannerController(IBannerService bannerService)
        {
            _bannerService = bannerService;
        }

        public async Task<IActionResult> Index()
        {
            var values = await _bannerService.TGetListAsync();
            return View(values);
        }
    }
}
```

## 🔵 STEP 14 — Product Controller & Core UI Performance Caching (Main UI Scope)
The application handles product operations using a highly optimized caching strategy designed to prevent database roundtrips, reduce latency, and enforce loose coupling across infrastructure layers.

Instead of injecting the native .NET IMemoryCache framework directly into multiple controllers, this system architecture introduces a custom abstract ICacheService layer combined with a centralized CacheKeys constant hub.

### ⚙️ Why This Caching Architecture? (Value Propositions & Benefits)
- Enterprise-Grade Loose Coupling (Abstraction Over Technology): Controllers interact solely with our custom ICacheService interface contracts. They remain completely agnostic of the underlying caching platform. If system scale dictates migrating from local volatile server memory (In-Memory) to a distributed, persistent session cluster like Redis, not a single line of controller code will change. The engineering team would only inject a new RedisCacheManager implementation inside the IoC Container (Open/Closed Principle).
- Prevention of Code Duplication (DRY Principle): By migrating cache key identifiers from hardcoded inline "magic strings" inside individual controllers into a centralized VillaAgency.Business.Constants.CacheKeys static hub, key changes are governed from a single source of truth.
- Read-Heavy vs. Write-Heavy Domain Partitioning:
    * Main UI Scope (Read-Heavy): The public user interface runs a defensive TryGet lookup strategy. If data is warm, it bypasses the entire Business and DataAccess pipelines completely, reducing query resolution speeds down to microseconds. Cold cache states trigger an asynchronous fetch, storing DTO responses under a strict 30-minute absolute / 10-minute sliding policy threshold.
   * Admin Scope (Write-Heavy): The administration panel bypasses cache checks during list fetches to guarantee the admin always observes real-time database reality. However, the exact moment a state mutability operation occurs (Create, Update, Delete), it fires an active Cache Invalidation (Eviction) pipeline via _cacheService.Remove(), purging stale data fragments and preventing "dirty reads" globally.

#### 💻 Production Source Blueprint Manifest
##### 1. Caching Contracts & Centralized Key Registry

```c#
namespace VillaAgency.Business.Abstract
{
    public interface ICacheService
    {
        bool TryGet<T>(string cacheKey, out T value);
        void Set<T>(string cacheKey, T value, TimeSpan absoluteExpiration, TimeSpan slidingExpiration);
        void Remove(string cacheKey);
    }
}
```

```c#
namespace VillaAgency.Business.Constants
{
    public static class CacheKeys
    {
        public const string ProductsUiCacheKey = "products_ui_cache_key";
    }
}
```

```c#
using Microsoft.Extensions.Caching.Memory;
using System;
using VillaAgency.Business.Abstract;

namespace VillaAgency.Business.Concrete
{
    public class CacheManager : ICacheService
    {
        private readonly IMemoryCache _memoryCache;

        public CacheManager(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        public bool TryGet<T>(string cacheKey, out T value)
        {
            return _memoryCache.TryGetValue(cacheKey, out value);
        }

        public void Set<T>(string cacheKey, T value, TimeSpan absoluteExpiration, TimeSpan slidingExpiration)
        {
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpiration,
                SlidingExpiration = slidingExpiration
            };

            _memoryCache.Set(cacheKey, value, cacheOptions);
        }

        public void Remove(string cacheKey)
        {
            _memoryCache.Remove(cacheKey);
        }
    }
}
```
##### 2. Public Presentation Layer Implementation (Read-Heavy)
```c#
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VillaAgency.Business.Abstract;
using VillaAgency.Business.Constants;
using VillaAgency.Dto.ProductDtos;

namespace VillaAgency.WebUI.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICacheService _cacheService;

        public ProductController(IProductService productService, ICacheService cacheService)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        }

        // DEFENSIVE READ LOGIC (Microsecond Latency Response)
        public async Task<IActionResult> Index()
        {
            if (!_cacheService.TryGet(CacheKeys.ProductsUiCacheKey, out List<ResultProductDto> cachedProducts))
            {
                cachedProducts = await _productService.TGetListAsync();

                _cacheService.Set(
                    CacheKeys.ProductsUiCacheKey, 
                    cachedProducts, 
                    TimeSpan.FromMinutes(30), 
                    TimeSpan.FromMinutes(10)
                );
            }
            return View(cachedProducts);
        }
    }
}
```
##### 3. Administrative Panel Management Implementation (Write-Heavy / State Invalidation)
```c#
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System;
using System.Threading.Tasks;
using VillaAgency.Business.Abstract;
using VillaAgency.Business.Constants;
using VillaAgency.Dto.ProductDtos;

namespace VillaAgency.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICacheService _cacheService;

        public ProductController(IProductService productService, ICacheService cacheService)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        }

        // REAL-TIME READ (Bypasses caching to ensure data management accuracy)
        public async Task<IActionResult> Index()
        {
            var products = await _productService.TGetListAsync();
            return View(products);
        }

        public IActionResult Create() => View();

        // WRITE & CACHE INVALIDATION
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductDto dto)
        {
            await _productService.TCreateAsync(dto);
            _cacheService.Remove(CacheKeys.ProductsUiCacheKey);
            return RedirectToAction(nameof(Index));
        }

        // DELETE & CACHE INVALIDATION
        public async Task<IActionResult> Delete(ObjectId id)
        {
            await _productService.TDeleteAsync(id);
            _cacheService.Remove(CacheKeys.ProductsUiCacheKey);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(ObjectId id)
        {
            var value = await _productService.TGetByIdAsync(id);
            return View(value);
        }

        // UPDATE & CACHE INVALIDATION
        [HttpPost]
        public async Task<IActionResult> Update(UpdateProductDto dto)
        {
            await _productService.TUpdateAsync(dto);
            _cacheService.Remove(CacheKeys.ProductsUiCacheKey);
            return RedirectToAction(nameof(Index));
        }
    }
}
```
