using Eticaret.Core.Entities;
using Eticaret.Data;
using Eticaret.Service.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Eticaret.WebUI.Controllers
{
    public class CategoriesController : Controller
    {
        //private readonly DatabaseContext _context;
        //public CategoriesController(DatabaseContext context)
        //{
        //    _context = context;
        //}
        private readonly IService<Category> _service;

        public CategoriesController(IService<Category> service)
        {
            _service = service;

        }
        public async Task <IActionResult> Index(int? id)
        {
            if (id==null)
            {
                return NotFound();
            }
            var category= await _service.GetQueryable().Include(x=>x.Products).FirstOrDefaultAsync(x=> x.Id == id);

            if(category == null)
            {
                return NotFound();
            }   
            return View(category);
        }
    }
}
