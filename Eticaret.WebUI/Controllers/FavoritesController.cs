using Eticaret.Core.Entities;
using Eticaret.Data;
using Eticaret.Service.Abstract;
using Eticaret.WebUI.ExtensionMethods;
using Microsoft.AspNetCore.Mvc;

namespace Eticaret.WebUI.Controllers
{
    public class FavoritesController : Controller
    {
        //private readonly DatabaseContext _context;

        //public FavoritesController(DatabaseContext context)
        //{
        //    _context = context;
        //}

        private readonly IService<Product> _service;
        public FavoritesController(IService<Product> service)
        {
            _service = service;

        }

        public IActionResult Index()
        {
            // Favori ürünleri almak
            var favoriler = GetFavorites();
            return View(favoriler);
        }
        private List<Product> GetFavorites()
        {
            return HttpContext.Session.GetJson<List<Product>>("GetFavorites")?? [];
              // Eğer favoriler boşsa, yeni liste döndür
        }

        // Favorilere ürün ekleme
        public async Task<IActionResult> Add(int ProductId)
        {
            var favoriler = GetFavorites();  // Favori listesini al
            var product = await _service.FindAsync(ProductId);  // Ürünü bul

            if (product != null && !favoriler.Any(p => p.Id == product.Id))  // Ürün favoriye eklenmemişse
            {
                favoriler.Add(product);  // Favorilere ekle
                HttpContext.Session.SetJson("GetFavorites",favoriler);  // Yeni favori listesini session'a kaydet
            }

            return RedirectToAction("Index");
        }

        // Favori ürünleri Session'dan al
  

        // Favori ürünleri Session'a kaydet
      

        public async Task<IActionResult> Remove(int ProductId)
        {
            var favoriler = GetFavorites();  // Favori listesini al
            var product = await _service.FindAsync(ProductId);  // Ürünü bul
            if (product != null && favoriler.Any(p => p.Id == product.Id))  // Ürün favoriye eklenmişse
            {
                favoriler.RemoveAll(i=>i.Id==product.Id);  // Favorilerden çıkar
                HttpContext.Session.SetJson("GetFavorites", favoriler);   // Yeni favori listesini session'a kaydet
            }
            return RedirectToAction("Index");
        }
    }
}
