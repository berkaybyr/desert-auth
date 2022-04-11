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
        Service _s = new Service(true);
        EasLog _log = new EasLog();
        EasQL _easql = new EasQL();

        [HttpGet]
        public IActionResult AuthenticateByToken(string secret, string userIp, string token)
        {
            string LOG = $"TOKEN:{token} IP:{userIp} SECRET:{secret}";
            string RESPONSE = $"<AuthenticateByToken><api><code>100</code></api><result bdo_access=\"user\"><user key=\"{token}\"/></result></AuthenticateByToken>";
            int ERROR = 100;
            try
            {
                string UNAME = token.Split(",")[0];
                string PASSWORD = token.Split(",")[1];
                var cmd = new SqlCommand("SELECT * FROM PaGamePrivate.TblUserInformation WHERE _userId LIKE @userid");
                cmd.Parameters.AddWithValue("@userid", $"{UNAME},%");
                var dbSearch = _easql.GetTable(_s.WorldConn, cmd);
                if (dbSearch.Rows.Count == 0)
                {
                    if (!_s.isEnableAutoRegister)
                    {
                        _s.Log("[USER NOT FOUND] " + LOG);
                        return StatusCode(ERROR, RESPONSE);
                    }
                }
                else if (dbSearch.Rows.Count == 1)
                {
                    string? DB_PASSWORD = dbSearch.Rows[0]["_userId"].ToString().Split(",")[1];
                    if (!DB_PASSWORD.Equals(PASSWORD))
                    {
                        _s.Log("[WRONG PASSWORD] " + LOG);
                        return StatusCode(ERROR, RESPONSE);
                    }
                    
                    long? USERNO = long.Parse(dbSearch.Rows[0]["_userNo"].ToString());
                    cmd.Parameters.Clear();
                    cmd = new SqlCommand($"SELECT _userNo FROM PaGamePrivate.TblRoleGroupMember WHERE _userNo = @userno");
                    cmd.Parameters.AddWithValue("@userno", USERNO);
                    var ADMINNO = _easql.GetTable(_s.WorldConn, cmd);
                    if (ADMINNO.Rows.Count == 0 && _s.isMaintenanceMode)
                    {
                        _s.Log("[MAINTENANCE MODE] " + LOG);
                        return StatusCode(ERROR, RESPONSE);
                    }
                }
                else {
                    _s.Log("[MULTIPLE ENTRY FOUND] " + LOG);
                    return StatusCode(ERROR, RESPONSE);
                }

                _s.Log("[VALID TOKEN] " + LOG);
                return StatusCode(200, RESPONSE);

            }
            catch (Exception e)
            {
                _s.Log("[INVALID TOKEN] " + LOG + " " + e);
                return StatusCode(ERROR, RESPONSE);
            }

        }



    }
}
//not working ip is local ip main auth server gives the request not possible to take ip from the request url, check game logs or tbluserinformation for ip
//var IPBLOCKTBL = _easql.GetTable(_s.WorldConn, $"SELECT * FROM PaGamePrivate.TblBlockedIP WHERE _startIP LIKE '{userIp}' OR _endIP LIKE '{userIp}'");
//if (IPBLOCKTBL.Rows.Count != 0 && _s.isCheckIPBlock)
//{
//    _log.Create("[IP BANNED] " + LOG);
//    return StatusCode(ERROR, RESPONSE);
//}