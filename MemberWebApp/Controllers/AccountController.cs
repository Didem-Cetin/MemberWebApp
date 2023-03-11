using MemberWebApp.Entities;
using MemberWebApp.Helpers;
using MemberWebApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NETCore.Encrypt.Extensions;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace MemberWebApp.Controllers
{
    [Authorize(AuthenticationSchemes =CookieAuthenticationDefaults.AuthenticationScheme)]
    public class AccountController : Controller
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IConfiguration _configuration;
        private readonly IHasher _hasher;
        public AccountController(DatabaseContext databaseContext, IConfiguration configuration, IHasher hasher)
        {
            _databaseContext = databaseContext;
            _configuration = configuration;
            _hasher = hasher;
        }
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                string hashedPassword = _hasher.DoMD5HashedString(model.Password);
                User user = _databaseContext.Users.SingleOrDefault(x => x.Username.ToLower() == model.Username.ToLower() && x.Password == hashedPassword);

                if (user != null)
                {
                    if (user.Locked)
                    {
                        ModelState.AddModelError(nameof(model.Username), "Username is locked");
                        return View(model);
                    }

                    List<Claim> claims = new List<Claim>();
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
                    claims.Add(new Claim(ClaimTypes.Name, user.FullName ?? string.Empty));
                    claims.Add(new Claim(ClaimTypes.Role, user.Role));
                    claims.Add(new Claim("Username", user.Username));


                    ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    return RedirectToAction("Index", "Home");

                }
                else
                {
                    ModelState.AddModelError("", "Username or Password is incorrect");
                }


            }
            return View(model);
        }

       
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_databaseContext.Users.Any(x => x.Username.ToLower() == model.Username.ToLower()))
                {
                    ModelState.AddModelError(nameof(model.Username), "Username is already exists");
                    return View(model);
                }
                string hashedPassword = _hasher.DoMD5HashedString(model.Password);

                //Kullanıcının girdiği Username i(model.Username) Database e atıyorum...
                User user = new User()
                {
                    Username = model.Username,
                    Password = hashedPassword
                };
                _databaseContext.Users.Add(user);
                int affectedRowCount = _databaseContext.SaveChanges();
                if (affectedRowCount == 0)
                {
                    ModelState.AddModelError("", "User can not be added");
                }
                else
                {
                    return RedirectToAction(nameof(Login));
                }
            }
            return View(model); //Hata gözüksün diye modeli dönüyopruz

        }

        public IActionResult Profile()
        {
            ProfileInfoLoader();

            return View();
        }

        private void ProfileInfoLoader()
        {
            Guid userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
            User user = _databaseContext.Users.SingleOrDefault(x => x.Id == userId);

            ViewData["FullName"] = user.FullName;
            ViewData["ProfileImage"] = user.ProfileImageFileName;

        }

        [HttpPost]
        public IActionResult ProfileChangeFullName([Required][StringLength(50)] string? fullname)
        {

            if (ModelState.IsValid)
            {
                Guid userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
                User user = _databaseContext.Users.SingleOrDefault(x => x.Id == userId);

                user.FullName = fullname;
                _databaseContext.SaveChanges();

                return RedirectToAction(nameof(Profile));

            }

            ProfileInfoLoader();

            return View("Profile");
        }

        [HttpPost]
        public IActionResult ProfileChangePassword([Required][MinLength(6)][MaxLength(16)] string? password)
        {

            if (ModelState.IsValid)
            {
                Guid userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
                User user = _databaseContext.Users.SingleOrDefault(x => x.Id == userId);

                string hashedPassword=_hasher.DoMD5HashedString(password);

                user.Password = hashedPassword;
                _databaseContext.SaveChanges();

                ViewData["result"] = "PasswordChanged";

            }

            ProfileInfoLoader();

            return View("Profile");
        }

        [HttpPost]
        public IActionResult ProfileChangeImage([Required] IFormFile file)
        {

            if (ModelState.IsValid)
            {


                Guid userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
                User user = _databaseContext.Users.SingleOrDefault(x => x.Id == userId);

                //p_789554-ASDFG.png
                string fileName = $"p_{userId}.png";  //Dosya adı oluşturduk
                string fileName2 = $"p_{userId}.{file.ContentType.Split('/')[1]}"; 
                //image/png  image/jpg , / dan parçalayıp png ya da jpg yi yani[1] i alabiliriz

                Stream stream = new FileStream($"wwwroot/uploads/{fileName}", FileMode.OpenOrCreate); //Streamimizi hangi pathe oluşturucağımızı belirledik.

                file.CopyTo(stream);//Dosyayı sunucuya kopyaladık

                stream.Close(); 
                stream.Dispose();



                user.ProfileImageFileName = fileName;
                _databaseContext.SaveChanges();

                return RedirectToAction(nameof(Profile));

            }

            ProfileInfoLoader();

            return View("Profile");
        }


        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction(nameof(Login));
        }
    }
}
