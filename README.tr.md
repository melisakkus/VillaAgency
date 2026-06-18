# 📑 PROJE GÜNLÜĞÜ: VillaAgency Projesi (.NET 8 & MongoDB)

Dil Seçenekleri : [🇬🇧 English](README.md) | [🇹🇷 Türkçe](README.tr.md)

---

## 📌 Proje Özeti & Hedefler
* **Amaç:** MongoDB veritabanını, çok katmanlı mimari (N-Tier Architecture) ve Repository Tasarım Deseni (Repository Design Pattern) kullanarak .NET 8 MVC projesine entegre etmek.
* **Tema:** Villa satın alma odaklı emlak platformu ve buna bağlı bir Admin Paneli.
* **Temel Yaklaşım:** Bağımlılıklerin tersine çevrilmesi (Dependency Inversion), tip güvenliği (Strongly-typed configurations) ve gevşek bağlılık (Loose Coupling).

---

## 🛠 Kullanılan Teknolojiler & NuGet Paketleri

Projenin katmanlı mimari yapısında, her katmanın sorumluluğuna göre NuGet üzerinden bağımlılıklar (Packages) yüklenmiştir. Görsel mimaride yer alan güncel paket listesi ve kullanım amaçları şu şekildedir:

| Katman Adı | Bağımlılık / NuGet Paketi | Sürüm | Amacı |
| :--- | :--- | :--- | :--- |
| **Bütün Katmanlar** | `.NET 8 SDK / runtime` | v8.0 | Projenin genel çalışma zamanı (runtime) altyapısı. |
| **VillaAgency.Entity** | `MongoDB.Bson` | v3.9.0 | MongoDB'ye özgü veri formatı olan BSON (Binary JSON) nesne kimliklerini (`ObjectId`) ve veritabanı niteliklerini (Attributes) model sınıflarında kullanmak için. |
| **VillaAgency.DataAccess** | `MongoDB.Driver` | v3.9.0 | MongoDB veritabanına bağlanmak, sorgu üretmek (Find, Filter) ve asenkron CRUD operasyonlarını yürütmek için ana sürücü. |
| **VillaAgency.DataAccess** | `Microsoft.Extensions.Configuration` | v10.0.8 | `appsettings.json` gibi yapılandırma dosyalarındaki verileri okuma altyapısı sağlar. |
| **VillaAgency.DataAccess** | `Microsoft.Extensions.Configuration.Binder` | v10.0.8 | Yapılandırma dosyasındaki ham metin verilerini (MongoDB string'leri gibi) doğrudan C# nesnelerine (`MongoDbSettings`) otomatik eşlemek (map etmek) için. |
| **VillaAgency.DataAccess** | `Microsoft.Extensions.DependencyInjection` | v10.0.8 | DataAccess katmanındaki servislerin (Context, Repository) uygulama havuzuna IoC/DI prensipleriyle kaydedilmesini sağlar. |
| **VillaAgency.Business** | `Mapster` | v3.9.0 | Business katmanında DTO ↔ Entity modelleri arasında dönüşüm yapmak için kullanılan hafif bir mapping kütüphanesidir. |
| **VillaAgency.WebUI** | `Microsoft.AspNetCore.Mvc` | Yerleşik | Kullanıcı arayüzünü, Admin panelini ve Controller/View mekanizmasını ayağa kaldıran ana web çatısı. |

---

## 🌟 PROJENİN ARTI DEĞERLERİ & MİMARİ AVANTAJLARI (Neden Bu Yapı?)

Bu proje geliştirilirken sadece anlık ihtiyaçlar değil, kurumsal yazılım mimarilerinin en önemli kuralları (SOLID prensipleri) göz önünde bulundurulmuştur. Projeyi inceleyen bir geliştiricinin bilmesi gereken temel artılar şunlardır:

### 1. SÜRDÜRÜLEBİLİR VE GÜVENLİ BAĞLANTI YÖNETİMİ
* **Avantaj:** Veritabanı şifreleri veya adresleri kodun içinde (Hardcoded) asla yer almaz.
* **Nasıl Çalışır?** Bağlantı bilgileri `appsettings.json` dosyasından okunur ve `MongoDbSettings` sınıfı ile eşlenir. Bu sayede hem tip güvenliği (Strongly-Typed) sağlanır hem de olası bir bağlantı hatasında (Örn: Yerel bilgisayardaki MongoDB servisinin/Mongo Daemon'ın çalışmaması durumunda) hatanın kaynağı ve çözümü tek bir dosyadan kolayca izlenebilir.

### 2. MİMARİNİN GÜCÜ: TEKNOLOJİ BAĞIMSIZLIĞI (Loose Coupling)
* **Avantaj:** Bugün **MongoDB (NoSQL)** olarak başlayan bu proje, yarın tek bir satır iş mantığı değiştirilmeden **PostgreSQL veya MS SQL (Relational/İlişkisel SQL)** veritabanına taşınabilir!
* **Mimari Altyapı (SOLID - Dependency Inversion):** * `WebUI` (Arayüz) ve `Business` (İş Mantığı) katmanları MongoDB teknolojisini doğrudan **tanımaz**. Sadece ortak bir sözleşme olan `IRepository` arayüzünü (Interface) tanır.
    * Eğer veritabanı değişirse; `Business` ve `WebUI` katmanlarına **hiç dokunulmaz**. Sadece `DataAccess` katmanında Entity Framework Core kullanan yeni bir `EfRepository` yazılır ve `DataAccessServiceExtension.cs` dosyasındaki bağımlılık kaydı güncellenir. 
    * Bu durum, SOLID prensiplerinin **D** harfi olan **Dependency Inversion (Bağımlılıkların Tersine Çevrilmesi)** ilkesinin birebir uygulanmış halidir. Sistem somut sınıflara değil, soyut kontratlara bağlıdır.

### 3. PERFORMANS ODAKLI SUNUM KATMANI ÖNBELLEKLEME (In-Memory Optimizasyonu)
* **Mikro Saniyeler Seviyesinde Yanıt Süresi (Latency Reduction):** Yoğun trafik alan villa/ürün listeleme (Index) sayfası, her istekte veritabanına gitmek yerine sunucu hafızasından (In-Memory) beslenir. Bu durum sayfa yüklenme hızını maksimuma çıkarırken, MongoDB cluster üzerindeki I/O (Giriş/Çıkış) ve Read (Okuma) yükünü neredeyse sıfıra indirir.
* **Katı Sorumlulukların Ayrılması (Strict Separation of Concerns):** Caching mekanizması mimari olarak yalnızca WebUI katmanında konumlandırılmıştır. Bu sayede Business katmanı, sunum teknolojisinin önbellek ihtiyaçlarından ve IMemoryCache bağımlılığından tamamen izole (decoupled) tutulmuştur. İş katmanı sadece saf iş kurallarına ve veri dönüşümüne (Mapster) odaklanır.
* **Defansif Önbellek Denetimi (Defensive Cache Lookup):** TryGetValue deseni kullanılarak defansif bir kontrol mekanizması işletilir. Talep edilen DTO listesi bellekte mevcutsa, istek Business ve DataAccess katmanlarına hiç uğramadan doğrudan Controller seviyesinden yanıtlanır. Bellekte veri yoksa (Cache Miss), veritabanından asenkron olarak çekilerek 30 dakikalık mutlak (Absolute) ve 10 dakikalık kayan (Sliding) sürelerle optimize bir şekilde hafızaya işlenir.
* **Proaktif Önbellek Geçersiz kılma (Proactive Cache Invalidation):** Veri bütünlüğünü korumak adına proaktif bir "State-Purging" (durum temizleme) stratejisi uygulanır. Sistemde veri değişimine yol açan Create, Update ve Delete (CUD) operasyonları tetiklendiği an, ilgili CacheKey bellekten anında imha edilir (evict). Bu sayede kullanıcıların "kirli veri" (dirty data) görme riski tamamen engellenir ve veritabanı ile önbellek arasında %100 gerçek zamanlı (real-time) senkronizasyon sağlanır.
  
---
## 📁 Proje Klasör Yapısı (Solution Explorer)

```plaintext
VillaAgency (Solution)
├── 📄 global.json (Opsiyonel - SDK sürüm sabitleyici)
│
├── 💻 VillaAgency.Entity (Class Library)
│       ├── 📁 Common
│       │   └── 📄 BaseEntity.cs (Ortak ID alanı)
│       └── 📁 Entities
│           └── 📄 Banner.cs (Şehir, Başlık, Görsel)
│
├── 💻 VillaAgency.DataAccess (Class Library)
│       ├── 📁 Abstract
│       │   └── 📄 IGenericDal.cs (Generic Repository Arayüzü)
│       └── 📁 Concrete
│       │   └── 📁 MongoDb.Driver
│       │         └── 📄 GenericRepository.cs (MongoDB somut implementasyonu)
│       ├── 📁 Configurations
│       │   └── 📄 MongoDbSettings.cs (appsettings.json eşlemesi)
│       ├── 📁 Context
│       │   └── 📄 MongoDbContext.cs (Bağlantı ve Koleksiyon yönetimi)
│       ├── 📁 Extensions
│           └── 📄 DataAccessServiceExtension.cs (DI Container Kayıtları)
│
├── 💻 VillaAgency.Dto (Class Library)
│       └── 📁 BannerDtos (Data Transfer Object sınıfları)
│       │   └── 📄 CreateBannerDto.cs
│       │   └── 📄 ResultBannerDto.cs
│       │   └── 📄 UpdateBannerDto.cs
│
├── 💻 VillaAgency.Business (Class Library)
│       ├── 📁 Abstract (İş mantığı arayüzleri - Örn: IBannerService)
│       │   └── 📄 IBannerService.cs
│       ├── 📁 Concrete (İş mantığı somut sınıfları - Örn: BannerManager)
│           └── 📄 BannerManager.cs
│       └── 📁 Extensions
│           └── 📄 BusinessServiceExtension.cs (Katman zincirleme kaydı)
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
│       ├── 📄 appsettings.json (Bağlantı string'leri)
│       └── 📄 Program.cs (Uygulama başlangıç noktası)

```

# 🚀 Adım Adım Geliştirme Süreci (Adım 0'dan Başlangıca)
## 🟢 ADIM 0 — Temellerin Atılması (Entity & Dto Katmanları)
Geliştirmeye en bağımsız katman olan Entity ile başlandı.

- BaseEntity.cs Yazıldı: Tekrarlayan kodları engellemek için tüm entity'lerin türeyeceği taban sınıf oluşturuldu. MongoDB kimlik yapısı (ObjectId) burada tanımlandı.
  
  ```csharp
  using MongoDB.Bson;
  using MongoDB.Bson.Serialization.Attributes;
  
  public class BaseEntity
  {
     public ObjectId Id { get; set; }
  }

- Banner.cs Yazıldı: BaseEntity'den miras alan ilk kalıcı sınıf oluşturuldu.

  ```csharp
  public class Banner : BaseEntity
  {
      public string City { get; set; }
      public string Title { get; set; }
      public string Image { get; set; }
  }

## 🔵 ADIM 1 — Yapılandırma Ayarları (DataAccess / MongoDbSettings.cs)
appsettings.json içerisindeki gizli veya değişken veritabanı ayarlarını kod içerisinde "Magic String" (sert kodlanmış metin) olarak kullanmamak için Strongly-Typed (Tip Güvenli) bir sınıf oluşturuldu.

appsettings.json İçeriği:

```JSON
{
  "MongoDB": {
    "ConnectionString": "-",
    "DatabaseName": "VillaAgencyDb"
  }
}
```

MongoDbSettings.cs Sınıfı:

```C#
public class MongoDbSettings
{
    public string ConnectionString { get; set; } ;
    public string DatabaseName { get; set; } ;
}
```

## 🔵 ADIM 2 — Veritabanı Bağlantısı (DataAccess / MongoDbContext.cs)
Entity Framework Core'daki `DbContext` yapısına benzer şekilde, projenin MongoDB bağlantısını, istemci havuzunu ve koleksiyon erişimlerini merkezi bir noktadan yönetmek amacıyla inşa edilmiştir.
```C#
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

## 🔵 ADIM 3 — Soyutlama Kontratı (DataAccess / IGenericDal.cs)
Sistemin veritabanı teknolojisinden (MongoDB, SQL Server, Oracle vb.) bağımsız çalışabilmesi için bir sözleşme (Interface) tasarlandı. SOLID'in Dependency Inversion Principle (DIP) ilkesi uygulandı. Business katmanı sadece bu interface'i tanır.

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

## 🔵 ADIM 4 — Somut Kodlama (DataAccess / MongoRepository.cs)
IRepository<T> interface'inin MongoDB sürücüsü kullanılarak doldurulduğu yerdir. MongoDbContext aracılığıyla ilgili koleksiyona bağlanır.

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

## 🔵 ADIM 5 — DataAccess Bağımlılık Kayıtları (DataAccess / DataAccessServiceExtension.cs)
Program.cs dosyasının şişmesini önlemek ve her katmanın kendi bağımlılığından sorumlu olmasını sağlamak için bir Extension Method yazıldı.

```C#
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

## 🔵 ADIM 6 — Katman Zincirleme (Business / BusinessServiceExtension.cs)
Mimari kurallar gereği WebUI (Arayüz) katmanı doğrudan DataAccess katmanına erişmemelidir. WebUI sadece Business katmanını tetikler, Business ise zincirleme olarak DataAccess servislerini container'a ekler.

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
            // 1. Önce DataAccess katmanının kendi servislerini kaydetmesini sağlıyoruz
            services.AddDataAccessServices(config);

            // 2. Business katmanına ait Manager'ları (İş sınıflarını) buraya kaydediyoruz
            services.AddScoped<IBannerService, BannerManager>();

            return services;
        }
    }
}

