using System.ComponentModel.DataAnnotations;

namespace MemberWebApp.Models
{
    public class RegisterViewModel:LoginViewModel
    {

        [Required(ErrorMessage = "RePassword is required")]
        [MinLength(6), MaxLength(16)]
        [Compare(nameof(Password),ErrorMessage ="Password and Repassword eşleşmiyor")]
        public string RePassword { get; set; }
    }

}
