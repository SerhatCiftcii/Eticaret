using Eticaret.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eticaret.WebUI.Controllers
{
    public class ProductsController : Controller
    {
        private readonly DatabaseContext _context;
        public ProductsController(DatabaseContext context)
        {
            _context = context;
        }
        public async Task <IActionResult> Index()
        {
            var database= await _context.Products.Where(x=>x.IsActive).Include(x=>x.Category).Include(x=>x.Brand).ToListAsync();
            return View(database);
        }
        public async Task <IActionResult> Details(int? id) 
        {
            if (id == null)
            {
                return NotFound();
            }
            var product= await _context.Products.Include(x=>x.Brand).Include(x=>x.Category).FirstOrDefaultAsync(x=>x.Id==id);
            if (product == null) 
            { 
                return NotFound();
            }
            return View(product);
        }
    }
}
