using System.ComponentModel.DataAnnotations;

namespace Eticaret.Core.Entities
{
    public class Brand: IEntity
    {
        public int Id { get; set; }
        [Display(Name = "Marka Adı")]
        public string Name { get; set; }
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }
        public string? Logo { get; set; }
        [Display(Name = "Aktif Mi?")]
        public bool IsActive { get; set; }
        [Display(Name = "Sıra No")]
        public int OrderNo { get; set; }//sıralama
        [Display(Name = "Kayıt Tarihi"), ScaffoldColumn(false)]  // scaffoldcolumn false ile viewde gözükmesini engelledik
        public DateTime CreateDate { get; set; }
        public IList<Product>? Products { get; set; }



    }
}
