using desert_auth.Class;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace desert_auth.Controllers
{
    [Route("api/v0_9/AuthenticateByToken.xml")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        Service _s = new Service(true);
        Log _log = new Log();

        [HttpGet]
        public IActionResult AuthenticateByToken(string secret, string userIp, string token) 
        {
            var _easql = new EasQL();
            var IPBLOCKTBL = _easql.GetTable(_s.WorldConn, $"SELECT * FROM PaGamePrivate.TblBlockedIP WHERE _startIP LIKE '{userIp}' OR _endIP LIKE '{userIp}'");            
            string LOG = $"TOKEN:{token} IP:{userIp} SECRET:{secret}";            
            string RESPONSE = $"<AuthenticateByToken><api><code>100</code></api><result bdo_access=\"user\"><user key=\"{token}\"/></result></AuthenticateByToken>";
            int ERROR = 100;
            try
            {
                if (!token.Contains(","))
                {
                    _log.Create("INVALID TOKEN " + LOG);
                    return StatusCode(ERROR, RESPONSE);
                }                
                string UNAME = token.Split(",")[0];
                string PASSWORD = token.Split(",")[1];
                
                string query = "SELECT _userId FROM PaGamePrivate.TblUserInformation WHERE _userId LIKE '%"+UNAME+",%'";                
                var dbSearch = _easql.GetTable(_s.WorldConn, query);
                if (dbSearch.Rows.Count != 1 && _s.isEnableAutoRegister == false)
                {
                    _log.Create("USER NOT FOUND OR MULTI ENTRY " + LOG);
                    return StatusCode(ERROR, RESPONSE);
                }

                string DB_PASSWORD = dbSearch.Rows[0]["_userId"].ToString().Split(",")[1];
                if (!DB_PASSWORD.Equals(PASSWORD) && _s.isEnableAutoRegister == false)
                {
                    _log.Create("WRONG PASSWORD " + LOG );
                    return StatusCode(ERROR, RESPONSE);
                   
                }
                if (IPBLOCKTBL.Rows.Count != 0 && _s.isCheckIPBlock == true)
                {
                    _log.Create("IP BANNED " + LOG);
                    return StatusCode(ERROR, RESPONSE);
                }
                _log.Create("VALID TOKEN " + LOG );
                return StatusCode(200, RESPONSE);

            }
            catch (Exception e)
            {
                _log.Create("INVALID TOKEN " + LOG + " " + e);
                return StatusCode(ERROR, RESPONSE);
            }
            
        }
       
        

    }
}
