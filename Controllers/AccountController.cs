using Microsoft.AspNetCore.Mvc;
using EasMe;
using desert_auth.Class;
using System.Data.SqlClient;

namespace desert_auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        EasQL _sql = new EasQL();
        Service _service = new Service(true);
        [HttpPost("Register")]
        public IActionResult Register(string username, string password, string confirmpassword, string email, string ip)
        {
            DateTime registerDate = DateTime.Now;
            string query = "PaGamePublic.uspAuthenticateOrCreateUser";
            var cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@userId", username);
            cmd.Parameters.AddWithValue("@ip", ip);
            _sql.ExecStoredProcedure(_service.WorldConn,cmd);
            

            return StatusCode(100);

        }
        [HttpPost("Login")]
        public IActionResult Login(string username, string password)
        {
            return StatusCode(100);
        }
        [HttpPost("ResetPassword")]
        public IActionResult ResetPassword(string username, string email, string phone, string ip)
        {
            return StatusCode(100);
        }
    
        [HttpPost("ChangePassword")]
        public IActionResult ChangePassword(string email,string username, string password, string newpassword, string confirmpassword, string ip)
        {
            return StatusCode(100);
        }

    }
}
