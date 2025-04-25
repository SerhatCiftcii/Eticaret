using Eticaret.Core.Entities;
using Eticaret.Service.Abstract;
using Eticaret.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Eticaret.WebUI.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminPolicy")]
    public class MainController : Controller
    {
        private readonly IService<Product> _productService;
        private readonly IService<Category> _categoryService;
        private readonly IService<AppUser> _appUserService;
        private readonly IService<Order> _orderService;

        public MainController(IService<Product> productService,
                                    IService<Category> categoryService,
                                    IService<AppUser> appUserService,
                                    IService<Order> orderService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _appUserService = appUserService;
            _orderService = orderService;
        }

        public async Task<IActionResult> Index()
        {
            var totalProductCount = (await _productService.GetAllAsync()).Count;
            var activeCategoryCount = (await _categoryService.GetAllAsync(c => c.IsActive)).Count;
            var totalCategoryCount = (await _categoryService.GetAllAsync()).Count;
            var lowStockProductCount = (await _productService.GetAllAsync(p => p.Stock < 5)).Count;
            var totalCustomerCount = (await _appUserService.GetAllAsync(u => !u.IsAdmin)).Count;
            var newOrderCountLast24Hours = (await _orderService.GetAllAsync(o => o.OrderDate >= DateTime.Now.AddDays(-1))).Count;
            var pendingOrderCount = (await _orderService.GetAllAsync(o => o.OrderState == Core.Entities.EnumOrderState.Waiting)).Count;

            // Aktif kategorileri al
            var activeCategories = await _categoryService.GetAllAsync(c => c.IsActive);
            var activeCategoryNames = activeCategories.Select(c => c.Name).ToList();

            var viewModel = new AdminDashboardViewModel
            {
                TotalProductCount = totalProductCount,
                ActiveCategoryCount = activeCategoryCount,
                TotalCategoryCount = totalCategoryCount,
                LowStockProductCount = lowStockProductCount,
                TotalCustomerCount = totalCustomerCount,
                NewOrderCountLast24Hours = newOrderCountLast24Hours,
                PendingOrderCount = pendingOrderCount,
                CategoryNames = activeCategoryNames
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<JsonResult> GetCategoryNames(bool? isActive)
        {
            try
            {
                List<string> categoryNames;

                if (isActive.HasValue)
                {
                    categoryNames = (await _categoryService.GetAllAsync(c => c.IsActive == isActive.Value))
                                    .Select(c => c.Name)
                                    .ToList();
                }
                else
                {
                    categoryNames = (await _categoryService.GetAllAsync())
                                    .Select(c => c.Name)
                                    .ToList();
                }

                return Json(categoryNames);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        public async Task<IActionResult> LowStockProducts()
        {
            var lowStockProducts = await _productService.GetAllAsync(p => p.Stock < 5);
            return View(lowStockProducts);
        }
    }
}