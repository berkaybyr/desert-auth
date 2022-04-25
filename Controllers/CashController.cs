using desert_auth.Class;
using EasMe;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Net;

namespace desert_auth.Controllers
{
    [Route("RESTAPI/[action]")]
    [ApiController]
    public class CashController : ControllerBase
    {
        EasQL _easql = new EasQL();
        Service _s = new Service(true);
        UserData _data = new UserData();
        [HttpGet]
        public IActionResult GetGMBalance(string FORMAT, string USERNO, string GAMECODE, long TRANTIME, string CHECKSUM)
        {//FORMAT=XML&SITECODE=1&USERNO=gunman&GAMECODE=bd&TRANTIME=20200413171325&CHECKSUM=b8cfff3fa3f27a7b7d10c40d21aef8dc9c97c364917c32c860782aef1c64aab8

            if (!_s.isEnableAcoin)
            {
                _s.Log($"[GETBALANCE] [ACOIN DISABLED] TOKEN:{USERNO} BALANCE: 0");                
                return Ok(GetResponse(0, "OK", USERNO, 0));
            }
            try
            {
                long CASHREAL = 0;
                string Username = USERNO.Split(",")[0];
                string Password = USERNO.Split(",")[1];
                var Usernumber = _data.GetUserNobyUsername(Username);
                var cmd = new SqlCommand("SELECT _balance FROM PaGamePrivate.TblUserInformation WHERE _userNo = @userno AND _realPassword = @password AND _userName = @username AND _isValid = 1");
                cmd.Parameters.AddWithValue("@userno", Usernumber);
                cmd.Parameters.AddWithValue("@password", Password);
                cmd.Parameters.AddWithValue("@username", Username);
                var Balance = _easql.ExecScalar(_s.WorldConn, cmd);
                if (Balance != null)
                {
                    CASHREAL = long.Parse(Balance.ToString());                  

                    _s.Log($"[GETBALANCE] TOKEN:{USERNO} BALANCE:{CASHREAL}");
                    return Ok(GetResponse(0, "OK", USERNO, CASHREAL));
                }
                else
                {
                    _s.Log($"[GETBALANCE] [FAILED] TOKEN:{USERNO} ");
                    return Ok(GetResponse(-1, "USER_NOT_EXIST", USERNO, 0));
                }
            }
            catch (Exception e)
            {
                _s.Log($"[GETBALANCE] [FAILED] TOKEN:{USERNO} ERROR: {e.Message}");
                return Ok(GetResponse(-1, "ERROR_OCCURED", USERNO, 0));
            }
            
            
            string GetResponse(int RETCODE, string ERRMSG, string USERNO, long CASHREAL)
            {
                return @$"
                <RESPONSE>
                <RETCODE>{RETCODE}</RETCODE> <!-- 0 for success -->
                <ERRMSG>{ERRMSG}</ERRMSG> <!-- OK for success -->
                <USERNO>{USERNO}</USERNO> <!-- account name -->
                <CASHREAL>{CASHREAL}</CASHREAL> <!-- CS balance -->
                <CASHBONUS>0</CASHBONUS> <!-- UNK -->
                <CASHTOTAL>0</CASHTOTAL> <!-- UNK -->
                <CHECKSUM>0</CHECKSUM> <!-- ignore -->
                </RESPONSE>";
            }
            

        }

