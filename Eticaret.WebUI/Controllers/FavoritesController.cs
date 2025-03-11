using Eticaret.Core.Entities;
using Eticaret.Data;
using Eticaret.WebUI.ExtensionMethods;
using Eticaret.WebUI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eticaret.WebUI.Controllers
{
    public class FavoritesController : Controller
    {
        private readonly DatabaseContext _context;

        public FavoritesController(DatabaseContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Favori ürünleri almak
            var favoriler = GetFavorites();
            return View(favoriler);
        }

        // Favorilere ürün ekleme
        public async Task<IActionResult> Add(int ProductId)
        {
            var favoriler = GetFavorites();  // Favori listesini al
            var product = await _context.Products.FindAsync(ProductId);  // Ürünü bul

            if (product != null && !favoriler.Any(p => p.Id == product.Id))  // Ürün favoriye eklenmemişse
            {
                favoriler.Add(product);  // Favorilere ekle
                SetFavorites(favoriler);  // Yeni favori listesini session'a kaydet
            }

            return RedirectToAction("Index");
        }

        // Favori ürünleri Session'dan al
        private List<Product> GetFavorites()
        {
            var favorites = HttpContext.Session.GetJson<List<Product>>("GetFavorites");
            return favorites ?? new List<Product>();  // Eğer favoriler boşsa, yeni liste döndür
        }

        // Favori ürünleri Session'a kaydet
        private void SetFavorites(List<Product> favorites)
        {
            HttpContext.Session.SetJson("GetFavorites", favorites);
        }
    }
}
