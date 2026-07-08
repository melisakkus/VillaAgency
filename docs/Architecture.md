# VillaAgency — Mimari ve Tasarım Kararları

Bu belge, VillaAgency projesinin katman mimarisini, veri erişim stratejisini ve geliştirme sürecinde alınan tasarım kararlarının gerekçelerini ayrıntılı olarak açıklar. Projenin genel tanıtımı, özellik listesi ve kurulum talimatları için depo kökündeki [README](../README_NEW.md) dosyasına bakın.

Buradaki her başlık, kodda karşılığı olan somut bir tercihi anlatır; varsayımsal ya da uygulanmamış bir özellik içermez.

## İçindekiler

- [Mimari Yaklaşım](#mimari-yaklaşım)
- [Veri Erişim Katmanı: Generic ve Entity-Özel Repository'lerin Bir Arada Kullanımı](#veri-erişim-katmanı-generic-ve-entity-özel-repositorylerin-bir-arada-kullanımı)
- [İş Mantığı Katmanı: Generic Manager'dan Bilinçli Bir Vazgeçiş](#iş-mantığı-katmanı-generic-managerdan-bilinçli-bir-vazgeçiş)
- [Doğrulama: FluentValidation](#doğrulama-fluentvalidation)
- [Loglama: Serilog ile Yapılandırılmış Kayıt](#loglama-serilog-ile-yapılandırılmış-kayıt)
- [Merkezi Hata Yönetimi](#merkezi-hata-yönetimi)
- [Kimlik Doğrulama ve Rol Bazlı Yetkilendirme](#kimlik-doğrulama-ve-rol-bazlı-yetkilendirme)
- [Sayfalama Tercihi: Neden Önbellekleme Yerine Sayfalama?](#sayfalama-tercihi-neden-önbellekleme-yerine-sayfalama)
- [Ölçeklenebilirlik ve Sürdürülebilirlik](#ölçeklenebilirlik-ve-sürdürülebilirlik)
- [Bilinen Sınırlamalar](#bilinen-sınırlamalar)

---

## Mimari Yaklaşım

Proje beş ayrı sınıf kütüphanesine bölünmüştür ve bağımlılık her zaman tek yönlüdür:

```
VillaAgency.Entity        (bağımlılık yok)
VillaAgency.Dto           (bağımlılık yok)
VillaAgency.DataAccess    → Entity, Dto
VillaAgency.Business      → DataAccess, Dto
VillaAgency.UI (WebUI)    → Business
```

`WebUI`, `DataAccess` veya `Entity` projelerine doğrudan referans vermez; Controller'lara yalnızca `Business` katmanındaki servis arayüzleri (`IProductService`, `IBannerService` vb.) enjekte edilir. Bunun tek istisnası, filtre predicate'lerinin (`Expression<Func<TEntity, bool>>`) servis metotlarına parametre olarak geçilmesi gereken noktalardır — bu durumda Entity tipi Controller içinde görünür, ancak Entity hiçbir zaman View'a taşınmaz; View'lara giden her veri DTO'dur.

DI kayıtları da bu katmanlamayı yansıtacak şekilde zincirlenir: her katman kendi servislerini kendi `Add*Services()` extension metoduyla kaydeder ve bir alt katmanın kayıt metodunu kendi içinde çağırır.

```
Program.cs
  └─ AddBusinessServices(config)
       ├─ AddDataAccessServices(config)   // repository + MongoContext kayıtları
       ├─ AddIdentityServices(config)     // Identity + Mongo store kayıtları
       └─ (Business servisleri + FluentValidation kayıtları)
```

Bunun sağladığı şey basittir: `Program.cs`, tüm bağımlılık ağacını tek bir üst seviye çağrıyla (`AddBusinessServices`) kurar; her katman kendi bağımlılık kayıt sorumluluğunu kendi projesinde taşır. İleride bir `VillaAgency.Api` projesi eklenmek istenirse, mevcut zincire dokunmadan yalnızca kendi `Add*Services()` metodunu çağırması yeterlidir.

---

## Veri Erişim Katmanı: Generic ve Entity-Özel Repository'lerin Bir Arada Kullanımı

Tüm entity'lerin ortak atası olan `BaseEntity`, `Id` alanını bilinçli olarak `string` tutar, MongoDB'nin native `ObjectId` tipini değil:

```csharp
public class BaseEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
}
```

Gerekçesi şudur: `ObjectId`, MongoDB.Bson'a bağımlı bir tiptir. Bu tipin Business ve UI katmanlarına (route parametreleri, form alanları, DTO'lar) sızması, bu katmanların MongoDB'ye bağımlı hale gelmesi demektir. `string` kullanımı ve `[BsonRepresentation(BsonType.ObjectId)]` ile bu dönüşümün veritabanı seviyesinde otomatik yapılması, üst katmanların altyapı detayından habersiz kalmasını sağlar.

`GenericRepository<T>`, `IGenericDal<T>` sözleşmesini uygulayarak CRUD operasyonlarını (`CreateAsync`, `UpdateAsync`, `DeleteAsync`, `GetListAsync`, `GetByIdAsync`, sayfalamalı `GetFilteredListAsync`) tek bir yerde toplar. Koleksiyon adı, `typeof(T).Name.Pluralize()` (Humanizer) ile otomatik türetilir; yeni bir entity eklendiğinde koleksiyon adı elle tanımlanmaz.

Ancak her entity aynı ihtiyaçlara sahip değildir, bu yüzden projede iki strateji bir arada bulunur:

- **Yalnızca generic olan entity'ler** (Banner, Contact, Feature, Question) için ayrı bir repository yazılmaz; Business katmanı doğrudan `IGenericDal<TEntity>` enjekte eder.
- **Entity-özel DAL'a sahip olanlar** (Product, Message, Video, Counter), generic repository'nin karşılayamayacağı sorgular için kendi arayüzlerini (`I<Entity>Dal : IGenericDal<TEntity>`) tanımlar. Örneğin:
  - `IProductDal.GetRandomProductPerCategoryAsync()` — anasayfadaki öne çıkan ürünler için `$sample` → `$group` → `$slice` aggregation pipeline'ı ile kategori başına rastgele seçim
  - `IProductDal.ChangeStatusAsync(id, status)` — tüm dokümanı değil yalnızca `Status` ve `UpdatedDate` alanlarını güncelleyen kısmi update
  - `IMessageDal.MarkAsReadAsync / MarkAsDeletedAsync` — mesaj durumunu tek alan güncellemesiyle değiştirme
  - `ICounterDal.GetProductCountsByCategoryAsync()` — dashboard için `$group` aggregation'ı ile kategori bazlı sayım

Yeni bir entity eklenirken varsayılan yaklaşım generic-only'dir; entity-özel DAL yalnızca gerçek bir ihtiyaç ortaya çıktığında açılır. Bu, her entity için gereksiz bir arayüz/sınıf çifti oluşturmaktan kaçınır.

---

## İş Mantığı Katmanı: Generic Manager'dan Bilinçli Bir Vazgeçiş

DataAccess katmanında generic repository deseni işe yaradıktan sonra, akla gelen doğal bir sonraki adım aynı yaklaşımı Business katmanına da taşımaktı — `IGenericService<TEntity, TResultDto, TCreateDto, TUpdateDto>` ve buna karşılık gelen bir `GenericManager<...>`. Bu yaklaşım denendi, ancak üç somut gerekçeyle terk edildi:

1. **Domain sızıntısı.** Generic bir servis Controller'a enjekte edildiğinde, Controller'ın imzasında entity tipinin (`IGenericService<Banner, ...>`) görünmesi kaçınılmaz hale gelir. Bu, Sunum katmanının veritabanı varlıklarını doğrudan tanımasına, yani katmanlar arası izolasyonun delinmesine yol açar.
2. **Tek Sorumluluk ihlali riski.** Business katmanı yalnızca veri taşımaz, iş kurallarını da barındırır. Entity'ye özgü bir davranış eklenmesi gerektiğinde (örn. yeni bir mesaj oluşturulurken `IsRead = false` ve `MessageDate = DateTime.UtcNow` atanması), generic bir yapı bunu `if/else` bloklarıyla kirletmek zorunda kalır.
3. **Jenerik tip şişkinliği.** Dört farklı jenerik parametre taşıyan bir arayüz, hem DI kayıtlarını hem metot imzalarını okunması güç hale getirir.

Bunun yerine hibrit bir model uygulandı: DataAccess'te generic repository korunurken, Business katmanında her entity kendi arayüzüne (`IBannerService`) ve kendi manager sınıfına (`BannerManager`) sahiptir. Bu sınıflar DTO ↔ Entity dönüşümünü Mapster ile yapar, `null` parametrelere `ArgumentNullException`, bulunamayan kayıtlara `KeyNotFoundException` fırlatır ve işlemleri `ILogger<T>` ile loglar. Yazılan kod miktarı generic yaklaşıma göre biraz artar, ama karşılığında Sunum katmanı veritabanından tamamen izole kalır ve her entity kendi kuralı için genişleyebilir bir zemine sahip olur.

---

## Doğrulama: FluentValidation

Doğrulama kuralları `Business/Validators/<Entity>Validators/` altında toplanır ve derleme taraması (`AddValidatorsFromAssemblyContaining<CreateBannerValidator>()`) ile otomatik kaydedilir; yeni bir validator eklendiğinde DI kaydına elle dokunmak gerekmez. `AddFluentValidationAutoValidation()` ile MVC pipeline'ına bağlanır, Controller'lar yalnızca standart `ModelState.IsValid` kontrolünü yapar.

Create ve Update DTO'ları için ayrı ayrı kural yazmak yerine, ortak kurallar bir `BaseProductValidator<T> : AbstractValidator<T> where T : BaseProductDto` içinde toplanır; `CreateProductValidator` ve `UpdateProductValidator` bu sınıftan türeyip yalnızca kendine özgü ek kuralları ekler. Kurallar arasında koşullu doğrulamalar da yer alır — örneğin `Area` alanı ya `0` (belirtilmemiş) ya da `10`'dan büyük olmak zorundadır (`.Must(area => area == 0 || area >= 10)`), bu da opsiyonel sayısal alanların yalnızca doldurulduklarında anlamlı bir aralıkta olmasını garanti eder.

`Program.cs` içindeki `SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true` ayarı, .NET'in non-nullable referans tipleri için otomatik uyguladığı örtük `[Required]` doğrulamasını devre dışı bırakır; bu sayede doğrulama sorumluluğu tamamen FluentValidation'da toplanır ve iki farklı doğrulama kaynağının çakışması engellenir.

---

## Loglama: Serilog ile Yapılandırılmış Kayıt

.NET'in yerleşik loglama altyapısı yerine Serilog'un tercih edilmesinin iki nedeni vardır: yapılandırmanın tamamen `appsettings.json` üzerinden yönetilebilmesi (`ReadFrom.Configuration()`) ve mesajların (`"Banner created successfully. Id: {Id}"`) düz metin yerine yapılandırılmış (structured) alanlar olarak loglanabilmesi — bu, ileride bir log yönetim aracına (Seq, Elasticsearch vb.) geçildiğinde `{Id}` veya `{Title}` gibi alanlara göre filtreleme yapılabilmesini mümkün kılar.

Loglar eşzamanlı olarak Console'a ve günlük rotasyonlu dosyalara (`Logs/log-.txt`, 30 gün saklama) yazılır. `Microsoft`/`System` kaynaklı framework logları `Warning` seviyesine kısıtlanmıştır — bu yalnızca düşük seviyeli gürültüyü susturur, bir `Error` kaydı kaynağı ne olursa olsun her zaman loglanır. `app.UseSerilogRequestLogging()` her HTTP isteğini (path, status code, süre) otomatik loglar; `app.Lifetime.ApplicationStopping` event'ine bağlanan `Log.CloseAndFlush`, uygulama kapanırken bellekte bekleyen logların kaybolmasını engeller.

Business katmanındaki manager sınıfları başarılı işlemleri `LogInformation`, beklenmedik-ama-hata-olmayan durumları (`entity is null` gibi) `LogWarning` ile kaydeder. `LogError` seviyesi Business katmanında **bilinçli olarak üretilmez**: hatalar try-catch ile yutulmadan yukarı fırlatılır ve yalnızca merkezi hata yönetim noktasında bir kez loglanır. Aksi halde aynı hata hem Business katmanında hem merkezi handler'da olmak üzere iki kez loglanır, bu da log dosyalarını şişirir ve olay analizini zorlaştırır.

---

## Merkezi Hata Yönetimi

Uygulama `app.UseExceptionHandler("/Home/Error")` ile çalışır: pipeline'ın herhangi bir noktasında yakalanmayan bir exception, isteği `HomeController.Error` aksiyonuna yönlendirir. Bu aksiyon `IExceptionHandlerPathFeature` ile orijinal exception'ı ve path'i alır, stack trace dahil tek noktadan loglar ve kullanıcıya teknik detay sızdırmadan sabit, güvenli bir mesaj (`ErrorViewModel.UserMessage`) gösterir.

`Error.cshtml` bilinçli olarak `Layout = null` ile bağımsız bir `<html>` dokümanı olarak render edilir; böylece sitenin genel şablonu bozuk bir durumdayken bile kullanıcı her zaman tutarlı bir hata ekranıyla karşılaşır. Sayfa, oturum durumuna göre farklı bir geri dönüş linki sunar: oturum açmış kullanıcı için "Back to Dashboard", anonim ziyaretçi için "Back to Home".

Mekanizma tek bir merkezi noktadan beslenir, ama isteğin türüne göre kullanıcı deneyimi farklılaşır. Admin panelinde bir hata oluştuğunda tarayıcı `/Home/Error` sayfasına yönlendirilir. Public tarafta ise iletişim formu gibi AJAX ile gönderilen formlarda sunucu hatası 500 status koduyla döner, jQuery bunu `error` callback'ine yönlendirir, sayfa hiç yenilenmez, kullanıcıya yalnızca inline bir uyarı (SweetAlert) gösterilir — form doldurma akışı kesilmeden hata bildirilmiş olur.

Projede ayrıca, `Middleware_Reference` klasörü altında exception tipine göre (`FluentValidation.ValidationException` → 400, diğerleri → 500) JSON formatında hata dönen tam bir `ExceptionMiddleware` sınıfı bulunur, ancak bu sınıf DI'a kayıtlı değildir ve çalışma zamanında hiçbir etkisi yoktur. Bunun nedeni basittir: bu bir MVC uygulamasıdır ve kullanıcıya JSON değil View dönülmelidir; `UseExceptionHandler` bu ihtiyacı zaten karşılar. Sınıf, projeye ileride bir Web API katmanı eklenmesi ihtimaline karşı — o senaryoda JSON dönen bir hata middleware'i doğrudan işe yarayacağı için — referans olarak saklanmaktadır.

---

## Kimlik Doğrulama ve Rol Bazlı Yetkilendirme

Kimlik doğrulama `AspNetCore.Identity.MongoDbCore` ile MongoDB üzerine kurulmuştur (`AppUser : MongoIdentityUser<string>`, `AppRole`). Gerekçe basittir: proje zaten uçtan uca MongoDB üzerinde çalışıyor, kullanıcı yönetimi için ayrı bir ilişkisel veritabanı açmak gereksiz bir bağımlılık olurdu.

Panel, tek tip bir "admin kullanıcı" yerine gerçek bir kurumsal yapıya benzer şekilde iki katmanlı bir yetki modeli üzerine kuruludur: bir **Admin**, kendi altında sınırlı yetkili **Manager** hesapları açabilir.

| Alan | Admin | Manager |
|---|---|---|
| Dashboard | Görüntüleme | Görüntüleme |
| Ürün Yönetimi (Product) | Tam CRUD + durum değiştirme | Tam CRUD + durum değiştirme |
| Mesaj Yönetimi (Message) | Tam CRUD + okundu/silindi işaretleme | Tam CRUD + okundu/silindi işaretleme |
| Banner | Tam CRUD | Erişim yok |
| İletişim Bilgileri (Contact) | Tam CRUD | Erişim yok |
| Özellik Bölümü (Feature/FAQ) | Tam CRUD | Erişim yok |
| Soru-Cevap (Question) | Tam CRUD | Erişim yok |
| Video | Tam CRUD | Erişim yok |
| Kullanıcı Yönetimi (User) | Tam CRUD | Erişim yok |

Bu ayrım, kodda tüm Admin controller'larının ortak atası olan `AdminBaseController`'ın (`[Area("Admin")] [Authorize]`) üzerine, Admin'e özel controller'larda (`BannerController`, `ContactController`, `FeatureController`, `QuestionController`, `VideoController`, `UserController`) `[Authorize(Roles = Roles.Admin)]` eklenmesiyle uygulanır. `ProductController`, `MessageController` ve `DashboardController` ise yalnızca temel yetkilendirmeyi taşır; bu üçü panelin operasyonel kısmını oluşturur ve bir Manager'ın günlük işini (ilan girmek, mesaj yanıtlamak) Admin'e ihtiyaç duymadan yapabilmesini sağlar.

Bu hiyerarşiyi pekiştiren bir detay: `UserController.Create`, panel üzerinden açılan yeni hesapları sabit olarak `Roles.Manager` ile kaydeder. Panelden yeni bir Admin hesabı açılamaz — Manager hesapları yalnızca mevcut bir Admin tarafından oluşturulabilen, ona bağlı hesaplardır.

Giriş akışında `AuthManager.LoginAsync`, kullanıcı adı ile e-postayı aynı alandan (`@` karakteri kontrolüyle) ayırt eder ve `lockoutOnFailure: true` ile Identity'nin `MaxFailedAccessAttempts = 5` / `10 dakika` kilitleme politikasını devreye sokar. `user.IsActive` alanı ayrıca kontrol edilir; pasife alınmış bir hesap doğru şifreyle bile giriş yapamaz — bu, bir Admin'in bir Manager'ın erişimini hesabı silmeden geçici olarak durdurabilmesini sağlar. Uygulama ilk ayağa kalktığında roller ve `admin@villaagency.com` ile bir ilk Admin hesabı otomatik oluşturulur; yeni bir ortama deploy edildiğinde veritabanına elle kayıt atmaya gerek kalmaz.

---

## Sayfalama Tercihi: Neden Önbellekleme Yerine Sayfalama?

Geliştirmenin erken bir aşamasında, ürün listeleme için `IMemoryCache`'i saran bir `ICacheService`/`CacheManager` soyutlaması kurulmuştu. Proje ilerledikçe, ürün listeleme ihtiyacı sunucu taraflı sayfalama ve filtreleme (`GetFilteredListAsync(predicate, page, pageSize)` → MongoDB `Skip`/`Limit`) ile çözüldü. Bu değişiklikle her istekte veritabanından tüm koleksiyon değil, yalnızca ilgili sayfaya ait küçük bir alt küme çekilir hale geldi.

Bu noktada caching'in getirisi sorgulandı: sorgu başına dönen veri hacmi zaten küçük olduğundan, önbellekleme ile elde edilecek performans kazancı, önbellek geçersizleştirme karmaşıklığının getirdiği ek yükü karşılamıyordu — özellikle Admin tarafında sık gerçekleşen create/update/delete/status-change işlemlerinde önbelleği senkron tutma ihtiyacı, sayfalanmış veri yapısında anlamını yitiriyordu. Bu nedenle caching yaklaşımından bilinçli olarak vazgeçilip yerine daha basit ve mevcut kullanım senaryosuna daha uygun olan sayfalama mimarisi benimsendi.

`ICacheService` ve `CacheManager` sınıfları bu karardan sonra silinmedi. DI container'a register edilmedikleri ve hiçbir yerden çağrılmadıkları için çalışma zamanında sıfır maliyetlidirler, ama kodda kalmalarının bir nedeni var: servisler zaten `IMemoryCache`'e değil `ICacheService` arayüzüne bağımlı olacak şekilde tasarlanmıştı. Trafik ileride önbellekleme gerektirecek boyuta ulaşırsa, Controller/Service tarafında tek satır değiştirmeden yalnızca `CacheManager`'ın DI kaydı açılarak (veya dağıtık bir `RedisCacheManager` ile değiştirilerek) sistem ölçeklenebilir. `BusinessServiceExtension.cs` içindeki yorum satırı hâline getirilmiş kayıt, bu kodun unutulmuş değil bilinçli olarak beklemede tutulan bir mimari tercih olduğunu gösterir.

Sayfalama, Admin ve Public tarafta aynı sözleşmeyle (`TGetFilteredListAsync(predicate, page, pageSize)`) çalışır; toplam sayfa sayısı `ICounterService` üzerinden ayrı bir sayım sorgusuyla hesaplanır. Mesaj gelen kutusunda bu prensip bir adım öteye taşınır: "Tümü / Okunmamış / Silinmiş" sekmelerinin her biri kendi sayfa numarasını bağımsız tutar ve yalnızca o an aktif olan sekmenin verisi veritabanından çekilir — üç liste her istekte gereksiz yere sorgulanmaz.

---

## Ölçeklenebilirlik ve Sürdürülebilirlik

Business ve WebUI katmanları yalnızca `IGenericDal<T>` / `I<Entity>Dal` arayüzlerine bağımlı olduğundan, MongoDB'den başka bir veri kaynağına geçilmesi gerekirse bu değişim yalnızca DataAccess katmanındaki implementasyonların yeniden yazılmasını gerektirir; Business ve WebUI kodu değişmez. Sayfalama, liste sayfalarının veri büyüklüğünden bağımsız, sabit maliyetli bir okuma yapmasını sağlar. Veri hacmi büyüdükçe `GetFilteredListAsync`'in filtrelediği alanlar (`Status`, `Category`, `IsDeleted`) için MongoDB tarafında bileşik index tanımlanması gündeme gelecek bir sonraki adımdır — şu an bu alanlarda açık bir index yoktur.

Sürdürülebilirlik tarafında, her entity aynı klasörleme kalıbını (`Dto/<Entity>Dtos`, `Business/Validators/<Entity>Validators`, `Business/Abstract`+`Concrete`) ve aynı isimlendirme konvansiyonunu (`Create<Entity>Dto`, `I<Entity>Service`, `<Entity>Manager`) izler; bu, projeye yeni katılan bir geliştiricinin bir entity'nin yapısını çözdüğünde diğerlerini de aynı kalıpla anlayabilmesini sağlar. Rol adları (`Roles.Admin`, `Roles.Manager`) tek bir sabitler sınıfında tutulur, controller'larda magic string olarak tekrarlanmaz. Yeni bir entity eklerken yapılması gereken adımlar bellidir ve dağınık değildir: DataAccess'e repository kaydı, Business'a servis kaydı; validator kaydı derleme taraması sayesinde otomatiktir.

---

## Bilinen Sınırlamalar

Bu bölüm, projeyi olduğundan iyi göstermek yerine yapılan tercihlerin maliyetini şeffaf şekilde ortaya koymak için var:

- **Test projesi yoktur.** Doğrulama şu ana kadar manuel yapılmıştır; iş kurallarının regresyona karşı otomatik güvence altına alınmaması, büyüyen bir kod tabanında ileride risk oluşturabilir.
- **`ICacheService` aktif değildir.** Yukarıda gerekçelendirildiği gibi bilinçli bir tercihtir, ancak devreye alındığında gerçek koşullarda sorunsuz çalışacağı henüz doğrulanmamıştır.
- **`ExceptionMiddleware` yalnızca referans amaçlıdır**, bir Web API katmanı eklenmeden önce gerçek koşullarda test edilmemiştir.

---

Bu doküman, projenin mevcut kod tabanına birebir sadık kalınarak hazırlanmıştır; anlatılan hiçbir davranış ya da karar, kodda karşılığı olmayan bir varsayıma dayanmaz.
