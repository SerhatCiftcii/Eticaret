using System.Diagnostics;
using Eticaret.Core.Entities;
using Eticaret.Data;
using Eticaret.WebUI.Models;
using Eticaret.WebUI.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Eticaret.WebUI.Controllers
{
    public class HomeController : Controller
    {
        //slider için databse context ve indexe model eklendi  categories de component kullandýðýmýz için database çekmemize gerek kalmamýþtý
        private readonly DatabaseContext _context;
        private readonly IEmailService _emailService;
        public HomeController(DatabaseContext context,IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<IActionResult> Index()
        {
            var model = new HomePageViewModel()
            {
                Sliders = await _context.Sliders.ToListAsync(),
                News = await _context.News.ToListAsync(),
                Products = await _context.Products.Where(x => x.IsActive && x.IsHome).ToListAsync()
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
                    // Admin'e e-posta gönderme
                    string adminSubject = "Siteden mesaj geldi";
                    string adminBody = $"Ýsim: {contact.Name} - Soyisim: {contact.Surname} - Email: {contact.Email} - Telefon: {contact.Phone} - Mesaj: {contact.Message}";
                    await _emailService.SendEmailAsync("s.ciftci561@gmail.com", adminSubject, adminBody);  // Admin maili

                    // Kullanýcýya e-posta gönderme
                    string userSubject = "Mesajýnýz baþarýyla alýndý";
                    string userBody = $"Merhaba {contact.Name},\n\nMesajýnýz baþarýyla alýndý. En kýsa sürede size geri dönüþ yapacaðýz.\n\nMesajýnýz:\n{contact.Message}";
                    await _emailService.SendEmailAsync(contact.Email, userSubject, userBody);  // Kullanýcý maili

                    // ViewData ile baþarý mesajý gönderme
                    ViewData["Message"] = @"<div class=""alert alert-success alert-dismissible fade show"" role=""alert"">
                                    <strong>Mesajýnýz baþarýyla iletilmiþtir!</strong>
                                    <button type=""button"" class=""btn-close"" data-bs-dismiss=""alert"" aria-label=""Close""></button>
                                </div>";
                }
                catch (Exception ex)
                {
                    // Hata mesajý
                    ViewData["Message"] = @"<div class=""alert alert-danger alert-dismissible fade show"" role=""alert"">
                                    <strong>Mesaj gönderme sýrasýnda bir hata oluþtu!</strong>
                                    <button type=""button"" class=""btn-close"" data-bs-dismiss=""alert"" aria-label=""Close""></button>
                                </div>";
                }
            }

            return View(contact); // Ayný sayfada kalýnýr
        }
    }
}
