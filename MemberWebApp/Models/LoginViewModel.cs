using System.ComponentModel.DataAnnotations;

namespace MemberWebApp.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(30, ErrorMessage = "Username can be max 30 char.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6,ErrorMessage ="Password is can be min 6 char.")]
        [MaxLength(16,ErrorMessage ="Password is can be max 16 char.")]
        public string Password { get; set; }
    }

}
