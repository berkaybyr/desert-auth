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

        UserData _data = new UserData();
        [HttpGet]
        public IActionResult GetGMBalance(string FORMAT, string USERNO, string GAMECODE, long TRANTIME, string CHECKSUM)
        {//FORMAT=XML&SITECODE=1&USERNO=gunman&GAMECODE=bd&TRANTIME=20200413171325&CHECKSUM=b8cfff3fa3f27a7b7d10c40d21aef8dc9c97c364917c32c860782aef1c64aab8

            if (!Service.IsEnableAcoin)
            {
                EasLog.Info($"[GETBALANCE] [ACOIN DISABLED] TOKEN:{USERNO} BALANCE:0");                
                return Ok(GetResponse(0, "OK", USERNO, 0));
            }
            try
            {
                long CASHREAL = 0;
                string Username = USERNO.Split(",")[0];
                string Password = USERNO.Split(",")[1];
                var Usernumber = _data.GetUserNobyUsername(Username);
                var Balance = _data.GetBalancebyUsername(Username, Password);

                if (Balance != -1)
                {
                    CASHREAL = long.Parse(Balance.ToString());
                    EasLog.Info($"[GETBALANCE] [SUCCESS] TOKEN:{USERNO} BALANCE:{CASHREAL}");
                    return Ok(GetResponse(0, "OK", USERNO, CASHREAL));
                }
                else
                {
                    EasLog.Error($"[GETBALANCE] TOKEN:{USERNO} ERRMSG:USER NOT FOUND");
                    return Ok(GetResponse(-1, "USER_NOT_EXIST", USERNO, 0));
                }
            }
            catch (Exception e)
            {
                EasLog.Error($"[GETBALANCE] TOKEN:{USERNO} ERRMSG:{e.Message}");
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

            if (!Service.IsEnableAcoin)
            {
                EasLog.Info($"[PURCHASE ITEM] [ACOIN DISABLED] TOKEN:{USERNO}");
                return Ok(GetResponse(-1, "ACOIN_DISABLED",USERNO,GAMEITEMID,CHARGEAMT,0));
            }
            try
            {
                long CASHREAL = 0;
                string Username = USERNO.Split(",")[0];
                var Usernumber = _data.GetUserNobyUsername(Username);
                var cmd = new SqlCommand("SELECT _balance FROM PaGamePrivate.TblUserInformation WHERE _userNo = @userno AND _isValid = 1");
                cmd.Parameters.AddWithValue("@userno", Usernumber);
                var Balance = EasQL.ExecScalar(Service.WorldConn, cmd);


                if (Balance == null)
                {
                    EasLog.Error($"[PURCHASE ITEM] TOKEN:{USERNO} ERRMSG:USER NOT EXIST");
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
                        EasQL.ExecNonQuery(Service.WorldConn, cmd);
                        EasLog.Info($"[PURCHASE ITEM] [SUCCESS] TOKEN:{USERNO} NEW BALANCE:{CASHREAL} CHARGE AMOUNT: {CHARGEAMT} ITEMID: {GAMEITEMID}");
                        return Ok(GetResponse(0, "OK", USERNO, GAMEITEMID, CHARGEAMT,CASHREAL - CHARGEAMT));
                    }
                    else
                    {
                        EasLog.Error($"[PURCHASE ITEM] TOKEN:{USERNO} BALANCE:{CASHREAL} CHARGE AMOUNT: {CHARGEAMT} ITEMID: {GAMEITEMID} ERRMSG:USER DONT HAVE ENOUGH CASH");
                        return Ok(GetResponse(-1, "NOT_ENOUGH_CASH", USERNO, GAMEITEMID, CHARGEAMT, CASHREAL));
                    }
                    
                }
             
               
            }
            catch (Exception ex)
            {
                EasLog.Error($"[PURCHASE ITEM] TOKEN:{USERNO} ",ex);
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

        //KEY IS CUSTOM PASSWORD FOR THIS ACTION IT CAN BE SET IN SERVICE.INI FILE
        //CASH CAN BE MINUS TO DECREASE CASH FROM USER, MOSTLY NOT NEEDED        
        [HttpPost]
        public IActionResult UpdateBalance(string FAMILYNAME, long CASH, string REQUESTER, string KEY)
        {
            if (!Service.IsEnableAcoin)
            {
                EasLog.Error($"[UPDATE BALANCE] [ACOIN DISABLED] FAMILYNAME:{FAMILYNAME} BALANCE:0");
                return BadRequest(new { status = "ERROR", message = "Acoin is disabled"});
            }
            if (KEY != Service.UpdateBalanceKey)
            {
                EasLog.Error($"[UPDATE BALANCE] FAMILYNAME:{FAMILYNAME} CASH:{CASH} REQUESTER:{REQUESTER} ERRMSG:INVALID KEY");
                return BadRequest(new { status = "ERROR", message = "Invalid key" });
            }
            if(_data.GetUserNobyFamName(FAMILYNAME) == -1)
            {
                EasLog.Error($"[UPDATE BALANCE] FAMILYNAME:{FAMILYNAME} CASH:{CASH} REQUESTER:{REQUESTER} ERRMSG:FAMILY NOT EXIST");
                return BadRequest(new { status = "ERROR", message = "Family name not exist" });
            }
            
            var result = _data.UpdateBalance(FAMILYNAME, CASH);
            if (result == -1)
            {
                EasLog.Error($"[UPDATE BALANCE] FAMILYNAME:{FAMILYNAME} CASH:{CASH} REQUESTER:{REQUESTER} ERRMSG:UPDATE FAILED");
                return BadRequest(new { status = "ERROR", message = "An internal error occured, please contact support" });
            }
            else if (result == -2)
            {
                EasLog.Error($"[UPDATE BALANCE] FAMILYNAME:{FAMILYNAME} CASH:{CASH} REQUESTER:{REQUESTER} ERRMSG:USER DONT HAVE ENOUGH CASH ");
                return BadRequest(new { status = "ERROR", message = "User don't have enough cash" });
            }
            else
            {
                EasLog.Info($"[UPDATE BALANCE] [SUCCESS] FAMILYNAME:{FAMILYNAME} CASH:{CASH} REQUESTER:{REQUESTER}");
                return Ok(new { status = "SUCCESS", message = "Update balance is successful", newbalance = result });
            }
        }
        [HttpPost]
        public IActionResult TransferBalance(string fromFamilyName, string fromPassword, string toFamilyname, long CASH, string REQUESTER, string KEY)
        {
            if (!Service.IsEnableAcoin)
            {
                EasLog.Error($"[TRANSFER BALANCE] [ACOIN DISABLED] FAMILYNAME:{fromFamilyName} TOFAMILYNAME:{toFamilyname} CASH:{CASH} REQUESTER:{REQUESTER}");
                return BadRequest(new { status ="ERROR",message = "ACOIN_DISABLED" });
            }
            if (KEY != Service.UpdateBalanceKey) //USING SAME PASS KEY FOR UPDATE AND TRASNFER ACTIONS
            {
                EasLog.Error($"[TRANSFER BALANCE] FAMILYNAME:{fromFamilyName} TOFAMILYNAME:{toFamilyname} CASH:{CASH} REQUESTER:{REQUESTER} ERRMSG:INVALID KEY");
                return BadRequest(new { status = "ERROR", message = "Invalid key" });
            }
            if (_data.GetUserNobyFamName(fromFamilyName) == -1)
            {
                EasLog.Error($"[TRANSFER BALANCE] FAMILYNAME:{fromFamilyName} TOFAMILYNAME:{toFamilyname} CASH:{CASH} REQUESTER:{REQUESTER} ERRMSG:FROM FAMILY NOT EXIST");
                return BadRequest(new { status = "ERROR", message = "From family name not exist" });
            }
            if (_data.GetUserNobyFamName(toFamilyname) == -1)
            {
                EasLog.Error($"[TRANSFER BALANCE] FAMILYNAME:{fromFamilyName} TOFAMILYNAME:{toFamilyname} CASH:{CASH} REQUESTER:{REQUESTER} ERRMSG:TO FAMILY NOT EXIST");
                return BadRequest(new { status = "ERROR", message = "To family name not exist" });
            }
            if (_data.GetUserNobyFamName(fromFamilyName) == _data.GetUserNobyFamName(toFamilyname))
            {
                EasLog.Error($"[TRANSFER BALANCE] FAMILYNAME:{fromFamilyName} TOFAMILYNAME:{toFamilyname} CASH:{CASH} REQUESTER:{REQUESTER} ERRMSG:CANNOT TRANSFER TO SELF");
                return BadRequest(new { status = "ERROR", message = "Can not transfer to self" });
            }
            if (CASH > 99999)
            {
                EasLog.Error($"[TRANSFER BALANCE] FAMILYNAME:{fromFamilyName} TOFAMILYNAME:{toFamilyname} CASH:{CASH} REQUESTER:{REQUESTER} ERRMSG:INVALID CASH AMOUNT");
                return BadRequest(new { status = "ERROR", message = "Invalid cash amount" });
            }
            var result = _data.TransferBalance(fromFamilyName,fromPassword,toFamilyname, CASH);
            if (result == -1)
            {
                EasLog.Error($"[TRANSFER BALANCE] FAMILYNAME:{fromFamilyName} TOFAMILYNAME:{toFamilyname} CASH:{CASH} REQUESTER:{REQUESTER} ERRMSG:UPDATE FAILED");
                return BadRequest(new { status = "ERROR", message = "An internal error occured, please contact support" });
            }
            else if (result == -2)
            {
                EasLog.Error($"[TRANSFER BALANCE] FAMILYNAME:{fromFamilyName} TOFAMILYNAME:{toFamilyname} CASH:{CASH} REQUESTER:{REQUESTER} ERRMSG:USER DONT HAVE ENOUGH CASH ");
                return BadRequest(new { status = "ERROR", message = "User don't have enough cash" });
            }
            else
            {
                EasLog.Info($"[TRANSFER BALANCE] [SUCCESS] FAMILYNAME:{fromFamilyName} TOFAMILYNAME:{toFamilyname} CASH:{CASH} REQUESTER:{REQUESTER}");
                return Ok(new { status = "SUCCESS", message = "Transfer balance is successful"});
            }

        }
    }
}
