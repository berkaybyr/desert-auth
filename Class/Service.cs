using EasMe;
using System.Text.RegularExpressions;

namespace desert_auth.Class
{
    public static class Service
    {

        public static List<string> IPWhitelist { get; set; } = new List<string> { "127.0.0.1" };
        public static bool IsEnableAutoRegister { get; set; } = true;
        public static bool IsMaintenanceMode { get; set; } = false;
        public static bool IsEnableAcoin { get; set; } = false;
        public static bool IsCheckIpBlock { get; set; } = false;
        public static string AuthSecret { get; set; } = "A9939BB6E458440CB1575044B78B0712";
        public static string UpdateBalanceKey { get; set; } = "000000000000000000000000000000000000000000";

        public static readonly string GameConn = "Data Source=localhost;Initial Catalog=SA_BETA_GAMEDB_0002;Persist Security Info=True;";
        public static readonly string WorldConn = "Data Source=localhost;Initial Catalog=SA_BETA_WORLDDB_0002;Persist Security Info=True;";
        public static readonly string LogConn = "Data Source=localhost;Initial Catalog=PF_BETA_LOGDB_0001;Persist Security Info=True;";
        public static readonly string BillingConn = "Data Source=localhost;Initial Catalog=CH_JOY_BILLINGDB_0001;Persist Security Info=True;";
        
        public static void Load()
        {
            EasLog.LoadConfiguration(
                new EasLogConfiguration
                {
                    EnableDebugMode = true,
                });

            EasINI.LoadFile(Directory.GetCurrentDirectory() + @"\service.ini");
   
            //IPWHITELIST
            string? ipWhitelist = EasINI.Read("SETTINGS", "IPWhitelist");
            if (!string.IsNullOrEmpty(ipWhitelist))
            {
                IPWhitelist = ipWhitelist.Split(',').ToList();
            }
            else EasLog.Error("Service error, IPWhiteList is NULL");
            
            if (IPWhitelist.Count == 0) EasLog.Error("Service error, IPWhiteList is empty");
            
            foreach (string ip in IPWhitelist)
            {
                if (!ip.IsValidIPAddress())
                {
                    EasLog.Error($"Service error, Given ip in IPWhitelist is not valid => IP:  {ip} ");
                    
                }
            }
            
            //LOAD service.ini
            var __IsEnableAutoRegister = EasINI.Read("SETTINGS", "IsEnableAutoRegister");
            if (!string.IsNullOrEmpty(__IsEnableAutoRegister ))
            {
                IsEnableAutoRegister = __IsEnableAutoRegister.ToBoolean();
            }
            else EasLog.Error("Service error, IsEnableAutoRegister is NULL");
            
            var __IsMaintenanceMode = EasINI.Read("SETTINGS", "IsMaintenanceMode");
            if (!string.IsNullOrEmpty(__IsMaintenanceMode))
            {
                IsMaintenanceMode = __IsMaintenanceMode.ToBoolean();
            }
            else EasLog.Error("Service error, IsMaintenanceMode is NULL");

            var __IsCheckIpBlock = EasINI.Read("SETTINGS", "IsCheckIpBlock");
            if (!string.IsNullOrEmpty(__IsCheckIpBlock))
            {
                IsCheckIpBlock = __IsCheckIpBlock.ToBoolean();
            }
            else EasLog.Error("Service error, IsCheckIpBlock is NULL");
                        
            var __IsEnableAcoin = EasINI.Read("SETTINGS", "IsEnableAcoin");
            if (!string.IsNullOrEmpty(__IsEnableAcoin))
            {
                IsEnableAcoin = __IsEnableAcoin.ToBoolean();
            }
            else EasLog.Info("Service error, IsEnableAcoin is NULL");
            var __UpdateBalanceKey = EasINI.Read("SETTINGS", "UpdateBalanceKey");
            if (!string.IsNullOrEmpty(__UpdateBalanceKey))
            {
                UpdateBalanceKey = __UpdateBalanceKey;
            }
            else EasLog.Info("Service error, UpdateBalanceKey is NULL");


            if (IsEnableAutoRegister && IsMaintenanceMode)
            {
                EasLog.Error("[SERVICE] [ERROR] IsEnableAutoRegister and IsMaintenanceMode can't be true at the same time, please check your service.ini file");
                Environment.Exit(0);
            }
            //EasLog.Info($"isEnableAutoRegister: {IsEnableAutoRegister} isMaintenanceMode: {IsMaintenanceMode} isCheckIpBlock: {IsCheckIpBlock} isEnableAcoin: {IsEnableAcoin} updateBalanceKey: {UpdateBalanceKey}");
        }


        
         
        
        
    }
}
