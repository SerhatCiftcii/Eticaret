using System.Diagnostics;
using Eticaret.Core.Entities;
using Eticaret.Data;
using Eticaret.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eticaret.WebUI.Controllers
{
    public class HomeController : Controller
    {
        //slider için databse context ve indexe model eklendi  categories de component kullandýðýmýz için database çekmemize gerek kalmamýþtý
        private readonly DatabaseContext _context;

        public HomeController(DatabaseContext context)
        {
            _context = context;
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
        [HttpPost]
        public IActionResult ContactUs(Contact contact)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Contacts.Add(contact);
                    var sonuc = _context.SaveChanges();
                    if(sonuc > 0)
                    {
                        TempData["Messega"] = @"<div class=""alert alert-success alert-dismissible fade show"" role=""alert"">
  <strong>Mesajýnýz Ýletiþmiþtir</strong> En kýsa sürede dönüþ yapýlacaktýr.
  <button type=""button"" class=""btn-close"" data-bs-dismiss=""alert"" aria-label=""Close""></button>
</div>";
                        return RedirectToAction("ContactUs");
                    }
                  
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Hata oluþtu", ex.Message);
                }
            }
            return View(contact);
        }
    }
}