```

## 🔵 ADIM 7 — Son Tetikleme (WebUI / Program.cs)
Tüm mimarinin ayağa kalktığı ana giriş noktasıdır. Sadece tek satır eklenerek tüm katman mimarisi DI (Dependency Injection) sistemine öğretilmiş olur.

```C#
// WebUI katmanındaki Program.cs içerisi:
builder.Services.AddBusinessServices(builder.Configuration);

// Uygulama çalışma zamanına ait (runtime) bellek içi önbellekleme yeteneğini IoC Container'a kaydeder.
builder.Services.AddMemoryCache();
```

## 🔵 ADIM 8 — Entity Bazlı Business Katmanı Sözleşmesi (Business / IBannerService.cs)
Proje geliştikçe Business katmanı, tamamen generic yapıdan çıkarılarak entity bazlı servis sözleşmelerine dönüştürülmüştür. Bu yaklaşım sayesinde Business katmanı artık doğrudan DTO’lar ile çalışmakta, validasyon ve iş kuralları burada yönetilmektedir.
Bu yapı, Presentation katmanının (Controller) doğrudan Entity veya Repository ile temas etmesini engeller ve temiz mimariyi güçlendirir.

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

        Task<List<ResultBannerDto>> TGetFilteredListAsync(
            Expression<Func<Banner, bool>> predicate);
    }
}
```
### 🎯 Neden Entity Bazlı Service Yapısı?

