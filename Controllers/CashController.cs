using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace desert_auth.Controllers
{
    [Route("RESTAPI/[action]")]
    [ApiController]
    public class CashController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetGMBalance(string FORMAT, string USERNO, string GAMECODE, long TRANTIME, string CHECKSUM)
        {//FORMAT=XML&SITECODE=1&USERNO=gunman&GAMECODE=bd&TRANTIME=20200413171325&CHECKSUM=b8cfff3fa3f27a7b7d10c40d21aef8dc9c97c364917c32c860782aef1c64aab8
            long CASHREAL = 0;
            //CHECK USER CASH AND DEFINE CASHREAL
            string response = @$"
                <RESPONSE>
                <RETCODE>0</RETCODE> <!-- 0 for success -->
                <ERRMSG>OK</ERRMSG> <!-- OK for success -->
                <USERNO>{USERNO}</USERNO> <!-- account name -->
                <CASHREAL>{CASHREAL}</CASHREAL> <!-- CS balance -->
                <CASHBONUS>0</CASHBONUS> <!-- UNK -->
                <CASHTOTAL>0</CASHTOTAL> <!-- UNK -->
                <CHECKSUM>0</CHECKSUM> <!-- ignore -->
                </RESPONSE>";


            return StatusCode((int)HttpStatusCode.OK, response);
        }

        [HttpGet]
        public IActionResult PurchaseItem(string FORMAT, int SITECODE, string USERNO, string GAMECODE, int GAMEBILLINGID, long GAMEITEMID, string CHARACTERID, string GAMESERVERNAME, string GAMEWORLDNAME, string PRODUCTNAME, int PRODUCTCNT, long CHARGEAMT, string LANGUAGECODE, string IPADDR, long TRANTIME, string CHECKSUM)
        {//FORMAT=XML&SITECODE=1&USERNO=gunman&GAMECODE=bd&TRANTIME=20200413171325&CHECKSUM=b8cfff3fa3f27a7b7d10c40d21aef8dc9c97c364917c32c860782aef1c64aab8
            long CASHREAL = 0;

            string response = @$"
                <RESPONSE>
                <RETCODE>0</RETCODE> <!-- 0 for success -->
                <ERRMSG>OK</ERRMSG> <!-- OK for success -->
                <USERNO>{USERNO}</USERNO> <!-- account name -->
                <CHARGENO>{GAMEITEMID}</CHARGENO> <!-- GET[GAMEITEMID] maybe -->
                <CHARGEDAMT>{CHARGEAMT}</CHARGEDAMT> <!-- total for all items -->
                <CASHREAL>{CASHREAL}</CASHREAL> <!-- CS balance -->
                <CASHBONUS>0</CASHBONUS> <!-- UNK -->
                <CASHTOTAL>0</CASHTOTAL> <!-- UNK -->
                <CHECKSUM>0</CHECKSUM> <!-- ignore -->
                </RESPONSE>";


            return StatusCode((int)HttpStatusCode.OK, response);
        }

        [HttpPost]
        public IActionResult UpdateBalance(long UserId, string Username, string Password, long Cash, string Requester, string Token)
        {
            return StatusCode((int)HttpStatusCode.OK, "OK");
        }
    
    }
}