        [HttpGet]
        public IActionResult PurchaseItem(string FORMAT, int SITECODE, string USERNO, string GAMECODE, int GAMEBILLINGID, long GAMEITEMID, string CHARACTERID, string GAMESERVERNAME, string GAMEWORLDNAME, string PRODUCTNAME, int PRODUCTCNT, long CHARGEAMT, string LANGUAGECODE, string IPADDR, long TRANTIME, string CHECKSUM)
        {//FORMAT=XML&SITECODE=1&USERNO=gunman&GAMECODE=bd&TRANTIME=20200413171325&CHECKSUM=b8cfff3fa3f27a7b7d10c40d21aef8dc9c97c364917c32c860782aef1c64aab8

            if (!_s.isEnableAcoin)
            {
                _s.Log($"[PURCHASE ITEM] [FAILED ACOIN DISABLED] TOKEN:{USERNO}");
                return Ok(GetResponse(-1, "ACOIN_DISABLED",USERNO,GAMEITEMID,CHARGEAMT,0));
            }
            try
            {
                long CASHREAL = 0;
                string Username = USERNO.Split(",")[0];
                var Usernumber = _data.GetUserNobyUsername(Username);
                var cmd = new SqlCommand("SELECT _balance FROM PaGamePrivate.TblUserInformation WHERE _userNo = @userno AND _isValid = 1");
                cmd.Parameters.AddWithValue("@userno", Usernumber);
                var Balance = _easql.ExecScalar(_s.WorldConn, cmd);


                if (Balance == null)
                {
                    _s.Log($"[PURCHASE ITEM] [FAILED USER NOT EXIST] TOKEN:{USERNO}");
                    return Ok(GetResponse(-1, "USER_NOT_EXIST", USERNO, GAMEITEMID, CHARGEAMT,0));
                }
                else
                {
                    CASHREAL = long.Parse(Balance.ToString());
                    if (CHARGEAMT < CASHREAL)
                    {
                        cmd.Parameters.Clear();
                        cmd = new SqlCommand("UPDATE PaGamePrivate.TblUserInformation SET _balance=@balance WHERE _userNo = @userno AND _isValid = 1");
                        cmd.Parameters.AddWithValue("@userno", Usernumber);
                        cmd.Parameters.AddWithValue("@balance", CASHREAL - CHARGEAMT);
                        _easql.ExecQuery(_s.WorldConn, cmd);
                        _s.Log($"[PURCHASE ITEM] [SUCCESSFUL] TOKEN:{USERNO} NEW BALANCE:{CASHREAL} CHARGE AMOUNT: {CHARGEAMT} ITEMID: {GAMEITEMID}");
                        return Ok(GetResponse(0, "OK", USERNO, GAMEITEMID, CHARGEAMT,CASHREAL - CHARGEAMT));
                    }
                    else
                    {
                        _s.Log($"[PURCHASE ITEM] [FAILED] TOKEN:{USERNO} BALANCE:{CASHREAL} CHARGE AMOUNT: {CHARGEAMT} ITEMID: {GAMEITEMID}");
                        return Ok(GetResponse(-1, "NOT_ENOUGH_CASH", USERNO, GAMEITEMID, CHARGEAMT, CASHREAL));
                    }
                    
                }
             
               
            }
            catch (Exception e)
            {
                _s.Log($"[PURCHASE ITEM] [FAILED EXCEPTION] TOKEN:{USERNO} ERROR: {e.Message}");
                return Ok(GetResponse(-1, "ERROR_OCCURED", USERNO, GAMEITEMID, CHARGEAMT, 0));
            }
            
            string GetResponse(int RETCODE,string ERRMSG, string USERNO,long CHARGENO,long CHARGEAMT,long CASHREAL)
            {
                return @$"
                <RESPONSE>
                <RETCODE>{RETCODE}</RETCODE> <!-- -1 for error -->
                <ERRMSG>{ERRMSG}</ERRMSG> <!-- NOT_ENOUGH_CASH for error -->
                <USERNO>{USERNO}</USERNO> <!-- account name -->
                <CHARGENO>{GAMEITEMID}</CHARGENO> <!-- GET[GAMEITEMID] maybe -->
                <CHARGEDAMT>{CHARGEAMT}</CHARGEDAMT> <!-- total for all items -->
                <CASHREAL>{CASHREAL}</CASHREAL> <!-- CS balance -->
                <CASHBONUS>0</CASHBONUS> <!-- UNK -->
                <CASHTOTAL>0</CASHTOTAL> <!-- UNK -->
                <CHECKSUM>0</CHECKSUM> <!-- ignore -->
                </RESPONSE>";
            }
        }

        [HttpPost]
        public IActionResult UpdateBalance(long USERNO, string USERNAME, string PASSWORD, long CASH, string REQUESTER, string TOKEN)
        {
            return StatusCode(100);
        }
        [HttpPost]
        public IActionResult TransferBalance(long fromUSERNO, string USERNAME, string PASSWORD, string toUSERNO, long CASH, string REQUESTER, string TOKEN)
        {
            return StatusCode(100);
        }
    }
}
