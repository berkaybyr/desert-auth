using EasMe;
using System.Text.RegularExpressions;

namespace desert_auth.Class
{
    public class Service
    {
        
        
        //public readonly string GameConn = "Data Source=localhost;Initial Catalog=EVOBDO_GAMEDB_0001;Integrated Security=True";
        //public readonly string WorldConn = "Data Source=localhost;Initial Catalog=EVOBDO_WORLDDB_0001;Integrated Security=True";
        //public readonly string LogConn = "Data Source=localhost;Initial Catalog=EVOBDO_LOGDB_0001;Integrated Security=True";
        public List<string>? IPWhiteList;

        public string GameConn;
        public string WorldConn;
        public string LogConn;
        public string BillingConn;

        public bool isEnableAutoRegister; //enables auto register
        public bool isCheckIPBlock; //no possible to work, requests come from local from Main Auth Server request string has local IP in it
        public bool isCheckMultipleIP; //no implemented, pull ips from login log or user table and compare them with AC port connections if exists, this can be an int value X number of connections allowed from one IP
        public bool isMaintenanceMode; //enables maintenance mode, only admins in TblRoleGroupMember can login
        public bool isEnableAcoin; //enables acoin needto create table before activating

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

            //SETTINGS
            isEnableAutoRegister = Convert.ToBoolean(int.Parse(_ini.Read("SETTINGS", "isEnableAutoRegister")));
            isCheckIPBlock = Convert.ToBoolean(int.Parse(_ini.Read("SETTINGS", "isCheckIPBlock")));
            isCheckMultipleIP = Convert.ToBoolean(int.Parse(_ini.Read("SETTINGS", "isCheckMultipleIP")));
            isMaintenanceMode = Convert.ToBoolean(int.Parse(_ini.Read("SETTINGS", "isMaintenanceMode")));
            isEnableAcoin = Convert.ToBoolean(int.Parse(_ini.Read("SETTINGS", "isEnableAcoin")));
            if (isEnableAutoRegister == null || isCheckIPBlock == null || isCheckMultipleIP == null || isMaintenanceMode == null || isEnableAcoin == null) return false;

            //CONNECTION
            GameConn = _ini.Read("CONNECTION", "GameConn");
            WorldConn = _ini.Read("CONNECTION", "WorldConn");
            LogConn = _ini.Read("CONNECTION", "LogConn");
            BillingConn = _ini.Read("CONNECTION", "BillingConn");
            if (GameConn == null || WorldConn == null || LogConn == null || BillingConn == null) return false;

            return true;
        }

        EasLog _log = new EasLog();
        public void Log(string message)
        {
            _log.Create(message);
            Console.WriteLine($"[{DateTime.Now}] {message}");
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
        

        public bool IsValidPhone(string number)
        {
            string motif = @"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$";
            if (number != null) return Regex.IsMatch(number, motif);
            else return false;
        }        
    }
}
