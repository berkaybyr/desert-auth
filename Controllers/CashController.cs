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
                return StatusCode(200, @$"
                <RESPONSE>
                <RETCODE>0</RETCODE> <!-- 0 for success -->
                <ERRMSG>OK</ERRMSG> <!-- OK for success -->
                <USERNO>{USERNO}</USERNO> <!-- account name -->
                <CASHREAL>0</CASHREAL> <!-- CS balance -->
                <CASHBONUS>0</CASHBONUS> <!-- UNK -->
                <CASHTOTAL>0</CASHTOTAL> <!-- UNK -->
                <CHECKSUM>0</CHECKSUM> <!-- ignore -->
                </RESPONSE>");
            }
            long CASHREAL = 0;
            string Username = USERNO.Split(",")[0];
            //string Password = USERNO.Split(",")[1];
            var Usernumber = _data.GetUserNobyUsername(Username);
            var cmd = new SqlCommand("SELECT balance FROM dbo.TblUser WHERE userNo = @userno");
            cmd.Parameters.AddWithValue("@userno", Usernumber);
            var Balance = _easql.ExecScalar(_s.WorldConn, cmd);
            if (Balance != null)
            {
                CASHREAL = long.Parse(Balance.ToString());
            }
            
            string Response = @$"
                <RESPONSE>
                <RETCODE>0</RETCODE> <!-- 0 for success -->
                <ERRMSG>OK</ERRMSG> <!-- OK for success -->
                <USERNO>{USERNO}</USERNO> <!-- account name -->
                <CASHREAL>{CASHREAL}</CASHREAL> <!-- CS balance -->
                <CASHBONUS>0</CASHBONUS> <!-- UNK -->
                <CASHTOTAL>0</CASHTOTAL> <!-- UNK -->
                <CHECKSUM>0</CHECKSUM> <!-- ignore -->
                </RESPONSE>";

            _s.Log($"[GETBALANCE] TOKEN:{USERNO} BALANCE:{CASHREAL}");
            return StatusCode((int)HttpStatusCode.OK, Response);
        }

        [HttpGet]
        public IActionResult PurchaseItem(string FORMAT, int SITECODE, string USERNO, string GAMECODE, int GAMEBILLINGID, long GAMEITEMID, string CHARACTERID, string GAMESERVERNAME, string GAMEWORLDNAME, string PRODUCTNAME, int PRODUCTCNT, long CHARGEAMT, string LANGUAGECODE, string IPADDR, long TRANTIME, string CHECKSUM)
        {//FORMAT=XML&SITECODE=1&USERNO=gunman&GAMECODE=bd&TRANTIME=20200413171325&CHECKSUM=b8cfff3fa3f27a7b7d10c40d21aef8dc9c97c364917c32c860782aef1c64aab8

            if (!_s.isEnableAcoin)
            {
                _s.Log($"[PURCHASE ITEM] [FAILED] TOKEN:{USERNO} ACOIN DISABLED");
                return StatusCode(100);
            }
            long CASHREAL = 0;
            string Username = USERNO.Split(",")[0];
            var Usernumber = _data.GetUserNobyUsername(Username);
            var cmd = new SqlCommand("SELECT balance FROM dbo.TblUser WHERE userNo = @userno");
            cmd.Parameters.AddWithValue("@userno", Usernumber);
            var Balance = _easql.ExecScalar(_s.WorldConn, cmd);
            if (Balance != null)
            {
                CASHREAL = long.Parse(Balance.ToString());
                cmd.Parameters.Clear();
                cmd = new SqlCommand("UPDATE dbo.TblUser (balance,variousDate) VALUES (@balance,@date) WHERE userNo = @userno");
                cmd.Parameters.AddWithValue("@userno", Usernumber);
                cmd.Parameters.AddWithValue("@balance", CASHREAL - CHARGEAMT);
                _easql.ExecQuery(_s.WorldConn, cmd);
            }
            string response = @$"
                <RESPONSE>
                <RETCODE>0</RETCODE> <!-- 0 for success -->
                <ERRMSG>OK</ERRMSG> <!-- OK for success -->
                <USERNO>{USERNO}</USERNO> <!-- account name -->
                <CHARGENO>{GAMEITEMID}</CHARGENO> <!-- GET[GAMEITEMID] maybe -->
                <CHARGEDAMT>{CHARGEAMT}</CHARGEDAMT> <!-- total for all items -->
                <CASHREAL>{CASHREAL - CHARGEAMT}</CASHREAL> <!-- CS balance -->
                <CASHBONUS>0</CASHBONUS> <!-- UNK -->
                <CASHTOTAL>0</CASHTOTAL> <!-- UNK -->
                <CHECKSUM>0</CHECKSUM> <!-- ignore -->
                </RESPONSE>";


            
            if (CHARGEAMT > CASHREAL)
            {
                _s.Log($"[PURCHASE ITEM] [FAILED] TOKEN:{USERNO} BALANCE:{CASHREAL} CHARGE AMOUNT: {CHARGEAMT} ITEMID: {GAMEITEMID}");
                return StatusCode(100, @$"
                <RESPONSE>
                <RETCODE>-1</RETCODE> <!-- -1 for error -->
                <ERRMSG>NOT_ENOUGH_CASH</ERRMSG> <!-- NOT_ENOUGH_CASH for error -->
                <USERNO>{USERNO}</USERNO> <!-- account name -->
                <CHARGENO>{GAMEITEMID}</CHARGENO> <!-- GET[GAMEITEMID] maybe -->
                <CHARGEDAMT>{CHARGEAMT}</CHARGEDAMT> <!-- total for all items -->
                <CASHREAL>{CASHREAL}</CASHREAL> <!-- CS balance -->
                <CASHBONUS>0</CASHBONUS> <!-- UNK -->
                <CASHTOTAL>0</CASHTOTAL> <!-- UNK -->
                <CHECKSUM>0</CHECKSUM> <!-- ignore -->
                </RESPONSE>");
            }

            _s.Log($"[PURCHASE ITEM] [SUCCESSFUL] TOKEN:{USERNO} NEW BALANCE:{CASHREAL} CHARGE AMOUNT: {CHARGEAMT} ITEMID: {GAMEITEMID}");
            //GetGMBalance(FORMAT, USERNO, GAMECODE, TRANTIME, CHECKSUM);
            return StatusCode((int)HttpStatusCode.OK, response);
        }

        [HttpPost]
        public IActionResult UpdateBalance(long USERNO, string USERNAME, string PASSWORD, long CASH, string REQUESTER, string TOKEN)
        {
            return StatusCode(100);
        }
        [HttpPost]
        public IActionResult TransferBalance(long fromUSERNO, string USERNAME, string PASSWORD,string toUSERNO, long CASH, string REQUESTER, string TOKEN)
        {
            return StatusCode(100);
        }
    }
}
