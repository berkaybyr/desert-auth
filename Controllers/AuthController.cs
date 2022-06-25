using desert_auth.Class;
using EasMe;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace desert_auth.Controllers
{
    [Route("api/v0_9/AuthenticateByToken.xml")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        [HttpGet]
        public IActionResult AuthenticateByToken(string secret, string userIp, string token)
        {
            string LOG = $"TOKEN:{token} IP:{userIp}";
            if (secret != "A9939BB6E458440CB1575044B78B0712")
            {
                //AUTH API only receives request from local auth server so this check is not needed
            }
            string RESPONSE = $"<AuthenticateByToken><api><code>100</code></api><result bdo_access=\"user\"><user key=\"{token}\"/></result></AuthenticateByToken>";
            int ERROR = 100;
            try
            {
                if (!token.Contains(','))
                {
                    EasLog.Error($"[AUTHENTICATE USER] {LOG} ERRMSG:TOKEN IS INVALID");
                    return StatusCode(ERROR, RESPONSE);
                }
                string UNAME = token.Split(",")[0];
                string PASSWORD = token.Split(",")[1];

                if (Service.IsCheckIpBlock)
                {

                    var cmd = new SqlCommand($"SELECT * FROM PaGamePrivate.TblBlockedIP WHERE ServicetartIP LIKE @ip OR _endIP LIKE @ip");
                    cmd.Parameters.AddWithValue("@ip", $"%{userIp}%");
                    var IPBLOCKTBL = EasQL.GetTable(Service.WorldConn, cmd);
                    if (IPBLOCKTBL.Rows.Count != 0)
                    {
                        EasLog.Error($"[AUTHENTICATE USER] {LOG} ERRMSG:IP BANNED]");
                        return StatusCode(ERROR, RESPONSE);
                    }
                }

                //var cmd = new SqlCommand("SELECT * FROM PaGamePrivate.TblUserInformation WHERE _userId LIKE @userid");
                var cmd2 = new SqlCommand("SELECT * FROM PaGamePrivate.TblUserInformation WHERE _username = @username AND _isValid = 1");
                cmd2.Parameters.AddWithValue("@username", $"{UNAME}");
                var dbSearch = EasQL.GetTable(Service.WorldConn, cmd2);
                switch (dbSearch.Rows.Count)
                {
                    case 0:
                        if (!Service.IsEnableAutoRegister)
                        {
                            EasLog.Error($"[AUTHENTICATE USER] {LOG} ERRMSG:USER NOT FOUND]");
                            return StatusCode(ERROR, RESPONSE);
                        }
                        else
                        {
                            EasLog.Error($"[AUTHENTICATE USER] [NEW REGISTER] {LOG}");
                            cmd2.Parameters.Clear();
                            cmd2 = new SqlCommand("INSERT INTO PaGamePrivate.TblUserInformation (_userId,_password,_paymentpassword,_username, _realpassword,_balance) VALUES (@uid,'000001','000001',@username, @password,0)");
                            cmd2.Parameters.AddWithValue("@uid", $"{UNAME},{PASSWORD}");
                            cmd2.Parameters.AddWithValue("@username", $"{UNAME}");
                            cmd2.Parameters.AddWithValue("@password", $"{PASSWORD}");
                            EasQL.ExecNonQuery(Service.WorldConn, cmd2);
                            return StatusCode(200, RESPONSE);
                        }
                    case 1:
                        string? DB_PASSWORD = dbSearch.Rows[0]["_realPassword"].ToString();
                        if (!DB_PASSWORD.Equals(PASSWORD))
                        {
                            EasLog.Error($"[AUTHENTICATE USER] {LOG} ERRMSG:WRONG PASSWORD");
                            return StatusCode(ERROR, RESPONSE);
                        }
                        if (dbSearch.Rows[0]["_isValid"].ToString() == "0")
                        {
                            EasLog.Error($"[AUTHENTICATE USER] {LOG} ERRMSG:USER NOT VALID]");
                            return StatusCode(ERROR, RESPONSE);
                        }
                        if (Service.IsMaintenanceMode)
                        {
                            long? USERNO = long.Parse(dbSearch.Rows[0]["_userNo"].ToString());
                            cmd2.Parameters.Clear();
                            cmd2 = new SqlCommand($"SELECT _userNo FROM PaGamePrivate.TblRoleGroupMember WHERE _userNo = @userno");
                            cmd2.Parameters.AddWithValue("@userno", USERNO);
                            var ADMINNO = EasQL.GetTable(Service.WorldConn, cmd2);
                            if (ADMINNO.Rows.Count == 0)
                            {
                                EasLog.Error($"[AUTHENTICATE USER] {LOG} ERRMSG:MAINTENANCE MODE ");
                                return StatusCode(ERROR, RESPONSE);
                            }

                        }
                        break;
                    default:
                        EasLog.Error($"[AUTHENTICATE USER] {LOG} ERRMSG:MULTIPLE ENTRY FOUND");
                        return StatusCode(ERROR, RESPONSE);

                }

                EasLog.Info($"[AUTHENTICATE USER] [SUCCESS] " + LOG);
                return StatusCode(200, RESPONSE);

            }
            catch (Exception e)
            {
                EasLog.Error($"[AUTHENTICATE USER] {LOG}", e);
                return StatusCode(ERROR, RESPONSE);
            }

        }
    }
}
