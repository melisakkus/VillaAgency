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
│
├── 💻 VillaAgency.Business (Class Library)
│       ├── 📁 Abstract (İş mantığı arayüzleri - Örn: IBannerService)
│       │   └── 📄 IGenericService.cs 
│       ├── 📁 Concrete (İş mantığı somut sınıfları - Örn: BannerManager)
│           └── 📄 GenericManager.cs 
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
        Task<T> GetByIdAsync(string id);

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

## 🔵 ADIM 8 — İş Mantığı Sözleşmesi (Business / IGenericService.cs)
DataAccess katmanındaki IGenericDal<T> nasıl bir sözleşme sunuyorsa, Business katmanı da dışarıya kendi sözleşmesini IGenericService<T> arayüzü aracılığıyla sunar. Metot isimlerindeki "T" ön eki, bu metotların Business katmanına ait olduğunu vurgular ve katmanlar arası karışıklığı önler.

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

## 🔵 ADIM 9 — İş Mantığı Somut Sınıfı (Business / GenericManager.cs)
IGenericService<T> arayüzünün somut implementasyonudur. Şu an için doğrudan IGenericDal<T> metodlarına yönlendirme (delegation) yapar. İlerleyen süreçte iş kuralları (validasyon, loglama, önbellekleme vb.) bu katmana ekleneceği için bu ara katmanın varlığı kritik önem taşır.
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

## 🔵 ADIM 10 — Business Bağımlılık Kayıtlarının Güncellenmesi (Business / BusinessServiceExtension.cs)
Adım 6'da iskelet olarak yazılan BusinessServiceExtension.cs artık tamamlanır. IGenericService<T> ↔ GenericManager<T> eşlemesi DI Container'a kaydedilerek katman zinciri tamamlanmış olur.

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
            // 1. DataAccess katmanının servislerini zincirleme olarak kaydet
            services.AddDataAccessServices(config);

            // 2. Business katmanının generic servis kaydı
            services.AddScoped(typeof(IGenericService<>), typeof(GenericManager<>));

            return services;
        }
    }
}
```

