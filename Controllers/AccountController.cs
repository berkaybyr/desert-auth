using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Web;
namespace desert_auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        [HttpPost("Register")]
        public IActionResult Register(string username, string password, string email)
        {
            string ip = HttpContext.GetServerVariable("HTTP_X_FORWARDED_FOR");
            if (string.IsNullOrEmpty(ip))
            {
                ip = HttpContext.GetServerVariable("REMOTE_ADDR");
            }
            
            DateTime registerDate = DateTime.Now;

            return StatusCode(100,ip);
           
        }
        [HttpPost("Login")]
        public IActionResult Login(string username, string password)
        {
            if (username == "admin" && password == "admin")
            {
                return Ok(new { token = "admin" });
            }
            return Unauthorized();
        }
      
    }
}
