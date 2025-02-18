using System.ComponentModel.DataAnnotations;

namespace Eticaret.Core.Entities
{
    public class News : IEntity
    {
        public int Id { get; set; }
        [Display(Name = "Haber Adı")]
        public string Name { get; set; }
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }
        [Display(Name = "Resim")]
        public string? Image { get; set; }
        [Display(Name = "Aktif Mi?")]
        public bool IsActive { get; set; }
        [Display(Name = "Kayıt Tarihi"), ScaffoldColumn(false)]  // scaffoldcolumn false ile viewde gözükmesini engelledik
        public DateTime CreateDate { get; set; } = DateTime.Now;
    }
}