* **Dışa Açılan Sözleşme (Public Contract):** DTO’lar Business katmanının dışa açılan sözleşmesi haline gelir.
* **Domain İzolasyonu:** Entity yapıları dış katmanlardan (UI/API) tamamen izole edilir.
* **Merkezi İş Kuralları:** Validation ve iş kuralları (business rules) merkezi bir noktada toplanır.
* **Gevşek Bağlılık (Loose Coupling):** Katmanlar arası bağımlılık azalır, esneklik artar.
* **Bağımsız Yaşam Döngüsü:** Her entity, generic soyutlamalara zorlanmadan kendi servis yaşam döngüsüne sahip olur.
* **Temiz Mimari:** UI $\rightarrow$ Entity bağımlılığı engellenerek Clean Architecture prensipleri güçlendirilir.


## 🔵 ADIM 9 — Business Katmanı Implementasyonu ve DTO Mapping (BannerManager.cs)
### ⚙️ BannerManager ve İş Akışı

`BannerManager`, gerçek iş mantığının (business logic) uygulandığı sınıftır. Bu yapı ile birlikte, generic manager yaklaşımının aksine bir isteğin tüm yaşam döngüsü şu adımlarla yönetilir:

1. **Veri Kabulü:** Controller’dan gelen veriler doğrudan DTO olarak alınır.
2. **Validasyon:** İş kuralları ve gerekli validasyon kontrolleri gerçekleştirilir.
3. **Dönüşüm:** DTO $\leftrightarrow$ Entity dönüşümleri **Mapster** kütüphanesi ile güvenli bir şekilde yapılır.
4. **Veri Erişimi:** DataAccess katmanı ile sıkı bağımlılık kurulmaz, yalnızca abstraction üzerinden iletişim sağlanır.
5. **Çıkış:** UI/Presentation katmanına sadece DTO döndürülür.

