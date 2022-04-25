using desert_auth.Class;
using desert_auth.Models;
using EasMe;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

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
        public IActionResult Register(Register r)
        {
            if (!ModelState.IsValid)
            {
                _service.Log($"[REGISTER] [FAILED] UNAME:{r.Username} PASSWORD:{r.Password} EMAIL:{r.Email} IP:{r.IP} ERRMSG: INVALID MODEL");
                return BadRequest(new { message = "Invalid Model" });
            }
            string USERNAME = r.Username;
            string PASSWORD = r.Password;
            string CONFIRMPASSWORD = r.ConfirmPassword;
            string EMAIL = r.Email;
            string IP = r.IP;
            try
            {
                if (USERNAME == null || PASSWORD == null || CONFIRMPASSWORD == null || EMAIL == null)
                {
                    _service.Log($"[REGISTER] [FAILED] UNAME:{USERNAME} PASS:{PASSWORD} IP:{IP} EMAIL:{EMAIL} ERRMSG: MISSING PARAMETERS");
                    return BadRequest(new { message = "Fill all fields" });
                }
                if (PASSWORD != CONFIRMPASSWORD)
                {
                    _service.Log($"[REGISTER] [FAILED] UNAME:{USERNAME} PASS:{PASSWORD} IP:{IP} EMAIL:{EMAIL} ERRMSG: PASSWORDS NOT MATCH");
                    return BadRequest(new { message = "Passwords do not match" });
                }
                if (_service.IsValidEmail(EMAIL) == false)
                {
                    _service.Log($"[REGISTER] [FAILED] UNAME:{USERNAME} PASS:{PASSWORD} IP:{IP} EMAIL:{EMAIL} ERRMSG: INVALID EMAIL");
                    return BadRequest(new { message = "Invalid Email" });
                }

                var cmd = new SqlCommand("SELECT * FROM [PaGamePrivate].[TblUserInformation] WHERE _userName LIKE @USERNAME OR _email LIKE @EMAIL");
                cmd.Parameters.AddWithValue("@USERNAME", USERNAME);
                cmd.Parameters.AddWithValue("@EMAIL", EMAIL);
                var result = _sql.GetTable(_service.WorldConn, cmd);
                if (result.Rows.Count > 0)
                {
                    _service.Log($"[REGISTER USER] [FAILED] UNAME:{USERNAME} PASS:{PASSWORD} IP:{IP} EMAIL:{EMAIL} ERRMSG: USERNAME OR EMAIL ALREADY EXIST");
                    return BadRequest(new { message = "Username or Email already exist" });
                }

                if (!Regex.IsMatch(USERNAME, @"^[a-zA-Z0-9_]+$"))
                {
                    _service.Log($"[REGISTER USER] [FAILED] UNAME:{USERNAME} PASS:{PASSWORD} IP:{IP} EMAIL:{EMAIL} ERRMSG: INVALID USERNAME");
                    return BadRequest(new { message = "Invalid Username" });
                }

                if (!Regex.IsMatch(PASSWORD, @"^[a-zA-Z0-9]+$") && _service.isCheckStrongPassword)
                {
                    _service.Log($"[REGISTER USER] [FAILED] UNAME:{USERNAME} PASS:{PASSWORD} IP:{IP} EMAIL:{EMAIL} ERRMSG: INVALID PASSWORD");
                    return BadRequest(new { message = "Password is not strong enough" });
                }

                //EXEC STORED PROCEDURE TO ADD WORLD DB GAME TABLE                
                cmd = new SqlCommand("INSERT INTO PaGamePrivate.TblUserInformation (_registerDate,_isValid,_userName,_realPassword,_email,_userId,_password,_paymentPassword,_lastIp,_isAdultWorldUser,_balance) VALUES (@DATE,1,@USERNAME,@PASSWORD,@EMAIL,@USERID,'000001','000001',@IP,1,0)");
                cmd.Parameters.AddWithValue("@DATE", DateTime.Now);
                cmd.Parameters.AddWithValue("@USERNAME", USERNAME);
                cmd.Parameters.AddWithValue("@PASSWORD", PASSWORD);
                cmd.Parameters.AddWithValue("@EMAIL", EMAIL);
                cmd.Parameters.AddWithValue("@USERID", $"{USERNAME},{PASSWORD}");
                cmd.Parameters.AddWithValue("@IP", IP);
                _service.Log($"[REGISTER USER] [SUCCESSFUL] UNAME:{USERNAME} PASS:{PASSWORD} IP:{IP} EMAIL:{EMAIL}");
                _sql.ExecQuery(_service.WorldConn, cmd);
                return Ok(new { message = "Register Successful" });


            }
            catch (Exception ex)
            {
                _service.Log($"[REGISTER USER] [FAILED] UNAME:{USERNAME} PASS:{PASSWORD} IP:{IP} EMAIL:{EMAIL} ERRMSG: {ex.Message}");

                return BadRequest(new { message = "An internal error occured please contact support" });
            }



        }

        [HttpPost("Login")]
        public IActionResult Login(Login l)
        {
            if (!ModelState.IsValid)
            {
                _service.Log($"[LOGIN] [FAILED] UNAME:{l.Username} PASSWORD:{l.Password} ERRMSG: INVALID MODEL");
                return BadRequest(new { message = "Invalid Model" });
            }
            string USERNAME = l.Username;
            string PASSWORD = l.Password;
            string IP = l.IP;

            try
            {
                if (USERNAME == null || PASSWORD == null)
                {
                    return BadRequest(new { message = "Please fill all fields" });
                }
                if (USERNAME.Length < 6 || PASSWORD.Length < 6)
                {
                    return BadRequest(new { message = "Username and Password must be at least 6 characters" });
                }
                var cmd = new SqlCommand("SELECT * FROM [PaGamePrivate].[TblUserInformation] WHERE _userName LIKE @USERNAME AND _realPassword LIKE @PASSWORD and _isValid = 1");
                cmd.Parameters.AddWithValue("@USERNAME", USERNAME);
                cmd.Parameters.AddWithValue("@PASSWORD", PASSWORD);
                var result = _sql.GetTable(_service.WorldConn, cmd);


                if (result.Rows.Count != 1)
                {
                    _service.Log($"[LOGIN] [FAILED] UNAME:{USERNAME} PASS:{PASSWORD} IP:{IP} ERRMSG: USERNAME OR PASSWORD INCORRECT OR USER NOT VALID");
                    return BadRequest(new { message = "Username, password are wrong or user not exist." });
                }
                var _id = long.Parse(result.Rows[0]["_userNo"].ToString());
                var _user = new User
                {
                    ID = _id,
                    RegisterDate = DateTime.Parse(result.Rows[0]["_registerDate"].ToString()),
                    FamilyName = result.Rows[0]["_userNickname"].ToString(),
                    UserName = result.Rows[0]["_userName"].ToString(),
                    Password = result.Rows[0]["_realPassword"].ToString(),
                    Email = result.Rows[0]["_email"].ToString(),
                    Balance = long.Parse(result.Rows[0]["_balance"].ToString()),
                    LastIP = result.Rows[0]["_lastIp"].ToString(),
                    PremiumEnd = _data.GetPremiumEndTimebyID(_id),
                    TotalPlaytime = long.Parse(result.Rows[0]["_totalPlayTime"].ToString()),
                };


                _service.Log($"[LOGIN] [SUCCESSFUL] UNAME:{USERNAME} PASS:{PASSWORD} IP:{IP}");
                return Ok(new { message = "Login Successful", user = _user });

            }
            catch (Exception ex)
            {
                _service.Log($"[LOGIN] [FAILED] UNAME:{USERNAME} PASS:{PASSWORD} IP:{IP} ERRMSG: {ex.Message}");
                return BadRequest(new { message = "An internal error occured please contact support" });
            }
        }
        [HttpPost("ResetPassword")]
        public IActionResult ResetPassword(ResetPassword rp)
        {
            if (!ModelState.IsValid)
            {
                _service.Log($"[LOGIN] [FAILED] XXX ERRMSG: INVALID MODEL");
                return BadRequest(new { message = "Invalid Model" });
            }
            //if (_service.API_KEY != API_KEY)
            //{
            //    _service.Log($"[RESET PASSWORD] [FAILED] UNAME:{username} EMAIL:{email} IP:{ip} INVALID API KEY: {API_KEY}");
            //    return BadRequest(new {  message = "Invalid API Key" });
            //}
            return StatusCode(100);
        }

        [HttpPost("ChangePassword")]
        public IActionResult ChangePassword(string email, string username, string password, string newpassword, string confirmpassword, string ip, string API_KEY)
        {
            //if (_service.API_KEY != API_KEY)
            //{
            //    _service.Log($"[CHANGE PASSWORD] [FAILED] EMAIL:{email} UNAME:{username} PASSWORD:{password} NEWPASS:{newpassword} IP:{ip} INVALID API KEY: {API_KEY}");
            //    return Ok(new { status = "error", message = "Invalid API Key" });
            //}
            return StatusCode(100);
        }

    }
}
