using Eticaret.Core.Entities;
using Eticaret.Data;
using Eticaret.Service.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eticaret.WebUI.ViewComponents
{
    public class Categories : ViewComponent
    {
        /*     private readonly DatabaseContext _context;

             public Categories(DatabaseContext context)
             {
                 _context = context;
             }*/

        private readonly IService<Category> _service;

        public Categories(IService<Category> service)
        {
            _service = service;
        }

        public async Task <IViewComponentResult> InvokeAsync()
        {
       var categories= await _service.GetAllAsync(x=>x.IsActive && x.IsTopMenu);

            return View(categories);
        }
    }
}
