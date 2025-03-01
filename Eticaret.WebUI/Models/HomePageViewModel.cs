using Eticaret.Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Eticaret.WebUI.Models
{
    public class HomePageViewModel : Controller
    {
      public List <Slider>? Sliders { get; set; }
      public List <Product>? Products { get; set; }
      public List <News>? News { get; set; }

    }
}
