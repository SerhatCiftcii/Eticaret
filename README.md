# 🛒 E-Ticaret Projesi – ASP.NET Core 8.0

Bu proje, **ASP.NET Core 8.0 MVC**, **Entity Framework Core (Code-First + Fluent API)** ve **katmanlı mimari** kullanılarak geliştirilmiş temel bir e-ticaret web uygulamasıdır. Yönetim paneli ve kullanıcı dostu arayüzü ile mobil uyumludur.

---

## 🔧 Kullanılan Teknolojiler

- ASP.NET Core 8.0  
- Entity Framework Core (Code-First + Fluent API)  
- MSSQL Server  
- HTML5, CSS3, Bootstrap 5  
- JavaScript, jQuery  
- SMTP (MailKit veya System.Net.Mail)  
- Iyzico Sanal POS (örnek entegrasyon yapısı)  
- Git ve GitHub  

---

## 🎯 Temel Özellikler

### ✅ Mimari
- Katmanlı Mimari: **UI**, **Business**, **Data Access**, **Entity**
- Repository Pattern ve Dependency Injection desteği (Generic Repository yapısı + CartService gibi özel servisler)

### ✅ Admin Panel
- Kullanıcı Yönetimi  
- Ürün / Kategori / Slider / Sipariş / İletişim / Adres Bilgileri / Marka Yönetimi / Kampanya Duyuruları / Stok Takip
- Anlaşılır ve sade bir Dashboard (Index) sayfası

### ✅ Kullanıcı İşlemleri
- Üye Ol / Giriş Yap / Şifremi Unuttum (Doğrulama Kodlu)
- Ürün Listeleme / Detay Görüntüleme
- Sepet İşlemleri ve Sipariş Oluşturma  
- Adres Tanımlama  
- İlişkili Ürünler  
- Session bazlı sepet yönetimi  

### ✅ E-Posta (SMTP) Bildirimleri
- Kayıt sonrası e-posta onayı  
- Sipariş bildirimi  
- Şifre sıfırlama için doğrulama kodu gönderimi  

---

## 💳 Sanal POS Entegrasyonu (Iyzico)

Projede örnek bir **Iyzico ödeme entegrasyonu** yapısı bulunmaktadır.  
Gerçek API bilgileri girildiğinde sistem canlı alışverişe açılabilir.  
Iyzico'nun .NET SDK'sı ve **sandbox sanal kartları** ile test edebilirsiniz.  

Kaynak: [Iyzico .NET GitHub](https://github.com/iyzico/iyzipay-dotnet)  
Test kart bilgileri: [sandbox.iyzipay.com](https://sandbox-merchant.iyzipay.com/)

---

## 📧 SMTP Ayarları

SMTP üzerinden doğrulama ve bildirim e-postaları gönderilmektedir.  
Aşağıdaki ayarları `appsettings.json` içerisine ekleyiniz:

```json
"EmailSettings": {
  "Host": "smtp.gmail.com",
  "Port": 587,
  "EnableSSL": true,
  "UserName": "youremail@gmail.com",
  "Password": "yourapppassword",
  "From": "youremail@gmail.com"
}

+++++++++++++---+++++++
--------------------
 Admin Test Kullanıcısı
Projeye dahil edilen hazır bir admin hesabı ile giriş yapılabilir:

E-Posta: admin@serhat.com

Şifre: 12345Aa

Bu kullanıcı ile Admin Panel'e erişerek tüm içerik yönetim modüllerini test edebilirsiniz.

🗄️ Veritabanı Yapılandırması (SQL Server)
Veritabanı bağlantısı, Eticaret.Data altındaki DatabaseContext sınıfında yapılandırılmıştır. Buradaki bağlantı dizesi OnConfiguring metodu içinde belirtilmiştir.

Eğer bağlanmak istediğiniz veritabanı EcommerceDb ve yerel SQL Server'ı kullanıyorsanız, bağlantı dizesi şu şekilde olmalıdır:

DatabaseContext Sınıfı - OnConfiguring Yapılandırması

public class DatabaseContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    // Diğer DbSet'ler

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Server=SERHAT\SQLEXPRESS; 
                                      Database=EcommerceDb; 
                                      Trusted_Connection=True; 
                                      TrustServerCertificate=True;");
        base.OnConfiguring(optionsBuilder);
    }
}
Server=SERHAT\SQLEXPRESS; → Yerel makinenizdeki SQL Server'ın adı (SQLEXPRESS burada kullanılan instance adı).

Database=EcommerceDb; → Kullanılacak veritabanı adı. Burada EcommerceDb olarak belirtilmiştir.

Trusted_Connection=True; → Windows kimlik doğrulaması kullanılır.

TrustServerCertificate=True; → SSL sertifikası doğrulamasını devre dışı bırakır, bu genellikle geliştirme ortamı için gereklidir.

-Migration işlemleri sonrası veritabanı EF Core tarafından otomatik olarak oluşturulur:.

dotnet ef migrations add InitialCreate
dotnet ef database update

