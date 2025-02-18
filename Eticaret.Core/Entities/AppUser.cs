
using System.ComponentModel.DataAnnotations;

namespace Eticaret.Core.Entities
{
    public class AppUser :IEntity
    {
        public int Id { get; set; }
        [Display(Name ="Ad")]
        public string Name { get; set; }
        [Display(Name = "Soyadı")]
        public string Surname { get; set; }
        public string Email { get; set; }
        [Display(Name = "Telefon")]
        public string? Phone { get; set; }
        [Display(Name = "Şifre")]
        public string Password { get; set; }
        [Display(Name = "Kullanıcı Adı")]
        public string? UserName { get; set; }
        [Display(Name = "Aktif mi?")]
        public bool IsActive { get; set; } //kullanıcı aktif pasif
        [Display(Name = "Admin mi?")]
        public bool IsAdmin { get; set; } //kullanıcı admin mi
        [Display(Name ="Kayıt Tarihi"),ScaffoldColumn(false)]  // scaffoldcolumn false ile viewde gözükmesini engelledik
        public DateTime CreateDate { get; set; }=DateTime.Now; //kullanıcı oluşturulma tarihi
        [ScaffoldColumn(false)]
        public Guid? UserGuid { get; set; }= Guid.NewGuid();  
        //public DateTime? UpdateDate { get; set; }


    }
}
