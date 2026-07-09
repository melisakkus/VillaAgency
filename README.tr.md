# VillaAgency

Language Options: [🇬🇧 English](README.md) | [🇹🇷 Türkçe (bu dosya)](README.tr.md)

**MongoDB üzerinde çalışan, katmanlı mimariyle geliştirilmiş bir emlak/villa tanıtım ve yönetim platformu.**

> 🧩 Teknik mimari kararları, katman tasarımı ve gerekçeleri için: **[Architecture.tr.md](Architecture.tr.md)**
>
> 🔗 **Canlı Demo:** _https://villa-agency.com/_
>
> 🔐 Paneli birlikte gezmek veya kurulum/demo talebiniz için: [İletişim](#i̇letişim)

ASP.NET Core 8 MVC ile yazılmış bu proje, bir emlak acentesinin hem müşteriye açık tanıtım sitesini hem de bu siteyi besleyen içerik yönetim panelini tek bir çözümde birleştirir.

---

## Proje Hakkında

VillaAgency, bir emlak acentesinin ihtiyaç duyacağı iki farklı deneyimi aynı çözümde birleştirir.

**Herkese açık taraf**, ziyaretçilerin villa ilanlarını kategoriye göre filtreleyerek inceleyebildiği, acente hakkında bilgi aldığı, sıkça sorulan sorulara ulaştığı ve iletişim formu üzerinden talep bırakabildiği bir tanıtım sitesidir. **Yönetim paneli** ise bu sitedeki her şeyin (ilanlar, banner'lar, video içerikler, SSS, iletişim bilgileri, gelen mesajlar) günlük olarak yönetildiği, oturum açma zorunluluğu olan ayrı bir alandır.

Yönetim panelinde rol tabanlı yetkilendirme uygulanmıştır: bir **Admin**, sisteme sınırlı yetkili **Manager** hesapları ekleyebilir; bu hesaplar yalnızca kendilerine tanımlanan işlemleri (ürün ve mesaj yönetimi) yapabilir, geri kalan modüllere (Banner, İletişim, Özellik/SSS, Video, Kullanıcı Yönetimi) erişemez. Gelen mesajlar okundu/silindi durumlarını destekleyen bir gelen kutusu mantığıyla yönetilir. Ürün listeleme, büyüyen veri hacmini gözeterek sayfalama (paging) ile yapılır; sistemde oluşabilecek beklenmeyen hatalar kullanıcıya ham stack trace olarak gösterilmek yerine merkezi bir hata yönetimi mekanizmasıyla ele alınır.

Bu proje, bir web uygulamasının temel iskeletinin — katmanlama, doğrulama, loglama, hata yönetimi, yetkilendirme, veri erişimi — nasıl tutarlı bir şekilde kurulacağını göstermek amacıyla geliştirilmiştir. Alınan mimari kararların gerekçeleri ve bilinen sınırlamalar, ayrı bir dokümanda ([Architecture.tr.md](Architecture.tr.md)) ayrıntılı olarak ele alınmıştır.

---

## Öne Çıkan Özellikler

### Herkese Açık Site

- **Ürün/Villa Listeleme:** Kategoriye göre filtreleme ve sunucu taraflı sayfalama (paging) desteği.
- **Anasayfa Vitrini:** Her kategoriden en az bir ilanı anasayfada gösteren, veritabanı seviyesinde çalışan bir rastgele örnekleme (random sampling) sorgusu.
- **Modüler İçerik Yönetimi:** Banner, tanıtım videosu, özellikler/SSS ve iletişim bilgileri gibi bölümler, admin panelinden yönetilen bağımsız ViewComponent'lerle oluşturulur.
- **AJAX Tabanlı Formlar:** İletişim ve soru formları sayfa yenilenmeden gönderilir; kullanıcıya anlık başarı veya hata bildirimi gösterilir.

### Yönetim Paneli

- **Rol Bazlı Yetkilendirme:** Admin ve Manager rolleri arasında farklı erişim kapsamları (ayrıntılı yetki tablosu için [Mimari dokümanına](Architecture.tr.md#kimlik-doğrulama-ve-rol-bazlı-yetkilendirme) bakınız).
- **Ürün Yönetimi:** Kategori ve durum bazlı filtreleme, sayfalama, tek tıkla durum güncelleme (Aktif / Satıldı / Kiralandı / Arşivlendi).
- **Mesaj Kutusu:** Tümü / Okunmamış / Silinmiş sekmeleri, her sekme için bağımsız sayfalama, okundu işaretleme ve geri alınabilir (soft delete) silme.
- **Dashboard:** Toplam/aktif/satılan ürün sayıları, kategori dağılımı ve okunmamış son mesajların paralel sorgularla tek sayfada özetlenmesi.
- **Kullanıcı Yönetimi:** Bir hesabı silmeden, aktif/pasif durumuna alarak erişimini geçici olarak durdurma.
- **Sunucu Taraflı Doğrulama:** FluentValidation ile alan bazlı, açıklayıcı hata mesajları.
- **Dark Mode:** CSS custom property tabanlı karanlık tema desteği.

---

## Kullanılan Teknolojiler

- **Backend:** .NET 8, ASP.NET Core MVC
- **Veritabanı:** MongoDB
- **Kimlik Doğrulama:** ASP.NET Core Identity (MongoDB üzerinde)
- **Doğrulama:** FluentValidation
- **Loglama:** Serilog (Console + günlük rotasyonlu dosya)
- **Nesne Eşleme:** Mapster
- **Ön Yüz:** Bootstrap 5, jQuery, SweetAlert2

Her katmanın hangi paketleri neden kullandığına dair ayrıntılı liste ve gerekçeler için: **[Architecture.tr.md](Architecture.tr.md#bağımlılık-envanteri-nuget-paketleri)**

---

## İlgili Proje: Test Verisi Üretici (Python)

Veritabanını gerçekçi test verisiyle doldurmak için ayrı bir Python aracı da geliştirilmiştir: **[VillaAgency_DataGenerator-Python-](https://github.com/melisakkus/VillaAgency_DataGenerator-Python-/blob/main/README.tr.md)**.

Projenin geliştirme sürecinde, sistemin üretim ortamındaki davranışını simüle etmek amacıyla kapsamlı bir veri yönetim aracı geliştirilmiştir. Bu araç, uygulamanın boş bir veritabanıyla değil, gerçekçi bir veri hacmiyle ölçeklendirilebilir olmasını sağlayan bir "Data Seeding" ve "Data Migration" katmanı olarak tasarlanmıştır.

- Data Seeding (Veri Tohumlama): `Faker` ve `PyMongo` kullanılarak, mantıksal kısıtlara (oda sayısı, fiyat aralığı, kat bilgisi vb.) sahip 1000+ adet gerçekçi ilan dokümanı otomatik olarak üretilmiş ve veritabanına aktarılmıştır.

- Data Migration (Veri Göçü ve Optimizasyon): Projenin evrimsel sürecinde veritabanı şeması üzerinde şu iyileştirmeler yapılmıştır:

    - Performans Optimizasyonu: `bulk_write` operasyonları ile görsel linkleri toplu olarak optimize edilmiştir.

    - Veri Normalizasyonu: Fiyat alanındaki veri tipi tutarsızlıkları giderilerek, tüm veriler float türünden int türüne dönüştürülmüştür.

    - Şema Yönetimi (Schema Evolution): İlanlara olasılık ağırlıklı durum (Status) ve zaman damgası (Timestamp) eklenmiş; ihtiyaç duyulmayan eski alanlar MongoDB’nin `$unset` operatörü ile veritabanından tamamen temizlenmiştir.

Bu yapı sayesinde geliştirme aşamasında gerçek verilerle çalışılmış, uygulama arayüzünün ve sorgu performansının en baştan optimize edilmesi sağlanmıştır.

Detaylar için ilgili reponun [README.tr.md](https://github.com/melisakkus/VillaAgency_DataGenerator-Python-/blob/main/README.tr.md) dosyasına bakabilirsiniz.

---
## 📸 Ekran Görüntüleri

### 🌐 Public Website

#### 🏠 Ana Sayfa

 ![Ana Sayfa](docs/screenshots/Public%20UI/UI-Banner.png)

 ![Ana Sayfa](docs/screenshots/Public%20UI/UI-Feature.png)

- ![Ana Sayfa](docs/screenshots/Public%20UI/UI-Video.png)

- ![Ana Sayfa](docs/screenshots/Public%20UI/UI-Counter.png)

- ![Ana Sayfa](docs/screenshots/Public%20UI/UI-Deal0.png)

- ![Ana Sayfa](docs/screenshots/Public%20UI/UI-Deal.png)

- ![Ana Sayfa](docs/screenshots/Public%20UI/UI-Products.png)

- ![Ana Sayfa](docs/screenshots/Public%20UI/UI-Contact.png)

- ![Ana Sayfa](docs/screenshots/Public%20UI/UI-Message-Error.png)

- ![Ana Sayfa](docs/screenshots/Public%20UI/UI-Message-Send.png)

- ![Ana Sayfa](docs/screenshots/Public%20UI/UI-FAQ.png)

#### 🏡 İlan Listeleme

- ![İlan Listeleme](docs/screenshots/Public%20UI/UI-Properties.png)

- ![İlan Listeleme](docs/screenshots/Public%20UI/UI-Properties-2.png)

- ![İlan Listeleme](docs/screenshots/Public%20UI/UI-Properties-3.png)

#### 📄 İlan Detay Sayfası

- ![İlan Detay Sayfası](docs/screenshots/Public%20UI/UI-Detail.png)

- ![İlan Detay Sayfası](docs/screenshots/Public%20UI/UI-Detail-2.png)

- ![İlan Detay Sayfası](docs/screenshots/Public%20UI/UI-Detail-3.png)

### 🔐 Admin Paneli

#### 🔑 Giriş Sayfası

- ![Giriş Sayfası](docs/screenshots/Admin/Admin-1.png)

- ![Giriş Sayfası](docs/screenshots/Admin/Admin-2.png)

- ![Giriş Sayfası](docs/screenshots/Admin/Admin-3.png)

#### 📊 Dashboard

- ![Dashboard](docs/screenshots/Admin/Admin-4.png)

- ![Dashboard](docs/screenshots/Admin/Admin-5.png)

- ![Dashboard](docs/screenshots/Admin/Admin-6.png)

- ![Dashboard](docs/screenshots/Admin/Admin-7.png)

#### 👥 Kullanıcı ve Rol Yönetimi

- ![Kullanıcı ve Rol Yönetimi](docs/screenshots/Admin/Admin-8.png)

#### 💬 Mesaj Yönetimi

- ![Mesaj Yönetimi](docs/screenshots/Admin/Admin-9.png)

- ![Mesaj Yönetimi](docs/screenshots/Admin/Admin-10.png)

- ![Mesaj Yönetimi](docs/screenshots/Admin/Admin-11.png)

- ![Mesaj Yönetimi](docs/screenshots/Admin/Admin-12.png)

- ![Mesaj Yönetimi](docs/screenshots/Admin/Admin-13.png)

#### 🏡 İlan Yönetimi

- ![İlan Yönetimi](docs/screenshots/Admin/Admin-14.png)

- ![İlan Yönetimi](docs/screenshots/Admin/Admin-15.png)

- ![İlan Yönetimi](docs/screenshots/Admin/Admin-16.png)

- ![İlan Yönetimi](docs/screenshots/Admin/Admin-17.png)

- ![İlan Yönetimi](docs/screenshots/Admin/Admin-18.png)

- ![İlan Yönetimi](docs/screenshots/Admin/Admin-19.png)

- ![İlan Yönetimi](docs/screenshots/Admin/Admin-20.png)

- ![İlan Yönetimi](docs/screenshots/Admin/Admin-21.png)

#### 🖼️ Banner Yönetimi

- ![Banner Yönetimi](docs/screenshots/Admin/Admin-22.png)

#### 📞 İletişim Bilgileri Yönetimi

- ![İletişim Bilgileri Yönetimi](docs/screenshots/Admin/Admin-23.png)

#### ⭐ Feature Yönetimi

- ![Feature Yönetimi](docs/screenshots/Admin/Admin-24.png)

- ![Feature Yönetimi](docs/screenshots/Admin/Admin-25.png)

#### ❓ FAQ Yönetimi

- ![FAQ Yönetimi](docs/screenshots/Admin/Admin-26.png)

- ![FAQ Yönetimi](docs/screenshots/Admin/Admin-27.png)

#### 🎥 Video Yönetimi

- ![Video Yönetimi](docs/screenshots/Admin/Admin-28.png)

- ![Video Yönetimi](docs/screenshots/Admin/Admin-29.png)

### 👤 Manager Paneli

#### 📊 Dashboard

- ![Dashboard](docs/screenshots/Manager/Manager-1.png)

#### 💬 Mesaj Yönetimi

- ![Mesaj Yönetimi](docs/screenshots/Manager/Manager-2.png)

- ![Mesaj Yönetimi](docs/screenshots/Manager/Manager-3.png)

- ![Mesaj Yönetimi](docs/screenshots/Manager/Manager-4.png)

- ![Mesaj Yönetimi](docs/screenshots/Manager/Manager-5.png)

#### 🏡 İlan Yönetimi

- ![İlan Yönetimi](docs/screenshots/Manager/Manager-6.png)

- ![İlan Yönetimi](docs/screenshots/Manager/Manager-7.png)

- ![İlan Yönetimi](docs/screenshots/Manager/Manager-8.png)

---

## Lisans

Bu proje şu an herhangi bir açık kaynak lisansı taşımamaktadır — tüm hakları saklıdır. Kodun incelenmesi serbesttir, ancak kopyalanması, ticari olarak yeniden kullanılması veya dağıtılması için önceden izin alınması gerekir.

## İletişim

Sorularınız, canlı demo talebiniz veya iş birliği teklifleriniz için aşağıdaki kanallardan bana ulaşabilirsiniz:

- **GitHub:** [github.com/melisakkus](https://github.com/melisakkus)
- **LinkedIn:** _https://www.linkedin.com/in/melisa-akkus-/_
- **Email:** _melisa.akkus01@gmail.com_