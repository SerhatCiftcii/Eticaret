using Microsoft.AspNetCore.Mvc;

namespace Eticaret.WebUI.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
