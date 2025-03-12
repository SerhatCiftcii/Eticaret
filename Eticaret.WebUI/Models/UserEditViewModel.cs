using System.ComponentModel.DataAnnotations;
using Eticaret.Core.Entities;
namespace Eticaret.WebUI.Models
{
    public class UserEditViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Ad")]

        [Required(ErrorMessage = "Adınızı girmeniz gerekmektedir.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Soyadınızı girmeniz gerekmektedir.")]
        [Display(Name = "Soyadı")]
        public string Surname { get; set; }
        [DataType(DataType.EmailAddress), Required(ErrorMessage = "Email boş geçilemez")]
        [Display(Name = "E-Posta")]
        public string Email { get; set; }
        [Display(Name = "Telefon")]
        public string? Phone { get; set; }
    }
}
