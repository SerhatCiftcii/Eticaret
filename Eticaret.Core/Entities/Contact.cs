using System.ComponentModel.DataAnnotations;

namespace Eticaret.Core.Entities
{
    public class Contact:IEntity
    {
        public int Id { get; set; }
        [Display(Name = "Ad")]
        public string Name { get; set; }
        [Display(Name = "Soyad")]
        public string Surname { get; set; }

        public string? Email { get; set; }
        [Display(Name = "Telefon")]
        public string? Phone { get; set; }
        [Display(Name = "Mesaj")]
        public string Message { get; set; }
        [Display(Name = "Kayıt Tarihi"), ScaffoldColumn(false)]  // scaffoldcolumn false ile viewde gözükmesini engelledik
        public DateTime CreateDate { get; set; }= DateTime.Now;
    }
}
