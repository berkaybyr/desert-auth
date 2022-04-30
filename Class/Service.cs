using EasMe;
using System.Text.RegularExpressions;

namespace desert_auth.Class
{
    public class Service
    {
        
        public List<string> IPWhiteList;

        public string AuthSecret;
        public string GameConn;
        public string WorldConn;
        public string LogConn;
        public string BillingConn;
        public string UpdateBalanceKey; //KEY FOR UPDATE BALANCE
        
        public bool isEnableAutoRegister;
        public bool isCheckIPBlock; 
        public bool isCheckMultipleIP; //no implemented
        public bool isMaintenanceMode; //ONLY ADMINS IN TBLGROUPMEMBER CAN LOGIN
        public bool isEnableAcoin; //NEED TO ADD _balance COLUMN IN TBLUSERINFORMATION        
        public bool isLogSecret; //NEED TO ADD _balance COLUMN IN TBLUSERINFORMATION        

        public Service(bool reload)
        {
            if (reload)
            {
                if (!Load()) Environment.Exit(0);                
            }
        }
        bool Load()
        {
            
            string iniPath = Directory.GetCurrentDirectory() + @"\service.ini";
            if (!File.Exists(iniPath)) return false;
            var _ini = new EasINI(iniPath);           
            //IPWHITELIST
            string? temp = _ini.Read("SETTINGS", "IPWhiteList");
            if (temp == null) return false;
            IPWhiteList = temp.Split(',').ToList();
            if (IPWhiteList.Count == 0) return false;
            foreach (string ip in IPWhiteList)
            {
                if (!Regex.IsMatch(ip, @"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$"))
                {
                    Log($"[SERVICE] [ERROR] IPWHITELIST: {ip} is not valid");
                    return false;
                }
            }
     
            //SETTINGS
            isEnableAutoRegister = Convert.ToBoolean(int.Parse(_ini.Read("SETTINGS", "isEnableAutoRegister")));
            isCheckIPBlock = Convert.ToBoolean(int.Parse(_ini.Read("SETTINGS", "isCheckIPBlock")));
            isCheckMultipleIP = Convert.ToBoolean(int.Parse(_ini.Read("SETTINGS", "isCheckMultipleIP")));
            isMaintenanceMode = Convert.ToBoolean(int.Parse(_ini.Read("SETTINGS", "isMaintenanceMode")));
            isEnableAcoin = Convert.ToBoolean(int.Parse(_ini.Read("SETTINGS", "isEnableAcoin")));
            isLogSecret = Convert.ToBoolean(int.Parse(_ini.Read("SETTINGS", "isLogSecret")));
            UpdateBalanceKey = _ini.Read("SETTINGS", "UpdateBalanceKey");
            AuthSecret = _ini.Read("SETTINGS", "AuthSecret");
            if (isEnableAutoRegister == null)
            {
                Log($"[SERVICE] [ERROR] isEnableAutoRegister is not valid");
                return false;
            }
            if (isCheckIPBlock == null)
            {
                Log($"[SERVICE] [ERROR] isCheckIPBlock is not valid");
                return false;
            }
            if (isCheckMultipleIP == null)
            {
                Log($"[SERVICE] [ERROR] isCheckMultipleIP is not valid");
                return false;
            }
            if (isMaintenanceMode == null)
            {
                Log($"[SERVICE] [ERROR] isMaintenanceMode is not valid");
                return false;
            }
            if (isEnableAcoin == null)
            {
                Log($"[SERVICE] [ERROR] isEnableAcoin is not valid");
                return false;
            }
            if (isLogSecret == null)
            {
                Log($"[SERVICE] [ERROR] isLogSecret is not valid");
                return false;
            }
            if (UpdateBalanceKey == null)
            {
                Log($"[SERVICE] [ERROR] UpdateBalanceKey is not valid");
                return false;
            }
            if (AuthSecret == null)
            {
                Log($"[SERVICE] [ERROR] AuthSecret is not valid");
                return false;
            }
            //CONNECTION
            GameConn = _ini.Read("CONNECTION", "GameConn");
            WorldConn = _ini.Read("CONNECTION", "WorldConn");
            LogConn = _ini.Read("CONNECTION", "LogConn");
            BillingConn = _ini.Read("CONNECTION", "BillingConn");
            
            if (GameConn == null)
            {
                Log($"[SERVICE] [ERROR] GameConn is not valid");
                return false;
            }
            if (WorldConn == null)
            {
                Log($"[SERVICE] [ERROR] WorldConn is not valid");
                return false;
            }
            if (LogConn == null)
            {
                Log($"[SERVICE] [ERROR] LogConn is not valid");
                return false;
            }
            if (BillingConn == null)
            {
                Log($"[SERVICE] [ERROR] BillingConn is not valid");
                return false;
            }
            if (isEnableAutoRegister == true && isMaintenanceMode == true)
            {
                Log("[SERVICE] [ERROR] isEnableAutoRegister and isMaintenanceMode can't be true at the same time, please check your service.ini file");
                Log("[SERVICE] [ERROR] isEnableAutoRegister set to false");
                isEnableAutoRegister = false;
            }
            
            return true;
        }

        EasLog _log = new EasLog();
        public void Log(string message){

            _log.Create(message, true);            
        }
        public bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
         
        
        
    }
}
