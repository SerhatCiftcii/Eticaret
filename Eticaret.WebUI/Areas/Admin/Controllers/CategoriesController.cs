using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Eticaret.Core.Entities;
using Eticaret.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Eticaret.WebUI.Utils;
using Microsoft.AspNetCore.Authorization;//SelectList

namespace Eticaret.WebUI.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Policy = "AdminPolicy")]
    public class CategoriesController : Controller
    {
        private readonly DatabaseContext _context;

        public CategoriesController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: Admin/Categories
        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories.ToListAsync());
        }

        // GET: Admin/Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Admin/Categories/Create
        public async Task<IActionResult> Create()
        {
            var anaKategoriler = await _context.Categories
      .Where(c => c.ParentId == 0)
      .ToListAsync();

            ViewBag.Kategoriler = new SelectList(anaKategoriler, "Id", "Name");
            return View();

        }


        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category, IFormFile? Image)
        {
            if (ModelState.IsValid)
            {
                category.Image = await FileHelper.FileLoaderAsync(Image,"/Img/Categories/");
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var anaKategoriler = await _context.Categories
               .Where(c => c.ParentId == 0)
               .ToListAsync();

            ViewBag.Kategoriler = new SelectList(anaKategoriler, "Id", "Name");
            return View(category);
        }

        // GET: Admin/Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            // Ana kategorileri çek ve mevcut parent ID'yi seçili yap
            var anaKategoriler = await _context.Categories
                                              .Where(c => c.ParentId == 0 || c.Id == category.ParentId)
                                              .ToListAsync();

            ViewBag.Kategoriler = new SelectList(anaKategoriler, "Id", "Name", category.ParentId);
            return View(category);
        }



        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category, IFormFile? Image, bool DeleteLogo = false)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Kendini parent seçmesini engelle
                    if (category.ParentId == category.Id)
                    {
                        ModelState.AddModelError(string.Empty, "Bir ana kategori kategori kendisini üst kategori olarak seçemez.");
                        return View(category);
                    }
                    if (DeleteLogo)
                    {
                        category.Image = string.Empty;
                    }
                    if (Image is not null)
                        category.Image = await FileHelper.FileLoaderAsync(Image, "/Img/Categories/");

                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Categories.Any(c => c.Id == category.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            var anaKategoriler = await _context.Categories
                                              .Where(c => c.ParentId == 0 || c.Id == category.ParentId)
                                              .ToListAsync();

            ViewBag.Kategoriler = new SelectList(anaKategoriler, "Id", "Name", category.ParentId);
            return View(category);
        }



        // GET: Admin/Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Admin/Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                if (!string.IsNullOrEmpty(category.Image))//boş değilse sunucdan siilicez
                {
                    FileHelper.FileRemover(category.Image,"/Img/Categories/");
                }
                _context.Categories.Remove(category);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}
