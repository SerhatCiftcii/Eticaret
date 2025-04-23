using Eticaret.Core.Entities;
using Eticaret.Data;
using Eticaret.Service.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eticaret.WebUI.Controllers
{
    [Authorize]
    public class ProductRatingsController : Controller
    {
        private readonly IService<ProductRating> _serviceProductRating;
        private readonly IService<Product> _serviceProduct;
        private readonly IService<AppUser> _serviceAppUser;
        private readonly IService<Order> _serviceOrder;
        private readonly IService<OrderLine> _serviceOrderLine;
        private readonly DatabaseContext _dbContext;

        public ProductRatingsController(
            IService<ProductRating> ratingService,
            IService<Product> productService,
            IService<AppUser> userService,
            IService<Order> orderService,
            IService<OrderLine> orderLineService,
            DatabaseContext dbContext)
        {
            _serviceProductRating = ratingService;
            _serviceProduct = productService;
            _serviceAppUser = userService;
            _serviceOrder = orderService;
            _serviceOrderLine = orderLineService;
            _dbContext = dbContext;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRating(int productId, int rating, string returnUrl)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return Json(new { success = false, message = "Lütfen giriş yapın." });
                }

                var userGuid = HttpContext.User.FindFirst("UserGuid")?.Value;
                if (string.IsNullOrEmpty(userGuid))
                {
                    return Json(new { success = false, message = "Kullanıcı bilgisi bulunamadı." });
                }

                var user = await _serviceAppUser.GetAsync(x => x.UserGuid.ToString() == userGuid);
                if (user == null)
                {
                    return Json(new { success = false, message = "Kullanıcı bulunamadı." });
                }

                var product = await _serviceProduct.GetAsync(p => p.Id == productId);
                if (product == null)
                {
                    return Json(new { success = false, message = "Ürün bulunamadı." });
                }

                var hasPurchased = await _dbContext.OrderLines
                    .Where(ol => ol.ProductId == productId &&
                                   ol.Order.AppUserId == user.Id &&
                                   (ol.Order.OrderState == EnumOrderState.Approved ||
                                    ol.Order.OrderState == EnumOrderState.Shipped ||
                                    ol.Order.OrderState == EnumOrderState.Completed))
                    .AnyAsync();

                if (!hasPurchased)
                {
                    return Json(new { success = false, message = "Bu ürünü puanlayabilmek için önce satın almanız gerekmektedir." });
                }

                var existingRating = await _serviceProductRating.GetAsync(r =>
                    r.AppUserId == user.Id && r.ProductId == productId);

                if (existingRating != null)
                {
                    existingRating.Rating = rating;
                    existingRating.RatedAt = DateTime.Now;
                    await _serviceProductRating.UpdateAsync(existingRating);
                }
                else
                {
                    var newRating = new ProductRating
                    {
                        ProductId = productId,
                        AppUserId = user.Id,
                        Rating = rating,
                        RatedAt = DateTime.Now
                    };
                    await _serviceProductRating.AddAsync(newRating);
                }

                var avgRating = await _serviceProductRating.GetAllAsync(r => r.ProductId == productId)
                                                        .ContinueWith(ratings => ratings.Result.Any() ? ratings.Result.Average(r => r.Rating) : 0);

                product.AvgRating = avgRating;
                await _serviceProduct.UpdateAsync(product);

                return Json(new { success = true, avgRating = product.AvgRating });
            }
            catch (Exception ex)
            {
                // Hata loglama mekanizması ekleyebilirsiniz.
                return Json(new { success = false, message = "Beklenmeyen bir hata oluştu." });
            }
        }
    }
}