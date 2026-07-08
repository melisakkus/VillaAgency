# VillaAgency

Language Options: [🇬🇧 English](README.md) | [🇹🇷 Türkçe (bu dosya)](README.tr.md)

**MongoDB üzerinde çalışan, katmanlı mimariyle geliştirilmiş bir emlak/villa tanıtım ve yönetim platformu.**

> 🧩 Teknik mimari kararları, katman tasarımı ve gerekçeleri için: **[docs/ARCHITECTURE.tr.md](docs/ARCHITECTURE.tr.md)**
>
> 🔗 **Canlı Demo:** _(yayına alındığında link burada paylaşılacaktır)_

ASP.NET Core 8 MVC ile yazılmış bu proje, bir emlak acentesinin hem müşteriye açık tanıtım sitesini hem de bu siteyi besleyen içerik yönetim panelini tek bir çözümde birleştirir.

---

## Proje Hakkında

VillaAgency, bir emlak acentesinin ihtiyaç duyacağı iki farklı deneyimi aynı çözümde birleştirir.

**Herkese açık taraf**, ziyaretçilerin villa ilanlarını kategoriye göre filtreleyerek inceleyebildiği, acente hakkında bilgi aldığı, sıkça sorulan sorulara ulaştığı ve iletişim formu üzerinden talep bırakabildiği bir tanıtım sitesidir. **Yönetim paneli** ise bu sitedeki her şeyin (ilanlar, banner'lar, video içerikler, SSS, iletişim bilgileri, gelen mesajlar) günlük olarak yönetildiği, oturum açma zorunluluğu olan ayrı bir alandır.

Yönetim panelinde rol tabanlı yetkilendirme uygulanmıştır: bir **Admin**, sisteme sınırlı yetkili **Manager** hesapları ekleyebilir; bu hesaplar yalnızca kendilerine tanımlanan işlemleri (ürün ve mesaj yönetimi) yapabilir, geri kalan modüllere (Banner, İletişim, Özellik/SSS, Video, Kullanıcı Yönetimi) erişemez. Gelen mesajlar okundu/silindi durumlarını destekleyen bir gelen kutusu mantığıyla yönetilir. Ürün listeleme, büyüyen veri hacmini gözeterek sayfalama (paging) ile yapılır; sistemde oluşabilecek beklenmeyen hatalar kullanıcıya ham stack trace olarak gösterilmek yerine merkezi bir hata yönetimi mekanizmasıyla ele alınır.

Bu proje, bir web uygulamasının temel iskeletinin — katmanlama, doğrulama, loglama, hata yönetimi, yetkilendirme, veri erişimi — nasıl tutarlı bir şekilde kurulacağını göstermek amacıyla geliştirilmiştir. Alınan mimari kararların gerekçeleri ve bilinen sınırlamalar, ayrı bir dokümanda ([docs/ARCHITECTURE.tr.md](docs/ARCHITECTURE.tr.md)) ayrıntılı olarak ele alınmıştır.

---

## Ekran Görüntüleri

> Görselleri eklemek için: proje kökünde `docs/screenshots/` klasörü oluşturun, görselleri oraya koyun ve aşağıdaki placeholder satırları kendi dosya adlarınızla değiştirin. Göstermeye değer 4-6 kare önerilir: **(1)** herkese açık anasayfa, **(2)** ürün/villa listeleme + filtreleme, **(3)** ürün detay sayfası, **(4)** admin dashboard, **(5)** mesaj kutusu (sekmeli görünüm), **(6)** dark mode karşılaştırması. Görselleri yan yana değil, alt alta ve kısa bir açıklama satırıyla birlikte koymak GitHub'da daha temiz görünür.

**Herkese Açık Anasayfa**
`docs/screenshots/public-home.png`

**Ürün Listeleme & Filtreleme**
`docs/screenshots/public-products.png`

**Admin Dashboard**
`docs/screenshots/admin-dashboard.png`

**Mesaj Kutusu**
`docs/screenshots/admin-messages.png`

---

## Öne Çıkan Özellikler

### Herkese Açık Site

- **Ürün/Villa Listeleme:** Kategoriye göre filtreleme ve sunucu taraflı sayfalama (paging) desteği.
- **Anasayfa Vitrini:** Her kategoriden en az bir ilanı anasayfada gösteren, veritabanı seviyesinde çalışan bir rastgele örnekleme (random sampling) sorgusu.
- **Modüler İçerik Yönetimi:** Banner, tanıtım videosu, özellikler/SSS ve iletişim bilgileri gibi bölümler, admin panelinden yönetilen bağımsız ViewComponent'lerle oluşturulur.
- **AJAX Tabanlı Formlar:** İletişim ve soru formları sayfa yenilenmeden gönderilir; kullanıcıya anlık başarı veya hata bildirimi gösterilir.

### Yönetim Paneli

- **Rol Bazlı Yetkilendirme:** Admin ve Manager rolleri arasında farklı erişim kapsamları (ayrıntılı yetki tablosu için [Mimari dokümanına](docs/ARCHITECTURE.tr.md#kimlik-doğrulama-ve-rol-bazlı-yetkilendirme) bakınız).
- **Ürün Yönetimi:** Kategori ve durum bazlı filtreleme, sayfalama, tek tıkla durum güncelleme (Aktif / Satıldı / Kiralandı / Arşivlendi).
- **Mesaj Kutusu:** Tümü / Okunmamış / Silinmiş sekmeleri, her sekme için bağımsız sayfalama, okundu işaretleme ve geri alınabilir (soft delete) silme.
- **Dashboard:** Toplam/aktif/satılan ürün sayıları, kategori dağılımı ve son mesajların paralel sorgularla tek sayfada özetlenmesi.
- **Kullanıcı Yönetimi:** Bir hesabı silmeden, aktif/pasif durumuna alarak erişimini geçici olarak durdurma.
- **Sunucu Taraflı Doğrulama:** FluentValidation ile alan bazlı, açıklayıcı hata mesajları.
- **Dark Mode:** CSS custom property tabanlı karanlık tema desteği.

---

## Kullanılan Teknolojiler ve Seçim Gerekçeleri

Projenin katmanlı mimarisinde, her katman sadece ihtiyaç duyduğu bağımlılıkları içerecek şekilde izole edilmiştir.

| Katman Adı | Bağımlılık / NuGet Paketi | Versiyon | Amacı |
| :--- | :--- | :--- | :--- |
| **Genel** | .NET 8 SDK | v8.0 | Projenin genel çalışma zamanı (runtime) ve altyapısı. |
| **VillaAgency.Entity** | MongoDB.Bson | v3.9.0 | Model sınıflarının MongoDB standartlarına (`BsonId`, `BsonRepresentation`) uyumlu olması. |
| **VillaAgency.Entity** | AspNetCore.Identity.MongoDbCore | v7.0.0 | `AppUser`/`AppRole` Identity modellerinin doğrudan bu katmanda MongoDB'ye uyumlu tanımlanabilmesi. |
| **VillaAgency.Dto** | MongoDB.Bson | v3.9.0 | DTO'larda gerekli olduğunda `ObjectId`/Bson tiplerinin taşınabilmesi. |
| **VillaAgency.DataAccess** | MongoDB.Driver | v3.9.0 | Veritabanı etkileşimi, aggregation pipeline ve asenkron CRUD işlemleri. |
| **VillaAgency.DataAccess** | AspNetCore.Identity.MongoDbCore | v7.0.0 | ASP.NET Core Identity altyapısının MongoDB üzerinde native (EF Core gerektirmeden) çalışması. |
| **VillaAgency.DataAccess** | Humanizer.Core | v3.0.10 | Koleksiyon/isim dönüşümlerinde (ör. tekil→çoğul) okunabilir string üretimi. |
| **VillaAgency.DataAccess** | Microsoft.Extensions.Configuration | v10.0.8 | `appsettings.json` verilerinin okunması ve yönetilmesi. |
| **VillaAgency.DataAccess** | Microsoft.Extensions.Configuration.Binder | v10.0.8 | Ham konfigürasyon değerlerinin güçlü tipli (`MongoDbSettings`) C# nesnelerine bağlanması. |
| **VillaAgency.DataAccess** | Microsoft.Extensions.DependencyInjection | v10.0.8 | Repository ve Context sınıflarının uygulama havuzuna DI ile kaydedilmesi. |
| **VillaAgency.Business** | Mapster | v10.0.8 | Entity ve DTO katmanları arasında düşük maliyetli nesne eşleme (mapping). |
| **VillaAgency.Business** | FluentValidation | v12.1.1 | İş mantığı katmanında veri bütünlüğü ve doğrulama kuralları. |
| **VillaAgency.Business** | FluentValidation.DependencyInjectionExtensions | v12.1.1 | Modül bazlı validator sınıflarının IoC container'a otomatik kaydedilmesi. |
| **VillaAgency.Business** | Microsoft.AspNetCore.Http.Abstractions | v2.3.11 | Business katmanının, dosya yükleme gibi HTTP context'e bağlı işlemlerde MVC'ye sıkı bağımlı olmadan çalışabilmesi. |
| **VillaAgency.Business** | Microsoft.Extensions.Caching.Abstractions | v10.0.9 | `ICacheService` soyutlamasının altyapısı (bkz. [Sayfalama Tercihi](docs/ARCHITECTURE.tr.md#sayfalama-tercihi-neden-önbellekleme-yerine-sayfalama); şu an pasif). |
| **VillaAgency.WebUI** | FluentValidation.AspNetCore | v11.3.1 | FluentValidation kurallarının `ModelState` ile otomatik entegrasyonu. |
| **VillaAgency.WebUI** | Serilog.AspNetCore | v10.0.0 | Yapılandırılmış (structured) loglama ve merkezi hata izleme altyapısı. |
| **VillaAgency.WebUI** | Serilog.Sinks.Console | v6.1.1 | Geliştirme ortamında logların konsola yazılması. |
| **VillaAgency.WebUI** | Serilog.Sinks.File | v7.0.0 | Logların günlük rotasyonlu fiziksel dosyalara (`Logs/log-.txt`) yazılması. |
| **VillaAgency.WebUI** | Microsoft.VisualStudio.Web.CodeGeneration.Design | v8.0.23 | MVC Controller/View iskeletlerinin (scaffolding) otomatik oluşturulması. |

**Mimari notlar:**

- **İzolasyon:** `VillaAgency.WebUI` projesi, `DataAccess` veya `Entity` katmanlarına doğrudan referans vermez. UI katmanı, veritabanı detaylarından tamamen soyutlanmıştır.
- **Bağımlılık Zinciri:** DI kayıtları, `Program.cs` üzerinde tek bir `AddBusinessServices` çağrısıyla başlatılır; her katman kendi bağımlılıklarını (`AddDataAccessServices`, `AddIdentityServices`) kendi projesinde zincirleme olarak kaydeder.
- **DTO Stratejisi:** `VillaAgency.Dto` katmanındaki `MongoDB.Bson` bağımlılığı yalnızca veritabanı kimliklerinin (`ObjectId`) korunması amacıyla bulunur.

Katman sorumlulukları, veri erişim stratejisi, doğrulama/loglama/hata yönetimi mimarisi ve alınan tasarım kararlarının gerekçeleri için: **[docs/ARCHITECTURE.tr.md](docs/ARCHITECTURE.tr.md)**

---

## Kurulum ve Çalıştırma

### Gereksinimler
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- Bir MongoDB örneği (yerel kurulum veya [MongoDB Atlas](https://www.mongodb.com/atlas) bulut kümesi)

### Adımlar

```bash
git clone https://github.com/melisakkus/VillaAgency.git
cd VillaAgency
```

`VillaAgency.UI/appsettings.json` dosyasını oluşturun (bu dosya `.gitignore` içinde tutulur, repoda yer almaz):

```json
{
  "MongoDB": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "VillaAgencyDb"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": { "Microsoft": "Warning", "System": "Warning" }
    },
    "WriteTo": [
      { "Name": "Console" },
      {
        "Name": "File",
        "Args": {
          "path": "Logs/log-.txt",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 30,
          "outputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
        }
      }
    ]
  },
  "AllowedHosts": "*"
}
```

Ardından:

```bash
dotnet restore
dotnet run --project VillaAgency.UI
```

Uygulama ilk kez ayağa kalktığında `Admin` ve `Manager` rolleriyle birlikte varsayılan bir yönetici hesabı otomatik olarak oluşturulur (bkz. `Program.cs`). Kendi ortamınızda çalıştırırken bu bilgilerle doğrudan giriş yapabilirsiniz.

> 🔐 **Canlı demoyu incelemek isteyen değerlendiriciler için:** Giriş bilgilerini paylaşmak yerine, panel üzerinde birlikte gezinebilmek için benimle iletişime geçmenizi rica ederim (bkz. [İletişim](#i̇letişim)).

> ⚠️ **Güvenlik notu:** Seed edilen hesap yalnızca geliştirme/demo ortamı içindir. Prodüksiyona alınmadan önce mutlaka şifre değiştirilmeli veya seed mantığı ortamlara (Development/Production) göre koşullandırılmalıdır.

Loglar çalışma dizinindeki `Logs/` klasöründe günlük olarak rotasyona uğrar.

---

## Lisans

Bu proje şu an herhangi bir açık kaynak lisansı taşımamaktadır — tüm hakları saklıdır. Kodun incelenmesi serbesttir, ancak kopyalanması, ticari olarak yeniden kullanılması veya dağıtılması için önceden izin alınması gerekir.

## İletişim

Sorularınız, canlı demo talebiniz veya iş birliği teklifleriniz için aşağıdaki kanallardan bana ulaşabilirsiniz:

- **GitHub:** [github.com/melisakkus](https://github.com/melisakkus)
- **LinkedIn:** _(profil linkinizi buraya ekleyin)_
- **E-posta:** _(iletişim e-postanızı buraya ekleyin)_