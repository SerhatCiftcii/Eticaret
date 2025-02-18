using System.ComponentModel.DataAnnotations;

namespace Eticaret.Core.Entities
{
    public class Product:IEntity
    {
        public int Id { get; set; }
        [Display(Name = "Ürün Adı")]
        public string Name { get; set; }
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }
        [Display(Name = "Resim")]
        public string? Image { get; set; }
        [Display(Name = "Fiyat")]
        public Decimal Price { get; set; }
        [Display(Name = "Ürün Kodu")]
        public string? ProductCode { get; set; }//ürünkodu
        [Display(Name = "Stok")]
        public int Stock { get; set; }
        [Display(Name = "Aktif Mi?")]
        public bool IsActive { get; set; }
        [Display(Name = "Anasayfada Göster?")]
        public bool IsHome { get; set; }
        [Display(Name = " Kategori")]
        public int? CategoryId { get; set; }
        [Display(Name = " Kategori")]
        public Category? Category { get; set; }
        [Display(Name = " Marka")]
        public int? BrandId { get; set; }
        [Display(Name = " Marka")]
        public Brand? Brand { get; set; }
        [Display(Name = "Sıra No")]
        public string OrderNo { get; set; }//sıralama
        [Display(Name = "Kayıt Tarihi"), ScaffoldColumn(false)]  // scaffoldcolumn false ile viewde gözükmesini engelledik
        public DateTime CreateDate { get; set; }
    }
}
