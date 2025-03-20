using Eticaret.Core.Entities;
using Eticaret.Service.Abstract;
using Eticaret.Service.Concrete;
using Eticaret.WebUI.ExtensionMethods;
using Microsoft.AspNetCore.Mvc;

namespace Eticaret.WebUI.Controllers
{
    public class CartController : Controller
    {
        private readonly IService<Product> _serviceProduct;
        public CartController(IService<Product> serviceProduct)
        {
            _serviceProduct= serviceProduct;
        }

        public IActionResult Index()
        {
            return View();
        } 
        public IActionResult Add(int ProuctId, int quantitiy=1)
        {
            var product = _serviceProduct.Find(ProuctId);
            if (product != null)
            {
                var cart = GetCart();
                cart.AddProduct(product,quantitiy);
                HttpContext.Session.SetJson("Cart", cart);
            }
            return RedirectToAction("Index");
        }
        public IActionResult Update(int ProuctId, int quantitiy=1)
        {
            var product = _serviceProduct.Find(ProuctId);
            if (product != null)
            {
                var cart = GetCart();
                cart.UpdateProduct(product,quantitiy);
                HttpContext.Session.SetJson("Cart", cart);
            }
            return RedirectToAction("Index");
        }  
        public IActionResult Remove(int ProuctId)
        {
            var product = _serviceProduct.Find(ProuctId);
            if (product != null)
            {
                var cart = GetCart();
                cart.RemoveProduct(product);
                HttpContext.Session.SetJson("Cart", cart);
            }
            return RedirectToAction("Index");
        }
      private CartService GetCart()
        {
            return HttpContext.Session.GetJson<CartService>("Cart") ?? new CartService();
        }
    }
}
