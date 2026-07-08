# VillaAgency

**MongoDB üzerinde çalışan, katmanlı mimariyle geliştirilmiş bir emlak/villa tanıtım ve yönetim platformu.**

ASP.NET Core 8 MVC ile yazılmış bu proje, bir emlak acentesinin hem müşteriye açık tanıtım sitesini hem de bu siteyi besleyen içerik yönetim panelini tek bir çözümde birleştirir.

📖 Tasarım kararlarının detaylı gerekçeleri için → [docs/Architecture.md](docs/Architecture.md)

---

## İçindekiler

- [Proje Hakkında](#proje-hakkında)
- [Ekran Görüntüleri](#ekran-görüntüleri)
- [Özellikler](#özellikler)
- [Kullanılan Teknolojiler](#kullanılan-teknolojiler)
- [Mimari Genel Bakış](#mimari-genel-bakış)
- [Klasör Yapısı](#klasör-yapısı)
- [Kurulum ve Çalıştırma](#kurulum-ve-çalıştırma)
- [Tasarım Kararları (Özet)](#tasarım-kararları-özet)
- [Bilinen Sınırlamalar](#bilinen-sınırlamalar)
- [Gelecek Geliştirmeler](#gelecek-geliştirmeler)

---

## Proje Hakkında

VillaAgency, bir emlak acentesinin ihtiyaç duyacağı iki farklı deneyimi aynı çözümde birleştirir: ziyaretçilerin villa ilanlarını inceleyip iletişime geçebildiği herkese açık bir tanıtım sitesi ve bu sitedeki her şeyin (ilanlar, banner'lar, video içerikler, SSS, gelen mesajlar) yönetildiği, oturum korumalı bir Admin paneli.

Projeyi sıradan bir CRUD alıştırmasından ayıran nokta, hem arayüzün hem de panelin gerçek bir işletme senaryosu düşünülerek tasarlanmış olmasıdır: panelde rol tabanlı yetkilendirme (Admin/Manager hiyerarşisi) uygulanmıştır, gelen mesajlar okundu/silindi durumu olan bir gelen kutusu gibi davranır, ürün listeleme büyüyen veri hacmi düşünülerek sayfalanmıştır ve oluşan hiçbir hata kullanıcıya çıplak bir stack trace olarak yansımaz.

---

## Ekran Görüntüleri

*Yakında eklenecek.*

---

## Özellikler

### Herkese Açık Site

- Kategoriye ve duruma (aktif/satıldı/kiralandı) göre filtrelenebilen, sayfalanmış villa/ürün listeleme
- Anasayfada her kategoriden en az bir ilanı garanti eden, veritabanı seviyesinde rastgele örnekleme ile oluşturulan öne çıkan ürünler bölümü
- Banner, tanıtım videosu, özellik/SSS ve iletişim bilgisi bölümlerinin her biri bağımsız çalışan bileşenler (ViewComponent) olarak anasayfayı oluşturur
- İletişim formu ve ürün detay sayfasındaki soru formu, sayfa yenilenmeden AJAX ile gönderilir

### Yönetim Paneli

- İki seviyeli yetki modeli: **Admin** sitenin tüm içeriğini ve kullanıcı hesaplarını yönetir, **Manager** yalnızca ürün ve mesaj yönetimiyle sınırlıdır
- Ürün yönetiminde kategori/durum filtreleme, sayfalama ve tek tıkla durum değiştirme
- Gelen mesaj kutusu: tümü / okunmamış / silinmiş sekmeleri, her biri kendi sayfalamasına sahip, okundu işaretleme ve geri alınabilir (soft delete) silme
- Anlık istatistiklerin paralel sorgularla tek sayfada toplandığı bir dashboard
- Hesap aktif/pasif yapma (kullanıcıyı silmeden erişimini geçici olarak durdurma)
- FluentValidation ile sunucu tarafında doğrulanan, alan bazlı hata mesajları üreten formlar

---

## Kullanılan Teknolojiler

| Teknoloji | Kullanım Yeri | Neden |
|---|---|---|
| **.NET 8 / ASP.NET Core MVC** | Tüm katmanlar | LTS sürüm, Areas ile modüler routing, native DI |
| **MongoDB.Driver** | DataAccess | İç içe liste barındıran dokümanlar (`FeatureSection` gibi) için esnek şema |
| **AspNetCore.Identity.MongoDbCore** | DataAccess / Identity | Proje zaten MongoDB üzerinde; ayrı bir ilişkisel veritabanına gerek bırakmıyor |
| **Mapster** | Business | Az konfigürasyonla DTO ↔ Entity dönüşümü, `IgnoreNullValues` ile güvenli partial update |
| **FluentValidation** | Business | Karmaşık koşullu kuralları (`.When()`, `.Must()`) Attribute tabanlı doğrulamadan daha okunabilir ifade eder |
| **Serilog** | WebUI | `appsettings.json` üzerinden yönetilen, yapılandırılmış (structured) log kayıtları |

---

## Mimari Genel Bakış

Proje, bağımlılığın her zaman tek yönlü aktığı beş katmana ayrılmıştır:

```
VillaAgency.Entity        (bağımlılık yok)
VillaAgency.Dto           (bağımlılık yok)
VillaAgency.DataAccess    → Entity, Dto
VillaAgency.Business      → DataAccess, Dto
VillaAgency.UI (WebUI)    → Business
```

`WebUI`, veri erişim veya entity katmanlarına doğrudan referans vermez; Controller'lara yalnızca Business katmanındaki servis arayüzleri enjekte edilir. DI kayıtları da bu katmanlamayı yansıtacak şekilde zincirlenir: `Program.cs` tek bir `AddBusinessServices()` çağrısıyla tüm bağımlılık ağacını kurar, her katman kendi kayıt sorumluluğunu kendi projesinde taşır.

Veri erişiminde hibrit bir repository stratejisi kullanılır: CRUD-only entity'ler (Banner, Contact, Feature, Question) generic `IGenericDal<T>`'yi doğrudan kullanırken, özel sorgulara ihtiyaç duyan entity'ler (Product, Message, Video, Counter) kendi DAL arayüzlerini tanımlar. Business katmanında ise generic bir servis kullanılmaz — her entity kendi servis/manager çiftine sahiptir. Bu kararların gerekçeleri için → [docs/Architecture.md](docs/Architecture.md).

---

## Klasör Yapısı

```
VillaAgency/
├── VillaAgency.Entity/        # Entity sınıfları, Identity modelleri (AppUser, AppRole)
├── VillaAgency.Dto/           # Create/Update/Result DTO'ları (entity başına klasör)
├── VillaAgency.DataAccess/    # MongoDB repository'leri, Identity store, DI extension'ları
├── VillaAgency.Business/      # Servisler, manager sınıfları, FluentValidation kuralları
├── VillaAgency.UI/            # ASP.NET Core MVC WebUI
│   ├── Areas/Admin/           # Yönetim paneli (Controllers, Views)
│   ├── Controllers/           # Public site controller'ları
│   ├── ViewComponents/        # Anasayfayı oluşturan bağımsız bileşenler
│   └── Views/
└── docs/
    └── Architecture.md        # Detaylı mimari ve tasarım kararları dokümanı
```

---

## Kurulum ve Çalıştırma

Çözüm dosyası `VillaAgency.UI/VillaAgency.slnx` altındadır (kök dizinde `.sln` yoktur).

```bash
dotnet build VillaAgency.UI/VillaAgency.slnx
dotnet run --project VillaAgency.UI/VillaAgency.WebUI.csproj
```

MongoDB bağlantı ayarları `VillaAgency.UI/appsettings.json` içindeki `MongoDB:ConnectionString` / `MongoDB:DatabaseName` alanlarından yapılır.

Uygulama ilk çalıştırıldığında `Admin` ve `Manager` rolleri ile aşağıdaki ilk yönetici hesabı otomatik oluşturulur:

```
E-posta:  admin@villaagency.com
Şifre:    admin00
```

> Not: Proje şu an bir test projesi içermez; doğrulama manuel olarak yapılmaktadır.

---

## Tasarım Kararları (Özet)

**Neden Generic Repository var ama Generic Manager yok?**
DataAccess katmanında CRUD tekrarını önlemek için generic repository kullanıldı, ancak aynı yaklaşım Business katmanına taşınmadı: bu, entity tiplerinin Controller imzalarına sızmasına (domain sızıntısı), aşırı jenerik tip parametrelerine ve entity'ye özgü kuralların `if/else` yığınına dönüşmesine yol açıyordu. Bunun yerine her entity kendi servis/manager çiftine sahiptir.

**Neden Serilog?**
Yapılandırma tamamen `appsettings.json` üzerinden yönetilebiliyor ve log mesajları (`"Banner created. Id: {Id}"`) düz metin yerine yapılandırılmış alanlar olarak tutuluyor — bu, ileride bir log yönetim aracına geçişte filtreleme/indeksleme imkânı sağlıyor.

**Merkezi hata yönetimi neden bu şekilde?**
Business katmanı try-catch içermez; hatalar yakalanmadan yukarı fırlatılır ve yalnızca tek bir noktada (`HomeController.Error`) loglanır. Bu, aynı hatanın birden fazla katmanda tekrar loglanmasını (double logging) önler. Admin tarafında tam sayfa yönlendirme, Public tarafta AJAX formlarında inline uyarı kullanılır.

**Neden önbellekleme yerine sayfalama?**
Erken bir aşamada kurulan `ICacheService`/`CacheManager` soyutlaması, ürün listeleme sunucu taraflı sayfalamaya geçirilince aktif kullanımdan kaldırıldı — sayfalanmış küçük veri kümelerinde cache invalidation karmaşıklığı, kazanılan performansa değmiyordu. Soyutlama, ileride trafik artarsa tek satır değişiklikle devreye alınabilmesi için bilinçli olarak kodda bırakıldı.

**Admin/Manager rol ayrımı nasıl uygulanıyor?**
Tüm Admin controller'ları ortak `AdminBaseController`'dan (`[Authorize]`) türer; statik içerik ve kullanıcı yönetimi controller'ları ek olarak `[Authorize(Roles = Roles.Admin)]` taşır. Ürün, mesaj ve dashboard controller'ları ise her iki role de açıktır — panelin operasyonel kısmını oluştururlar.

Bu kararların tam gerekçeleri, değerlendirilen alternatifler ve kod referansları için → **[docs/Architecture.md](docs/Architecture.md)**

---

## Bilinen Sınırlamalar

- Test projesi yoktur; doğrulama şu ana kadar manuel yapılmıştır.
- `ICacheService` bilinçli olarak pasif tutulmaktadır (bkz. Tasarım Kararları).
- `ExceptionMiddleware` yalnızca referans amaçlıdır, bir Web API katmanı eklenmeden önce gerçek koşullarda test edilmemiştir.

---

## Gelecek Geliştirmeler

- İş kurallarını regresyona karşı güvence altına almak için bir unit test projesi eklenmesi
- Veri hacmi arttığında `Status`/`Category`/`IsDeleted` gibi filtrelenen alanlar için MongoDB bileşik index'leri tanımlanması
- Trafik artışı durumunda `ICacheService`'in devreye alınması (gerekirse Redis tabanlı bir implementasyonla)
- Bir Web API katmanı eklenmesi durumunda `ExceptionMiddleware`'in aktifleştirilmesi
