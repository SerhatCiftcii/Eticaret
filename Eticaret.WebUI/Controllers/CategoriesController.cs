using Eticaret.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Eticaret.WebUI.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly DatabaseContext _context;
        public CategoriesController(DatabaseContext context)
        {
            _context = context;
        }

        public async Task <IActionResult> Index(int? id)
        {
            if (id==null)
            {
                return NotFound();
            }
            var category= await _context.Categories.Include(x=>x.Products).FirstOrDefaultAsync(x=> x.Id == id);

            if(category == null)
            {
                return NotFound();
            }   
            return View(category);
        }
    }
}