> 🔒 **En Büyük Kazanım:** Bu akış sayesinde Entity sınıfları hiçbir şekilde Presentation (UI/API) katmanına sızmaz.

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
                throw new FormatException("Geçersiz ObjectId formatı.");

            var entity = await _genericDal.GetByIdAsync(objectId);

            return entity.Adapt<ResultBannerDto>();
        }

        // Diğer metodlar sadeleştirilmiştir...
    }
}
```

## 🔵 ADIM 10 — Entity Bazlı Business Servislerinin DI Kaydı (Business / BusinessServiceExtension.cs)
Her entity için ayrı servis kayıtları yapılmaktadır. Bu yapı sayesinde DI container daha okunabilir ve kontrol edilebilir hale gelir.
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
## 🔵 ADIM 11 — UI Katmanının Oluşturulması (WebUI)
### 🔄 Paralel Geliştirme (Parallel Development) Modeli

UI katmanı; DataAccess ve Business katmanları ile eş zamanlı olarak geliştirilmiştir. Projede katmanların tamamen bitmesi beklenmeden, **modül bazlı (Feature-Driven)** bir ilerleme sağlanmıştır.

**Örnek Süreç (Banner Modülü):**
* **DataAccess (DAL):** Modüle ait veri tabanı nesneleri ve soyutlamaları oluşturuldu.
* **Business (BL):** İş mantığı, validasyonlar ve DTO yapıları hazırlandı.
* **UI / Presentation:** Diğer katmanlarla eş zamanlı olarak listeleme ve View yapıları tasarlandı.

> 💡 **Sürekli Besleme (Continuous Feedback):** Bu süreçte katmanlar statik kalmamış; sürekli olarak geliştirilmiş, test edilmiş ve birbirini besleyecek şekilde esnek bir döngüyle ilerlemiştir.

## 🔵 ADIM 12 — Areas Yapısının Kurulması (Admin Panel)
Proje içerisinde kullanıcı arayüzü (Frontend) ve yönetim paneli (Admin) birbirinden ayrılarak ASP.NET Core Areas yapısı kullanılmıştır. Bu yapı sayesinde Admin tarafı, ana uygulamadan izole ve modüler bir alt sistem olarak tasarlanmıştır.
### 🎯 Mimari Amaçlar
Bu yapının temel amacı, sürdürülebilir ve ölçeklenebilir bir mimari oluşturmaktır:
* **Katman Ayrımı (Separation of Concerns):** Kullanıcı arayüzü ile admin panelinin net şekilde ayrılması
* **Modülerlik ve Ölçeklenebilirlik:** Projenin büyümesine paralel olarak yeni özelliklerin kolayca eklenebileceği modüler ve ölçeklenebilir bir yapı oluşturmak.
* **Bağımsız Yönetim (Independent Administration):** Admin tarafını ana uygulamadan izole ederek, bağımsız bir mini uygulama (Sub-system) gibi yönetebilmek.

### 📁 Yapılanlar
* Areas/Admin yapısı oluşturuldu
* Admin için Controller ve View yapısı hazırlandı
* Ortak kullanım için _AdminLayout.cshtml oluşturuldu
* Admin panel sayfalarında standart UI düzeni sağlandı

## 🔵 ADIM 13 — Banner Controller (Admin Panel)
Admin paneli içerisinde ilk CRUD yapısının temeli olarak BannerController oluşturulmuştur.
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

### 🔵 ADIM 14 — Product Controller & Core UI Performance Caching (Main UI Scope)
ProductController, ana public kullanıcı arayüzünde temel veri çekme işlemlerini yönetir ve database’e gereksiz roundtrip’leri önlemek için ResultProductDto nesneleri üzerinde sıkı bir caching stratejisi uygular.
- Read Strategy: Index view, IMemoryCache üzerinden asenkron bir cache kontrolü kullanır. Cache boş (cold) ise ProductManager katmanına giderek verileri çeker, entity’leri DTO’lara map eder ve 30 dakikalık absolute / 10 dakikalık sliding policy ile cache’e kaydeder.
- Write & Invalidation Strategy: Veri tutarlılığını korumak için create, update ve delete operasyonları başarılı MongoDB işlemi sonrasında explicit cache expulsion gerçekleştirir. Böylece stale (eski) veriler temizlenir ve kullanıcı redirect öncesi cache yeniden oluşturulmaya zorlanır.
```c#
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Bson;
using System.Threading.Tasks;
using VillaAgency.Business.Abstract;
using VillaAgency.DataAccess.Abstract;
using VillaAgency.Dto.ProductDtos;

