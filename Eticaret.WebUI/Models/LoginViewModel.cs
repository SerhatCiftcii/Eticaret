using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eticaret.WebUI.Models
{
    public class LoginViewModel
    {
        [DataType(DataType.EmailAddress),Required(ErrorMessage ="Email boş geçilemez")]
        [Display(Name = "E-Posta")]
        public string Email { get; set; }

        [Display(Name = "Şifre"), Required(ErrorMessage = "Şifre boş geçilemez")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

       

        //geri dönüş url kullanıcı adres çubunğunda quersy string varsas geri döndürcek yapımız
        public string? ReturnUrl { get; set; }

        public bool rememberMe { get; set; }
    }
}
