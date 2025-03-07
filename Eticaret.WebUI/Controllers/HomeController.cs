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
        //slider i�in databse context ve indexe model eklendi  categories de component kulland���m�z i�in database �ekmemize gerek kalmam��t�
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
                    // Admin'e e-posta g�nderme
                    string adminSubject = "Siteden mesaj geldi";
                    string adminBody = $"�sim: {contact.Name} - Soyisim: {contact.Surname} - Email: {contact.Email} - Telefon: {contact.Phone} - Mesaj: {contact.Message}";
                    await _emailService.SendEmailAsync("s.ciftci561@gmail.com", adminSubject, adminBody);  // Admin maili

                    // Kullan�c�ya e-posta g�nderme
                    string userSubject = "Mesaj�n�z ba�ar�yla al�nd�";
                    string userBody = $"Merhaba {contact.Name},\n\nMesaj�n�z ba�ar�yla al�nd�. En k�sa s�rede size geri d�n�� yapaca��z.\n\nMesaj�n�z:\n{contact.Message}";
                    await _emailService.SendEmailAsync(contact.Email, userSubject, userBody);  // Kullan�c� maili

                    // ViewData ile ba�ar� mesaj� g�nderme
                    ViewData["Message"] = @"<div class=""alert alert-success alert-dismissible fade show"" role=""alert"">
                                    <strong>Mesaj�n�z ba�ar�yla iletilmi�tir!</strong>
                                    <button type=""button"" class=""btn-close"" data-bs-dismiss=""alert"" aria-label=""Close""></button>
                                </div>";
                }
                catch (Exception ex)
                {
                    // Hata mesaj�
                    ViewData["Message"] = @"<div class=""alert alert-danger alert-dismissible fade show"" role=""alert"">
                                    <strong>Mesaj g�nderme s�ras�nda bir hata olu�tu!</strong>
                                    <button type=""button"" class=""btn-close"" data-bs-dismiss=""alert"" aria-label=""Close""></button>
                                </div>";
                }
            }

            return View(contact); // Ayn� sayfada kal�n�r
        }
    }
}
