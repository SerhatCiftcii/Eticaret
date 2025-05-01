using System.Diagnostics;
using Eticaret.Core.Entities;
using Eticaret.Data;
using Eticaret.Service.Abstract;
using Eticaret.WebUI.Models;
using Eticaret.WebUI.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eticaret.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IService<Product> _serviceProduct;
        private readonly IService<Slider> _serviceSlider;
        private readonly IService<News> _serviceNews;
        private readonly IService<Contact> _serviceContact;
        private readonly IEmailService _emailService;
        private readonly IService<ProductRating> _serviceProductRating; // ProductRating servisini ekledik

        public HomeController(IEmailService emailService,
                              IService<Product> serviceProduct,
                              IService<Slider> serviceSlider,
                              IService<News> serviceNews,
                              IService<Contact> serviceContact,
                              IService<ProductRating> serviceProductRating) // DI'den ProductRating servisini ald�k
        {
            _serviceProduct = serviceProduct;
            _serviceSlider = serviceSlider;
            _emailService = emailService;
            _serviceNews = serviceNews;
            _serviceContact = serviceContact;
            _serviceProductRating = serviceProductRating; // Servisi atad�k
        }

        // Sayfalama i�in index parametresi ekliyoruz
        public async Task<IActionResult> Index(int page = 1)
        {
            const int pageSize = 12;

            var allProducts = await _serviceProduct.GetAllAsync(x => x.IsActive && x.IsHome);
            var pagedProducts = allProducts.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var productIds = pagedProducts.Select(p => p.Id).ToList();
            var allRatings = await _serviceProductRating.GetAllAsync(r => productIds.Contains(r.ProductId));

            var ratingsByProduct = allRatings.GroupBy(r => r.ProductId)
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var product in pagedProducts)
            {
                if (ratingsByProduct.TryGetValue(product.Id, out var ratings) && ratings.Any())
                {
                    product.AvgRating = ratings.Average(r => r.Rating);
                }
                else
                {
                    product.AvgRating = 0;
                }
            }

            var model = new HomePageViewModel
            {
                Sliders = await _serviceSlider.GetAllAsync(),
                News = await _serviceNews.GetAllAsync(x => x.IsActive), // Kampanyalar her zaman gelir
                Products = pagedProducts,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)allProducts.Count / pageSize)
            };

            return View(model);
        }


        public IActionResult ContactUs()
        {
            return View();
        }

        [AutoValidateAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> ContactUs(Contact contact)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Veritaban�na ekleme
                    await _serviceContact.AddAsync(contact);
                    var sonuc = await _serviceContact.SaveChangesAsync();

                    if (sonuc > 0)
                    {
                        // Admin'e e-posta g�nderme
                        string adminSubject = "Eticaret Sitesinden Yeni Mesaj Geldi";
                        string adminBody = $@"
                        <h2>Yeni �leti�im Mesaj�</h2>
                        <table style='width: 100%; border-collapse: collapse;'>
                            <tr><th>Alan</th><th>Detay</th></tr>
                            <tr><td>�sim</td><td>{contact.Name}</td></tr>
                            <tr><td>Soyisim</td><td>{contact.Surname}</td></tr>
                            <tr><td>Email</td><td>{contact.Email}</td></tr>
                            <tr><td>Telefon</td><td>{contact.Phone}</td></tr>
                            <tr><td>Mesaj</td><td>{contact.Message}</td></tr>
                        </table>
                        <p style='color: #888;'>Bu mesaj e-ticaret siteniz �zerinden g�nderilmi�tir.</p>
                        ";
                        await _emailService.SendEmailAsync("s.ciftci561@gmail.com", adminSubject, adminBody);

                        // Kullan�c�ya e-posta g�nderme
                        string userSubject = "Mesaj�n�z Ba�ar�yla Al�nd� - E-Ticaret Sitesi";
                        string userBody = $@"
                        <div style='font-family: Arial, sans-serif;'>
                            <h3>Merhaba {contact.Name},</h3>
                            <p>Mesaj�n�z� ald�k! En k�sa s�rede sizinle ileti�ime ge�ece�iz.</p>
                            <h4>G�nderdi�iniz Mesaj:</h4>
                            <blockquote style='border-left: 4px solid #007bff; padding-left: 10px;'>{contact.Message}</blockquote>
                            <p>Te�ekk�r ederiz!</p>
                        </div>
                        ";
                        await _emailService.SendEmailAsync(contact.Email, userSubject, userBody);

                        // Ba�ar� mesaj�
                        TempData["Message"] = @"<div class='alert alert-success alert-dismissible fade show' role='alert'>
                                    <strong>Mesaj�n�z ba�ar�yla kaydedildi ve iletildi!</strong>
                                    <button type='button' class='btn-close' data-bs-dismiss='alert' aria-label='Close'></button>
                                </div>";
                    }
                    else
                    {
                        TempData["Message"] = @"<div class='alert alert-warning alert-dismissible fade show' role='alert'>
                                    <strong>Mesaj veritaban�na kaydedilemedi!</strong> L�tfen tekrar deneyin.
                                    <button type='button' class='btn-close' data-bs-dismiss='alert' aria-label='Close'></button>
                                </div>";
                    }
                }
                catch (Exception ex)
                {
                    TempData["Message"] = @"<div class='alert alert-danger alert-dismissible fade show' role='alert'>
                                <strong>Mesaj g�nderme s�ras�nda bir hata olu�tu!</strong>
                                <button type='button' class='btn-close' data-bs-dismiss='alert' aria-label='Close'></button>
                            </div>";
                }
            }

            return View(contact);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Route("AccessDenied")]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
