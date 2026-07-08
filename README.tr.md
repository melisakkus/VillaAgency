# VillaAgency

Language Options: [🇬🇧 English](README.md) | [🇹🇷 Türkçe (bu dosya)](README.tr.md)

**MongoDB üzerinde çalışan, katmanlı mimariyle geliştirilmiş bir emlak/villa tanıtım ve yönetim platformu.**

> 🧩 Teknik mimari kararları, katman tasarımı ve gerekçeleri için: **[docs/ARCHITECTURE.tr.md](docs/ARCHITECTURE.tr.md)**
>
> 🔗 **Canlı Demo:** _(yayına alındığında link burada paylaşılacaktır)_
>
> 🔐 Paneli birlikte gezmek veya kurulum/demo talebiniz için: [İletişim](#i̇letişim)

ASP.NET Core 8 MVC ile yazılmış bu proje, bir emlak acentesinin hem müşteriye açık tanıtım sitesini hem de bu siteyi besleyen içerik yönetim panelini tek bir çözümde birleştirir.

---

## Proje Hakkında

VillaAgency, bir emlak acentesinin ihtiyaç duyacağı iki farklı deneyimi aynı çözümde birleştirir.

**Herkese açık taraf**, ziyaretçilerin villa ilanlarını kategoriye göre filtreleyerek inceleyebildiği, acente hakkında bilgi aldığı, sıkça sorulan sorulara ulaştığı ve iletişim formu üzerinden talep bırakabildiği bir tanıtım sitesidir. **Yönetim paneli** ise bu sitedeki her şeyin (ilanlar, banner'lar, video içerikler, SSS, iletişim bilgileri, gelen mesajlar) günlük olarak yönetildiği, oturum açma zorunluluğu olan ayrı bir alandır.

Yönetim panelinde rol tabanlı yetkilendirme uygulanmıştır: bir **Admin**, sisteme sınırlı yetkili **Manager** hesapları ekleyebilir; bu hesaplar yalnızca kendilerine tanımlanan işlemleri (ürün ve mesaj yönetimi) yapabilir, geri kalan modüllere (Banner, İletişim, Özellik/SSS, Video, Kullanıcı Yönetimi) erişemez. Gelen mesajlar okundu/silindi durumlarını destekleyen bir gelen kutusu mantığıyla yönetilir. Ürün listeleme, büyüyen veri hacmini gözeterek sayfalama (paging) ile yapılır; sistemde oluşabilecek beklenmeyen hatalar kullanıcıya ham stack trace olarak gösterilmek yerine merkezi bir hata yönetimi mekanizmasıyla ele alınır.

Bu proje, bir web uygulamasının temel iskeletinin — katmanlama, doğrulama, loglama, hata yönetimi, yetkilendirme, veri erişimi — nasıl tutarlı bir şekilde kurulacağını göstermek amacıyla geliştirilmiştir. Alınan mimari kararların gerekçeleri ve bilinen sınırlamalar, ayrı bir dokümanda ([docs/ARCHITECTURE.tr.md](docs/ARCHITECTURE.tr.md)) ayrıntılı olarak ele alınmıştır.

---

## Ekran Görüntüleri

> Görselleri eklemek için: proje kökünde `docs/screenshots/` klasörü oluşturun, görselleri oraya koyun, aşağıdaki dosya adlarını kendi görsellerinizle değiştirin ve her başlığın altına `![Açıklama](docs/screenshots/dosya-adi.png)` satırını ekleyin — GitHub bu görselleri README içinde otomatik render eder.

**Herkese Açık Anasayfa**
`docs/screenshots/public-home.png`

**Ürün / Villa Listeleme & Filtreleme**
`docs/screenshots/public-products.png`

**Ürün Detay Sayfası**
`docs/screenshots/public-product-detail.png`

**İletişim Formu**
`docs/screenshots/public-contact.png`

**Admin Dashboard**
`docs/screenshots/admin-dashboard.png`

**Ürün Yönetimi (Admin)**
`docs/screenshots/admin-products.png`

**Mesaj Kutusu (Tümü / Okunmamış / Silinmiş)**
`docs/screenshots/admin-messages.png`

**Kullanıcı Yönetimi (Admin)**
`docs/screenshots/admin-users.png`

**Dark Mode Karşılaştırması**
`docs/screenshots/dark-mode.png`

**Hata Sayfası**
`docs/screenshots/error-page.png`

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

## Kullanılan Teknolojiler

- **Backend:** .NET 8, ASP.NET Core MVC
- **Veritabanı:** MongoDB
- **Kimlik Doğrulama:** ASP.NET Core Identity (MongoDB üzerinde)
- **Doğrulama:** FluentValidation
- **Loglama:** Serilog (Console + günlük rotasyonlu dosya)
- **Nesne Eşleme:** Mapster
- **Ön Yüz:** Bootstrap 5, jQuery, SweetAlert2

Her katmanın hangi paketleri neden kullandığına dair ayrıntılı liste ve gerekçeler için: **[docs/ARCHITECTURE.tr.md](docs/ARCHITECTURE.tr.md#bağımlılık-envanteri-nuget-paketleri)**

---

## İlgili Proje: Test Verisi Üretici (Python)

Veritabanını gerçekçi test verisiyle doldurmak için ayrı bir Python aracı da geliştirilmiştir: **[VillaAgency_DataGenerator-Python-](https://github.com/melisakkus/VillaAgency_DataGenerator-Python-)**.

Bu araç, `Faker` ile kategoriye özel mantıksal sınırlar (oda/banyo sayısı, fiyat aralığı, kat/otopark bilgisi) içeren 1000 adet ilan üretir; ardından MongoDB Atlas üzerinde toplu güncelleme (`bulk_write`) ile görsel linklerini günceller, fiyat alanını `float`'tan `int`'e dönüştürür ve ilanlara olasılık ağırlıklı durum (`Status`) ile oluşturulma zaman damgası ekleyip artık kullanılmayan eski alanları (`$unset`) temizler. Kısacası VillaAgency'nin boş bir veritabanıyla değil, üretim benzeri bir veri hacmiyle sergilenebilmesini sağlayan yardımcı bir migration/seed katmanıdır.

Detaylar için ilgili reponun [README.tr.md](https://github.com/melisakkus/VillaAgency_DataGenerator-Python-/blob/main/README.tr.md) dosyasına bakabilirsiniz.

---

## Lisans

Bu proje şu an herhangi bir açık kaynak lisansı taşımamaktadır — tüm hakları saklıdır. Kodun incelenmesi serbesttir, ancak kopyalanması, ticari olarak yeniden kullanılması veya dağıtılması için önceden izin alınması gerekir.

## İletişim

Sorularınız, canlı demo talebiniz veya iş birliği teklifleriniz için aşağıdaki kanallardan bana ulaşabilirsiniz:

- **GitHub:** [github.com/melisakkus](https://github.com/melisakkus)
- **LinkedIn:** _(profil linkinizi buraya ekleyin)_
- **E-posta:** _(iletişim e-postanızı buraya ekleyin)_