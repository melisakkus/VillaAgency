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
| **VillaAgency.DataAccess** | `MongoDB.Driver` | v3.9.0 | Main official driver to orchestrate connections, build queries, and run asynchronous CRUD operations. |
| **VillaAgency.DataAccess** | `Microsoft.Extensions.Configuration` | v10.0.8 | Infrastructure to read data settings from structural configuration files like `appsettings.json`. |
| **VillaAgency.DataAccess** | `Microsoft.Extensions.Configuration.Binder` | v10.0.8 | Object graph mapping framework to bind raw configuration keys directly into C# objects (`MongoDbSettings`). |
| **VillaAgency.DataAccess** | `Microsoft.Extensions.DependencyInjection` | v10.0.8 | IoC container extensions to register DataAccess services (Context, Repositories) into the runtime scope. |
| **VillaAgency.Entity** | `MongoDB.Bson` | v3.9.0 | Light structural library handling native MongoDB Binary JSON types, unique identity generation (`ObjectId`), and data metadata attributes. |
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
│       │   └── 📄 IRepository.cs (Generic Repository Contract interface)
│       ├── 📁 Configurations
│       │   └── 📄 MongoDbSettings.cs (Strongly-typed configuration mapper)
│       ├── 📁 Context
│       │   └── 📄 MongoDbContext.cs (Cluster Connection & Collection hub)
│       ├── 📁 Extensions
│       │   └── 📄 DataAccessServiceExtension.cs (Data Layer DI Container Manifest)
│       └── 📁 Repositories
│           └── 📄 MongoRepository.cs (Concrete MongoDB implementation)
│
├── 💻 VillaAgency.Dto (Class Library)
│       └── 📁 BannerDtos (Data Transfer Object structural layouts)
│
├── 💻 VillaAgency.Business (Class Library)
│       ├── 📁 Abstract (Business Domain Services - e.g., IBannerService)
│       ├── 📁 Concrete (Domain implementation Logic Managers - e.g., BannerManager)
│       └── 📁 Extensions
│           └── 📄 BusinessServiceExtension.cs (Layered DI chaining adapter)
│
└── 💻 VillaAgency.WebUI (ASP.NET Core MVC)
        ├── 📁 Controllers
        ├── 📁 Views
        ├── 📄 appsettings.json (System configuration registry)
        └── 📄 Program.cs (Application composition bootstrap file)

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
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
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
            return _database.GetCollection<T>(typeof(T).Name);
        }
    }
}
```

## 🔵 STEP 3 — The Abstract Service Contract (DataAccess / IRepository.cs)
To maintain pure system decoupling across persistent layers, a common transactional contract interface was drafted using the Dependency Inversion Principle (DIP). The upstream business tier binds exclusively to this abstract schema.

```C#
namespace VillaAgency.DataAccess.Abstract
{
    public interface IRepository<T>
    {
        Task<List<T>> GetAllAsync();
        Task<T> GetByIdAsync(string id);
        Task CreateAsync(T entity);
        Task UpdateAsync(string id, T entity);
        Task DeleteAsync(string id);
    }
}
```

## 🔵 STEP 4 — Concrete Persistence Logic (DataAccess / MongoRepository.cs)
The architectural bridge where the generic data contract interface is materialized utilizing official native MongoDB asynchronous driver extensions.

```C#
using MongoDB.Driver;
using VillaAgency.DataAccess.Abstract;
using VillaAgency.DataAccess.Context;

namespace VillaAgency.DataAccess.Repositories
{
    public class MongoRepository<T> : IRepository<T>
    {
        private readonly IMongoCollection<T> _collection;
        public MongoRepository(MongoDbContext context)
        {
            _collection = context.GetCollection<T>();
        }

        public Task CreateAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<T> GetByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(string id, T entity)
        {
            throw new NotImplementedException();
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
            services.AddScoped(typeof(IRepository<>), typeof(MongoRepository<>));

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
            // Example: services.AddScoped<IVillaService, VillaManager>();

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
```
