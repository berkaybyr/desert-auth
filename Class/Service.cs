using System.IO;
using System.Runtime.InteropServices;
using System.Text;

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
        
        public bool isEnableAutoRegister;
        public bool isCheckIPBlock;
        public bool isCheckMultipleIP;

        public Service(bool reload)
        {
            if (reload) Load();
        }
        bool Load()
        {
            //IPWHITELIST
            string? temp = IniReadValue("SETTINGS", "IPWhiteList");
            if (temp == null) return false;
            IPWhiteList = temp.Split(',').ToList();
            if (IPWhiteList.Count == 0) return false;

            //SETTINGS
            isEnableAutoRegister = Convert.ToBoolean(int.Parse(IniReadValue("SETTINGS", "isEnableAutoRegister")));
            isCheckIPBlock = Convert.ToBoolean(int.Parse(IniReadValue("SETTINGS", "isCheckIPBlock")));
            isCheckMultipleIP = Convert.ToBoolean(int.Parse(IniReadValue("SETTINGS", "isCheckMultipleIP")));
            if (isEnableAutoRegister == null || isCheckIPBlock == null || isCheckMultipleIP == null) return false;
            
            //CONNECTION
            GameConn = IniReadValue("CONNECTION", "GameConn");
            WorldConn = IniReadValue("CONNECTION", "WorldConn");
            LogConn = IniReadValue("CONNECTION", "LogConn");
            if (GameConn == null || WorldConn == null || LogConn == null) return false;

            return true;
        }

        
        #region READ INI FILE
        public string Path = Directory.GetCurrentDirectory() + @"\service.ini";

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool WritePrivateProfileString(string lpAppName, string lpKeyName, string lpString, string lpFileName);
        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public string? IniReadValue(string Section, string Key)
        {
            StringBuilder buffer = new StringBuilder(255);
            GetPrivateProfileString(Section, Key, "", buffer, 255, this.Path);
            return Convert.ToString(buffer);
        }
        #endregion
    }
}
