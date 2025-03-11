using System.ComponentModel.DataAnnotations;

namespace Eticaret.WebUI.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Ürün Adı")]
        public string Name { get; set; }
        
        [Display(Name = "Resim")]
        public string? Image { get; set; }

        [Display(Name = "Fiyat")]
        public Decimal Price { get; set; }

        // Sadece görüntüleme için ek özellik
        public bool IsFavorite { get; set; }

    }
}
