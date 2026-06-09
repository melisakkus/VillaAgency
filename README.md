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
│
├── 💻 VillaAgency.Business (Class Library)
│       ├── 📁 Abstract (Business Domain Services - e.g., IBannerService)
│       │   └── 📄 IGenericService.cs 
│       ├── 📁 Concrete (Domain implementation Logic Managers - e.g., BannerManager)
│           └── 📄 GenericManager.cs 
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
        Task<T> GetByIdAsync(string id);

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

        public async Task<T> GetByIdAsync(string id)
        {
            var filter = Builders<T>.Filter.Eq("_id",new ObjectId(id));
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

## 🔵 STEP 8 — Business Logic Contract (Business / IGenericService.cs)
Just as IGenericDal<T> in the DataAccess layer exposes its own contract, the Business layer exposes its contract to the outside world via the IGenericService<T> interface. The "T" prefix on method names emphasizes that these methods belong to the Business layer and prevents confusion between layers.
```C#
using MongoDB.Bson;
using System.Linq.Expressions;
using VillaAgency.Entity.Common;

namespace VillaAgency.Business.Abstract
{
    public interface IGenericService<T> where T : BaseEntity
    {
        Task TCreateAsync(T entity);
        Task TUpdateAsync(T entity);
        Task TDeleteAsync(ObjectId id);

        Task<List<T>> TGetListAsync();
        Task<T> TGetByIdAsync(string id);

        Task<int> TCountAsync();

        Task<List<T>> TGetFilteredListAsync(Expression<Func<T, bool>> predicate);
    }
}
```

## 🔵 STEP 9 — Business Logic Concrete Class (Business / GenericManager.cs)
This is the concrete implementation of the IGenericService<T> interface. For now, it delegates directly to IGenericDal<T> methods. The existence of this intermediate layer is critical because business rules such as validation, logging, and caching will be added here as the project grows, without ever touching the DataAccess layer.
```C#
using MongoDB.Bson;
using System.Linq.Expressions;
using VillaAgency.Business.Abstract;
using VillaAgency.DataAccess.Abstract;
using VillaAgency.Entity.Common;

namespace VillaAgency.Business.Concrete
{
    public class GenericManager<T> : IGenericService<T> where T : BaseEntity
    {
        private readonly IGenericDal<T> _genericDal;

        public GenericManager(IGenericDal<T> genericDal)
        {
            _genericDal = genericDal;
        }

        public async Task<int> TCountAsync()
        {
            return await _genericDal.CountAsync();
        }

        public async Task TCreateAsync(T entity)
        {
            await _genericDal.CreateAsync(entity);
        }

        public async Task TDeleteAsync(ObjectId id)
        {
            await _genericDal.DeleteAsync(id);
        }

        public Task<T> TGetByIdAsync(string id)
        {
            return _genericDal.GetByIdAsync(id);
        }

        public Task<List<T>> TGetFilteredListAsync(Expression<Func<T, bool>> predicate)
        {
            return _genericDal.GetFilteredListAsync(predicate);
        }

        public Task<List<T>> TGetListAsync()
        {
            return _genericDal.GetListAsync();
        }

        public async Task TUpdateAsync(T entity)
        {
            await _genericDal.UpdateAsync(entity);
        }
    }
}
```
## 🔵 STEP 10 — Updating Business Dependency Registrations (Business / BusinessServiceExtension.cs)
The BusinessServiceExtension.cs that was written as a skeleton in Step 6 is now completed. The IGenericService<T> ↔ GenericManager<T> mapping is registered in the DI Container, closing the layer chain.
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
            this IServiceCollection services, IConfiguration config)
        {
            // 1. Chain-register the DataAccess layer's services first
            services.AddDataAccessServices(config);

            // 2. Register the Business layer's generic service mapping
            services.AddScoped(typeof(IGenericService<>), typeof(GenericManager<>));

            return services;
        }
    }
}
```

