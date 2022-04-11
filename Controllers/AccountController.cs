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
        UserData _data = new UserData();
        [HttpPost("Register")]
        public IActionResult Register(string USERNAME, string PASSWORD, string CONFIRMPASSWORD, string PHONE,string EMAIL, string IP)
        {
            DateTime registerDate = DateTime.Now;
            try
            {
                if (USERNAME == null || PASSWORD == null || CONFIRMPASSWORD == null || PHONE == null || EMAIL == null)
                {
                    return Ok(new { status = "error", message = "Please fill all fields" });
                }
                if (PASSWORD != CONFIRMPASSWORD)
                {
                    return Ok(new { status = "error", message = "Password and Confirm Password does not match" });
                }
                if (USERNAME.Length < 6 || PASSWORD.Length < 6 || CONFIRMPASSWORD.Length < 6 || PHONE.Length < 6 || EMAIL.Length < 6)
                {
                    return Ok(new { status = "error", message = "Username, Password, Confirm Password, Phone and Email must be at least 6 characters" });
                }
                if (_service.IsValidEmail(EMAIL) == false)
                {
                    return Ok(new { status = "error", message = "Email is not valid" });
                }
                if (!_service.IsValidPhone(PHONE))
                {
                    return Ok(new { status = "error", message = "Phone is not valid" });
                }
                var cmd = new SqlCommand("SELECT * FROM [dbo].[TblUser] WHERE USERNAME LIKE @USERNAME OR EMAIL LIKE @EMAIL OR PHONE LIKE @PHONE");
                cmd.Parameters.AddWithValue("@USERNAME", USERNAME);
                cmd.Parameters.AddWithValue("@EMAIL", EMAIL);
                cmd.Parameters.AddWithValue("@PHONE", PHONE);
                var result = _sql.GetTable(_service.WorldConn,cmd);
                if (result.Rows.Count > 0)
                {
                    return Ok(new { status = "error", message = "Username, Email or Phone is already taken" });
                }

                
                int rowAffected = 0;
                //EXEC STORED PROCEDURE TO ADD WORLD DB GAME TABLE                
                cmd = new SqlCommand("PaGamePublic.uspAuthenticateOrCreateUser");
                cmd.Parameters.AddWithValue("@userId", USERNAME);
                cmd.Parameters.AddWithValue("@ip", IP);
                rowAffected = _sql.ExecStoredProcedure(_service.WorldConn, cmd);
                if(rowAffected == 1)
                {
                    //EXEC QUERY TO ADD USER TO USER TABLE                    
                    cmd = new SqlCommand("UPDATE dbo.TblUser (registerDate,username,password,email,phone,ip,balance) VALUES (@date, @username, @password, @email, @phone, @ip, @balance)");
                    cmd.Parameters.AddWithValue("@date", registerDate);
                    cmd.Parameters.AddWithValue("@username", USERNAME);
                    cmd.Parameters.AddWithValue("@password", PASSWORD);
                    cmd.Parameters.AddWithValue("@email", EMAIL);
                    cmd.Parameters.AddWithValue("@phone", PHONE);
                    cmd.Parameters.AddWithValue("@ip", IP);
                    cmd.Parameters.AddWithValue("@balance", 0);
                    rowAffected += _sql.ExecQuery(_service.WorldConn, cmd);
                    if (rowAffected == 2)
                    {
                        var UserNo = _data.GetUserNobyUsername(USERNAME);
                        return Ok("User Created, UserNo: " + UserNo);
                    }
                }
               
                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
            

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
