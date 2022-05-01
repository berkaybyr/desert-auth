﻿using desert_auth.Class;
using EasMe;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

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
            string LOG = $"TOKEN:{token} IP:{userIp}";            
            if (_s.isLogSecret)
            {
                LOG = $"TOKEN:{token} IP:{userIp} SECRET:{secret}";
            }
            string RESPONSE = $"<AuthenticateByToken><api><code>100</code></api><result bdo_access=\"user\"><user key=\"{token}\"/></result></AuthenticateByToken>";
            int ERROR = 100;
            try
            {
                if (!token.Contains(','))
                {
                    _s.Log($"[AUTHENTICATE USER] [FAILED] {LOG} ERRMSG:TOKEN IS INVALID");
                    return StatusCode(ERROR, RESPONSE);
                }
                string UNAME = token.Split(",")[0];
                string PASSWORD = token.Split(",")[1];
                //if (!Regex.IsMatch(UNAME, @"^[a-zA-Z0-9_]*$") || token.Contains(' '))
                //{
                    
                //    _s.Log($"[AUTHENTICATE USER] [FAILED] {LOG} ERRMSG:USERNAME CAN ONLY CONTAIN A-z,0-9,_");
                //    return StatusCode(ERROR, RESPONSE);
                //}
                
                if (HasSpecialChars(UNAME,false))
                {
                    _s.Log($"[AUTHENTICATE USER] [FAILED] {LOG} ERRMSG:USERNAME CAN ONLY CONTAIN A-z,0-9,_");
                    return StatusCode(ERROR, RESPONSE);
                }
                if (HasSpecialChars(PASSWORD, false))
                {
                    _s.Log($"[AUTHENTICATE USER] [FAILED] {LOG} ERRMSG:PASSWORD CAN ONLY CONTAIN A-z,0-9,_");
                    return StatusCode(ERROR, RESPONSE);
                }
                if (_s.isCheckIPBlock)
                {
                    
                    var cmd = new SqlCommand($"SELECT * FROM PaGamePrivate.TblBlockedIP WHERE _startIP LIKE @ip OR _endIP LIKE @ip");
                    cmd.Parameters.AddWithValue("@ip", $"%{userIp}%");
                    var IPBLOCKTBL = _easql.GetTable(_s.WorldConn, cmd);
                    if (IPBLOCKTBL.Rows.Count != 0)
                    {
                        _s.Log($"[AUTHENTICATE USER] [FAILED] {LOG} ERRMSG:IP BANNED]");
                        return StatusCode(ERROR, RESPONSE);
                    }
                }
                if (_s.isCheckMultipleIP)
                {
                    if (CheckIfMultipleLogin(""))
                    {
                        _s.Log($"[AUTHENTICATE USER] [FAILED] {LOG} ERRMSG:MULTIPLE LOGIN]");
                        return StatusCode(ERROR, RESPONSE);
                    }
                }
                //var cmd = new SqlCommand("SELECT * FROM PaGamePrivate.TblUserInformation WHERE _userId LIKE @userid");
                var cmd2 = new SqlCommand("SELECT * FROM PaGamePrivate.TblUserInformation WHERE _username = @username AND _isValid = 1");
                cmd2.Parameters.AddWithValue("@username", $"{UNAME}");
                var dbSearch = _easql.GetTable(_s.WorldConn, cmd2);
                switch (dbSearch.Rows.Count)
                {
                    case 0:
                        if (!_s.isEnableAutoRegister)
                        {
                            _s.Log($"[AUTHENTICATE USER] [FAILED] {LOG} ERRMSG:USER NOT FOUND]");
                            return StatusCode(ERROR, RESPONSE);
                        }
                        else
                        {
                            _s.Log($"[AUTHENTICATE USER] [NEW REGISTER] {LOG}");
                            cmd2.Parameters.Clear();
                            cmd2 = new SqlCommand("INSERT INTO PaGamePrivate.TblUserInformation (_userId,_password,_paymentpassword,_username, _realpassword,_balance) VALUES (@uid,'000001','000001',@username, @password,0)");                            
                            cmd2.Parameters.AddWithValue("@uid", $"{UNAME},{PASSWORD}");
                            cmd2.Parameters.AddWithValue("@username", $"{UNAME}");
                            cmd2.Parameters.AddWithValue("@password", $"{PASSWORD}");
                            _easql.ExecQuery(_s.WorldConn, cmd2);
                            return StatusCode(200, RESPONSE);
                        }
                    case 1:
                        string? DB_PASSWORD = dbSearch.Rows[0]["_realPassword"].ToString();
                        if (!DB_PASSWORD.Equals(PASSWORD))
                        {
                            _s.Log($"[AUTHENTICATE USER] [FAILED] {LOG} ERRMSG:WRONG PASSWORD");
                            return StatusCode(ERROR, RESPONSE);
                        }
                        if (dbSearch.Rows[0]["_isValid"].ToString() == "0")
                        {
                            _s.Log($"[AUTHENTICATE USER] [FAILED] {LOG} ERRMSG:USER NOT VALID]");
                            return StatusCode(ERROR, RESPONSE);
                        }
                        if (_s.isMaintenanceMode)
                        {
                            long? USERNO = long.Parse(dbSearch.Rows[0]["_userNo"].ToString());
                            cmd2.Parameters.Clear();
                            cmd2 = new SqlCommand($"SELECT _userNo FROM PaGamePrivate.TblRoleGroupMember WHERE _userNo = @userno");
                            cmd2.Parameters.AddWithValue("@userno", USERNO);
                            var ADMINNO = _easql.GetTable(_s.WorldConn, cmd2);
                            if (ADMINNO.Rows.Count == 0)
                            {
                                _s.Log($"[AUTHENTICATE USER] [FAILED] {LOG} ERRMSG:MAINTENANCE MODE ");
                                return StatusCode(ERROR, RESPONSE);
                            }

                        }
                        break;
                    default:
                        _s.Log($"[AUTHENTICATE USER] [FAILED] {LOG} ERRMSG:MULTIPLE ENTRY FOUND");
                        return StatusCode(ERROR, RESPONSE);

                }

                _s.Log($"[AUTHENTICATE USER] [SUCCESS] " + LOG);
                return StatusCode(200, RESPONSE);

            }
            catch (Exception e)
            {
                _s.Log($"[AUTHENTICATE USER] [FAILED] {LOG} ERRMSG:{e}");
                return StatusCode(ERROR, RESPONSE);
            }
            bool CheckIfMultipleLogin(string IP) {
                //NOT WORKING WITH ANTICHEAT ONLY WAY IS TO CHECK IF USER IS LOGGED IN PORT AND WHEN IP FROM TO PORT CONNECTION COUNT IS MORE THAN 1 CLOSE CONNECTION EITHER CHECK IT EVERY TIME A USER LOGGES IN OR CHECK IT ONCE AND THEN CHECK EVERY TIME
                return false;
            }
            bool HasSpecialChars(string yourString,bool isPassword)
            {
                string AllowedinPassword = "!%#@_[](){}/*";
                string AllowedinUsername = "_";
                foreach (char c in yourString)
                {
                    if (char.IsLetterOrDigit(c))
                    {
                        continue;
                    } 
                    else if (isPassword)
                    {
                        foreach(char ch in AllowedinPassword)
                        {
                            if (c != ch)
                            {
                                return true;
                            }                            
                        }                        
                    }
                    else
                    {
                        foreach (char ch in AllowedinUsername)
                        {
                            if (c != ch)
                            {
                                return true;
                            }
                        }
                    }
                    
                }
                return false;
                //if (isPassword)
                //{
                //    return yourString.Any(c => !char.IsLetterOrDigit(c) && c != '_');
                //}
                //else
                //{
                //    return yourString.Any(c => !char.IsLetter(c) && c != '_');
                //}
                //return yourString.Any(ch => !Char.IsLetterOrDigit(ch) && ch != '_');
            }            
        }     
    }
}
