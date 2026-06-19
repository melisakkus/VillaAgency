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

### 3. ABSTRACTED PERFORMANCE CACHING & INVALIDATION (Infrastructure Decoupling)
* **Microsecond-Level Latency Reduction:** Çok yüksek trafik alan villa/ürün listeleme (Index) sayfası, her istekte veritabanına sorgu atmak yerine optimize edilmiş bir caching katmanı aracılığıyla doğrudan sunucunun belleğinden servis edilir. Bu yaklaşım, sayfa yüklenme hızlarını maksimuma çıkarırken MongoDB cluster üzerindeki I/O ve Read yükünü neredeyse sıfıra indirir.
* **Technology-Agnostic Abstraction (SOLID Open/Closed Principle):** Controller'ların .NET’in yerleşik IMemoryCache motoruna sıkı sıkıya bağlanmasının (Tight Coupling) önüne geçmek için mimariye özel bir ICacheService soyutlama katmanı entegre edilmiştir. Bu sayede sunum katmanı, somut altyapı implementasyonlarından tamamen izole edilir. İleride sistem ölçekleme ihtiyaçları doğrultusunda sunucu RAM'inden (In-Memory), Redis gibi dağıtık bir yapıya (Distributed Cache) geçilmesi gerekirse, Controller tarafındaki tek bir satır kod bile değişmez; mühendislerin sadece IoC container içerisine yeni bir runtime manager enjekte etmesi yeterli olur.
* **Defensive Cache Lookup & Strict Token Governance:** Özel servis wrapper metotları kullanılarak defansif bir kontrol akışı (TryGetValue pattern) uygulanır. Eğer talep edilen DTO koleksiyonu bellek store'unda mevcutsa, istek anında Controller seviyesinde çözümlenir. Bir cache miss (bellekte bulamama) durumunda ise veriler asenkron olarak çekilir ve merkezi olarak yönetilen konfigürasyon politikalarıyla (30 dakikalık absolute / 10 dakikalık sliding expiration) cache'e yazılır. Ayrıca, tüm cache token'ları merkezi bir CacheKeys registry hub altında toplanarak kod içerisindeki "magic string" (sihirli string) kullanımı tamamen engellenmiştir (DRY Principle).
* **Proactive State Invalidation & CQRS Real-Time Balance:** Mutlak veri tutarlılığını garanti etmek için proaktif bir state-purging (durum temizleme) stratejisi uygulanır. Genel kullanıma açık UI bileşenleri salt okunur (Read-Only) cache stream'lerinden beslenirken; admin panelindeki veri manipülasyon işlemleri (Create, Update, Delete) başarılı bir veritabanı kaydının hemen ardından sistematik olarak _cacheService.Remove() pipeline'ını tetikler. Bu süreç, "dirty read" (kirli okuma) riskini tamamen ortadan kaldırarak kalıcı veritabanı ile cache katmanı arasında %100 gerçek zamanlı bir senkronizasyon sağlar.
  
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

### 🔵 ADIM 14 — Product Controller & Core UI Performance Caching Architecture (Main UI Scope)
Uygulama; veritabanı gidiş-dönüşlerini (database roundtrips) engellemek, gecikme sürelerini (latency) minimize etmek ve altyapı katmanları arasında Loose Coupling (gevşek bağlılık) sağlamak amacıyla optimize edilmiş bir caching stratejisi kullanır.

Yerleşik .NET IMemoryCache kütüphanesini doğrudan birden fazla controller içerisine enjekte etmek yerine, bu sistem mimarisinde soyut bir ICacheService katmanı ile merkezi bir CacheKeys constant (sabit) hub yapısı kurgulanmıştır.

### ⚙️ Neden Bu Caching Mimarisi? (Değer Önerileri ve Avantajlar)
- Enterprise-Grade Loose Coupling (Teknoloji Üzerinde Soyutlama): Controller katmanı yalnızca özel olarak yazılmış ICacheService interface kontratları ile etkileşime girer ve altyapıda hangi caching platformunun çalıştığından tamamen bağımsızdır (agnostic). İleride sistem ölçeği gereği yerel sunucu belleğinden (In-Memory) Redis gibi dağıtık ve kalıcı bir session cluster mimarisine geçiş kararı alınırsa, controller kodlarında tek bir satır dahi değişmeyecektir. Mühendislik ekibinin sadece IoC Container içerisinde yeni bir RedisCacheManager implementasyonunu enjekte etmesi yeterli olur (Open/Closed Principle).
- Kod Tekrarının Engellenmesi (DRY Prensibi): Cache key tanımlayıcıları, controller'ların içerisindeki gömülü ve sabit "magic string" (sihirli string) yapılarından arındırılarak merkezi bir VillaAgency.Business.Constants.CacheKeys statik sınıfına taşınmıştır. Bu sayede tüm anahtar değişiklikleri tek bir doğruluk kaynağından (source of truth) yönetilir.
- Read-Heavy vs. Write-Heavy Domain Bölümlemesi:
   * Main UI Scope (Read-Heavy): Genel kullanıma açık kullanıcı arayüzü, defansif bir TryGet kontrol stratejisi çalıştırır. Eğer veriler sıcaksa (warm data), Business ve DataAccess pipeline hatlarını tamamen baypas ederek sorgu çözümleme hızlarını mikrosaniyeler seviyesine indirir. Cold cache (önbelleğin boş olması) durumlarında ise asenkron bir veri çekme işlemi tetiklenir ve DTO yanıtları 30 dakikalık absolute / 10 dakikalık sliding expiration politikası altında depolanır.
   *  Admin Scope (Write-Heavy): Yönetim paneli, adminin her zaman gerçek zamanlı veritabanı durumunu gözlemleyebilmesi için listeleme esnasında cache kontrollerini baypas eder. Ancak, bir veri manipülasyon işlemi (Create, Update, Delete) gerçekleştiği anda, aktif bir Cache Invalidation (Eviction/Önbellekten Düşürme) pipeline'ı _cacheService.Remove() üzerinden tetiklenerek eski veri fragmanları temizlenir ve sistem genelinde "dirty read" (kirli okuma) oluşması engellenir.

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