namespace VillaAgency.WebUI.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IMemoryCache _memoryCache;
        private const string CacheKey = "products_ui_cache_key";

        public ProductController(IProductService productService, IMemoryCache memoryCache)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        //[ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, NoStore = false)]
        //public async Task<IActionResult> Index()
        //{
        //    var products = await _productService.TGetListAsync();
        //    return View(products);
        //}

        public async Task <IActionResult> Index()
        {
            if (!_memoryCache.TryGetValue(CacheKey, out List<ResultProductDto> cachedProducts))
            {
                cachedProducts = await _productService.TGetListAsync();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(30))
                                        .SetSlidingExpiration(TimeSpan.FromMinutes(10));

                _memoryCache.Set(CacheKey, cachedProducts, cacheEntryOptions);
            }
            return View(cachedProducts);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductDto dto)
        {
            await _productService.TCreateAsync(dto);
            _memoryCache.Remove(CacheKey);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(ObjectId id)
        {
            await _productService.TDeleteAsync(id);
            _memoryCache.Remove(CacheKey);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(ObjectId id)
        {
            var value = await _productService.TGetByIdAsync(id);
            return View(value);
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateProductDto dto)
        {
            await _productService.TUpdateAsync(dto);
            _memoryCache.Remove(CacheKey);
            return RedirectToAction(nameof(Index));
        }

    }
}
```
