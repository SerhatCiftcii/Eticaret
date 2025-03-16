using Eticaret.Core.Entities;
using Eticaret.Data;
using Eticaret.Service.Abstract;
using Eticaret.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eticaret.WebUI.Controllers
{
    public class ProductsController : Controller
    {
        /*   private readonly DatabaseContext _context;
           public ProductsController(DatabaseContext context)
           {
               _context = context;
           }*/

        private readonly IService<Product> _service;
        public ProductsController(IService<Product> service)
        {
            _service = service;
        }

        public async Task <IActionResult> Index(string search ="")
        {
            var database = await _service.GetAllAsync(x => x.IsActive &&
               (x.Name.ToLower().Contains(search.ToLower()) ||
                x.Description.ToLower().Contains(search.ToLower())));
            return View(database);

        }
        public async Task <IActionResult> Details(int? id) 
        {
            if (id == null)
            {
                return NotFound();
            }
            var product= await _service.GetQueryable().Include(x=>x.Brand).Include(x=>x.Category).FirstOrDefaultAsync(x=>x.Id==id);
            if (product == null) 
            { 
                return NotFound();
            }
            var model = new ProductDetailViewModel()
            {
                Product = product,
                RelatedProducts= _service.GetQueryable().Where(x => x.IsActive && x.CategoryId == product.CategoryId && x.Id != product.Id )
            };
            return View(model);
        }
    }
}
