# 📑 PROJE GÜNLÜĞÜ: VillaAgency Projesi (.NET 8 & MongoDB)

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
| **VillaAgency.DataAccess** | `MongoDB.Driver` | v3.9.0 | MongoDB veritabanına bağlanmak, sorgu üretmek (Find, Filter) ve asenkron CRUD operasyonlarını yürütmek için ana sürücü. |
| **VillaAgency.DataAccess** | `Microsoft.Extensions.Configuration` | v10.0.8 | `appsettings.json` gibi yapılandırma dosyalarındaki verileri okuma altyapısı sağlar. |
| **VillaAgency.DataAccess** | `Microsoft.Extensions.Configuration.Binder` | v10.0.8 | Yapılandırma dosyasındaki ham metin verilerini (MongoDB string'leri gibi) doğrudan C# nesnelerine (`MongoDbSettings`) otomatik eşlemek (map etmek) için. |
| **VillaAgency.DataAccess** | `Microsoft.Extensions.DependencyInjection` | v10.0.8 | DataAccess katmanındaki servislerin (Context, Repository) uygulama havuzuna IoC/DI prensipleriyle kaydedilmesini sağlar. |
| **VillaAgency.Entity** | `MongoDB.Bson` | v3.9.0 | MongoDB'ye özgü veri formatı olan BSON (Binary JSON) nesne kimliklerini (`ObjectId`) ve veritabanı niteliklerini (Attributes) model sınıflarında kullanmak için. |
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
│       │   └── 📄 IRepository.cs (Generic Repository Arayüzü)
│       ├── 📁 Configurations
│       │   └── 📄 MongoDbSettings.cs (appsettings.json eşlemesi)
│       ├── 📁 Context
│       │   └── 📄 MongoDbContext.cs (Bağlantı ve Koleksiyon yönetimi)
│       ├── 📁 Extensions
│           └── 📄 DataAccessServiceExtension.cs (DI Container Kayıtları)
│       └── 📁 Repositories
│       │   └── 📄 MongoRepository.cs (MongoDB somut implementasyonu)
│
├── 💻 VillaAgency.Dto (Class Library)
│       └── 📁 BannerDtos (Data Transfer Object sınıfları)
│
├── 💻 VillaAgency.Business (Class Library)
│       ├── 📁 Abstract (İş mantığı arayüzleri - Örn: IBannerService)
│       ├── 📁 Concrete (İş mantığı somut sınıfları - Örn: BannerManager)
│       └── 📁 Extensions
│           └── 📄 BusinessServiceExtension.cs (Katman zincirleme kaydı)
│
└── 💻 VillaAgency.WebUI (ASP.NET Core MVC)
        ├── 📁 Controllers
        ├── 📁 Views
        ├── 📄 appsettings.json (Bağlantı string'leri)
        └── 📄 Program.cs (Uygulama başlangıç noktası)

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
      [BsonId]
      [BsonRepresentation(BsonType.ObjectId)]
      public string Id { get; set; }
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

## 🔵 ADIM 3 — Soyutlama Kontratı (DataAccess / IRepository.cs)
Sistemin veritabanı teknolojisinden (MongoDB, SQL Server, Oracle vb.) bağımsız çalışabilmesi için bir sözleşme (Interface) tasarlandı. SOLID'in Dependency Inversion Principle (DIP) ilkesi uygulandı. Business katmanı sadece bu interface'i tanır.

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

## 🔵 ADIM 4 — Somut Kodlama (DataAccess / MongoRepository.cs)
IRepository<T> interface'inin MongoDB sürücüsü kullanılarak doldurulduğu yerdir. MongoDbContext aracılığıyla ilgili koleksiyona bağlanır.

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
            services.AddScoped(typeof(IRepository<>), typeof(MongoRepository<>));

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
            // Örnek: services.AddScoped<IVillaService, VillaManager>();

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
```
