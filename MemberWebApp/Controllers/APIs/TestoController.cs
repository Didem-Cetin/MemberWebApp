using MemberWebApp.Helpers;
using MemberWebApp.Models;
using MemberWebApp.Models.ApiModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MemberWebApp.Controllers.APIs
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestoController : ControllerBase
    {
        private readonly ITokenHelper _tokenHelper;

        public TestoController(ITokenHelper tokenHelper)
        {
            _tokenHelper = tokenHelper;
        }

        [HttpGet()]
        public IActionResult GetData()
        {
            return Ok(new { Name = "Didem", Surname = "Çetin", Age = 26 });
        }

        [HttpPost]
        public IActionResult PostData([FromBody] PostDataApiModel model)
        {
            return Ok(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody]LoginViewModel model)
        {
            if (model.Username=="aaa"&& model.Password=="123123")
            {
               string token= _tokenHelper.GenerateToken(model.Username, 0);

                return Ok(new {Error=false, Message="Token Created.", Data=token});
            }
            else
            {
                return BadRequest(new {Errror=true,Message="Incorrect identity."});
            }
        }
    }
}
