using System.ComponentModel.DataAnnotations;

namespace MemberWebApp.Controllers
{

        public class MemberUserModel
        {
            public Guid Id { get; set; }
            public string? FullName { get; set; }
            public string Username { get; set; }
            public bool Locked { get; set; } = false;
            public DateTime CreatedAt { get; set; } = DateTime.Now;
            public string? ProfileImageFileName { get; set; } = "default.png";
            public string Role { get; set; } = "user";
        }

        public class MemberCreateUserModel
        {
            [Required(ErrorMessage = "Username is required")]
            [StringLength(30, ErrorMessage = "Username can be max 30 char.")]
            public string Username { get; set; }

            [Required]
            [StringLength(50)]
            public string FullName { get; set; }
            public bool Locked { get; set; }

            [Required(ErrorMessage = "Password is required")]
            [MinLength(6, ErrorMessage = "Password is can be min 6 char.")]
            [MaxLength(16, ErrorMessage = "Password is can be max 16 char.")]
            public string Password { get; set; }

            [Required(ErrorMessage = "RePassword is required")]
            [MinLength(6, ErrorMessage = "RePassword is can be min 6 char.")]
            [MaxLength(16, ErrorMessage = "RePassword is can be max 16 char.")]
            [Compare(nameof(Password), ErrorMessage = "Password and Repassword do not match")]
            public string RePassword { get; set; }

            [Required]
            [StringLength(50)]
            public string Role { get; set; } = "user";

            public string? Done { get; set; }
        }

        public class MemberEditUserModel
        {
            public Guid Id { get; set; }
            [Required(ErrorMessage = "Username is required")]
            [StringLength(30, ErrorMessage = "Username can be max 30 char.")]
            public string Username { get; set; }

            [Required]
            [StringLength(50)]
            public string FullName { get; set; }
            public bool Locked { get; set; }
            [Required]
            [StringLength(50)]
            public string Role { get; set; } = "user";

            public string? Done { get; set; }
        }
    }

