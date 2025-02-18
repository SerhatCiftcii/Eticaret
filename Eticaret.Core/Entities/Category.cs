using System.ComponentModel.DataAnnotations;

namespace Eticaret.Core.Entities
{
    public class Category : IEntity
    {
        public int Id { get; set; }
        [Display(Name = "Kategori Adı")]
        public string Name { get; set; }
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }
        [Display(Name = "Resim")]
        public string? Image { get; set; }
        [Display(Name = "Aktif Mi?")]
        public bool IsActive { get; set; }
        [Display(Name = "Üst Menüde Göster?")]
        public bool IsTopMenu { get; set; }//katogriy üst menuda gözüksün mü
        [Display(Name = "Üst Kategori")]
        public int? ParentId { get; set; }//ana katogry alt kategory oluturma
        [Display(Name = "Sıra No")]
        public int OrderNo { get; set; }//sıralama
        [Display(Name = "Kayıt Tarihi"), ScaffoldColumn(false)]  // scaffoldcolumn false ile viewde gözükmesini engelledik
        public DateTime CreateDate { get; set; }=DateTime.Now;
        public IList<Product>? Products { get; set; }
    }
}
