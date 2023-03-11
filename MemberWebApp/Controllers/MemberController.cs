using AutoMapper;
using MemberWebApp.Entities;
using MemberWebApp.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NETCore.Encrypt.Extensions;


namespace MemberWebApp.Controllers
{
    [Authorize(Roles = "admin", AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class MemberController : Controller
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IHasher _hasher;

        public MemberController(DatabaseContext databaseContext, IMapper mapper, IConfiguration configuration, IHasher hasher)
        {
            _databaseContext = databaseContext;
            _mapper = mapper;
            _configuration = configuration;
            _hasher = hasher;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult MemberListPartial()
        {
            List<MemberUserModel> users = _databaseContext.Users.ToList().Select(x => _mapper.Map<MemberUserModel>(x)).ToList();

            return PartialView("_MemberListPartial", users);
        }


        public IActionResult AddNewUserPartial()
        {


            return PartialView("_AddNewUserPartial", new MemberCreateUserModel());
        }

        [HttpPost]
        public IActionResult AddNewUser(MemberCreateUserModel model)
        {
            if (ModelState.IsValid)
            {
                if (_databaseContext.Users.Any(x => x.Username.ToLower() == model.Username.ToLower()))
                {
                    ModelState.AddModelError(nameof(Index), "Username is already exist");
                    return PartialView("_AddNewUserPartial", model);
                }

                User user = _mapper.Map<User>(model);
                user.Password = _hasher.DoMD5HashedString(model.Password);

                _databaseContext.Users.Add(user);
                _databaseContext.SaveChanges();
                return PartialView("_AddNewUserPartial", new MemberCreateUserModel { Done = "User added." });
            }


            return PartialView("_AddNewUserPartial", model);
        }

        public IActionResult EditUserPartial(Guid id)

        {
            User user = _databaseContext.Users.Find(id);
            MemberEditUserModel model = _mapper.Map<MemberEditUserModel>(user);

            return PartialView("_EditUserPartial", model);
        }


        [HttpPost]
        public IActionResult EditUser(Guid id, MemberEditUserModel model)
        {
            if (ModelState.IsValid)
            {
                if (_databaseContext.Users.Any(x => x.Username.ToLower() == model.Username.ToLower() && x.Id != id))
                {
                    ModelState.AddModelError(nameof(Index), "Username is already exist.");
                    return PartialView("_EditUserPartial", model);
                }

                User user = _databaseContext.Users.Find(id);

                _mapper.Map(model, user);
                _databaseContext.SaveChanges();

                return PartialView("_EditUserPartial", new MemberEditUserModel { Done = "User updated." });
            }

            return PartialView("_EditUserPartial", model);
        }

        public IActionResult DeleteUser(Guid id)

        {
            User user = _databaseContext.Users.Find(id);
            if (user != null)
            {
                _databaseContext.Users.Remove(user);
                _databaseContext.SaveChanges();
            }

            return MemberListPartial();
        }

    }
}
