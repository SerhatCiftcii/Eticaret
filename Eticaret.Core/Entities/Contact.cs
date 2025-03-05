using System.ComponentModel.DataAnnotations;

namespace Eticaret.Core.Entities
{
    public class Contact : IEntity
    {
        public int Id { get; set; }

        [Display(Name = "Ad")]
        [Required(ErrorMessage = "Adınız gereklidir.")]
        public string Name { get; set; }

        [Display(Name = "Soyad")]
        [Required(ErrorMessage = "Soyadınız gereklidir.")]
        public string Surname { get; set; }

        [EmailAddress(ErrorMessage = "Lütfen geçerli bir e-posta adresi girin.")]
        [Required(ErrorMessage = "Eposta Adresini giriniz.")]
        [Display(Name = "E-posta")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Lütfen geçerli bir telefon numarası girin.")]
        [Display(Name = "Telefon")]
        
        public string? Phone { get; set; }

        [Display(Name = "Mesaj")]
        [Required(ErrorMessage = "Mesajınız gereklidir.")]
        public string Message { get; set; }

        [Display(Name = "Kayıt Tarihi"), ScaffoldColumn(false)]  // scaffoldcolumn false ile viewde gözükmesini engelledik
        public DateTime CreateDate { get; set; } = DateTime.Now;
    }
}
