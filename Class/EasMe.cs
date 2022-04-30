using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace EasMe
{
    /*
    REFERANCES MUST BE TO CLASS NAMES NOT EasMe FILE NAME
    Either like this;
    var _easql = new EasMe.EasQL();
    or
    using EasMe;
    var _easql = new EasQL();
     */

    
    public class EasQL
    {

        public DataTable GetTable(string connection, SqlCommand cmd, int timeout = 0)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connection))
            {
                try
                {                    
                    conn.Open();
                    cmd.Connection = conn;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);                    
                    da.SelectCommand.CommandTimeout = timeout; //Default timeout is 0, if you set timeout when pulling massive data it will give you an error
                    da.Fill(dt);
                    conn.Close();
                    conn.Dispose();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

            }
            return dt;
        }

        public int ExecQuery(string connection, SqlCommand cmd, int timeout = 0)
        {
            int rowsEffected = 0;
            using (SqlConnection conn = new SqlConnection(connection)) 
            {
                try
                {
                    cmd.Connection = conn;
                    cmd.CommandTimeout = timeout; //Default timeout is 0, if you set timeout when executing massive data it will give you an error
                    conn.Open();
                    rowsEffected = cmd.ExecuteNonQuery();
                    conn.Close();
                    conn.Dispose();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

            }
            return rowsEffected;
        }

        public object ExecScalar(string connection, SqlCommand cmd, int timeout = 0) 
        {
            var obj = new object();
            using (SqlConnection conn = new SqlConnection(connection))
            {
                try
                {
                    cmd.Connection = conn;
                    cmd.CommandTimeout = timeout;
                    conn.Open();
                    obj = cmd.ExecuteScalar();
                    conn.Close();
                    conn.Dispose();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

            }
            return obj;
        }

        public int ExecStoredProcedure(string connection, SqlCommand cmd)
        {
            cmd.CommandType = CommandType.StoredProcedure;
            return ExecQuery(connection, cmd);
        }

        public void BackupDatabase(string connection, string dbname, string path) //Path value must not contain new backup file name it is generated in the function
        {
            string query = $@"BACKUP DATABASE {dbname} TO DISK = '{path}\{dbname + DateTime.Now.ToString(" H-mm-ss dd-MM-yyyy")}.bak'";
            var cmd = new SqlCommand(query);
            ExecQuery(connection, cmd);
        }
        public void ShrinkDatabaseLog(string connection, string dbname, string dblogname)
        {
            string query = $@"ALTER DATABASE [{dbname}] SET RECOVERY SIMPLE WITH NO_WAIT
                            DBCC SHRINKFILE(N'{dblogname}', 1)
                            ALTER DATABASE [{dbname}] SET RECOVERY FULL WITH NO_WAIT";
            var cmd = new SqlCommand(query);
            ExecQuery(connection, cmd);
        }
        public void ShrinkDatabase(string connection, string dbname)
        {
            string query = $@"ALTER DATABASE [{dbname}] SET RECOVERY SIMPLE WITH NO_WAIT
                            DBCC SHRINKFILE(N'{dbname}', 1)
                            ALTER DATABASE [{dbname}] SET RECOVERY FULL WITH NO_WAIT";
            var cmd = new SqlCommand(query);
            ExecQuery(connection, cmd);
        }


        public int TruncateTable(string connection, string table)
        {
            string query = $@"TRUNCATE TABLE {table}";
            var cmd = new SqlCommand(query);
            ExecQuery(connection, cmd);
            return 0;
        }

        public int DropTable(string connection, string table)
        {
            string query = $@"DROP TABLE {table}";
            var cmd = new SqlCommand(query);
            ExecQuery(connection, cmd);
            return 0;
        }
        public List<string> GetAllTableName(string connection) //SCHEMANAME.TABLENAME
        {
            string query = $@"SELECT '['+SCHEMA_NAME(schema_id)+'].['+name+']' FROM sys.tables";
            var list = new List<string>();
            SqlCommand cmd = new SqlCommand(query);
            var dt = GetTable(connection, cmd);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(row[0].ToString());
            }
            return list;
        }
        public int DeleteDatabase(string connection, string dbname)
        {
            string query = $@"DROP DATABASE {dbname}";
            var cmd = new SqlCommand(query);
            ExecQuery(connection, cmd);
            return 0;
        }
       

    }

    public class EasINI
    {
        
        public string Path;

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool WritePrivateProfileString(string lpAppName, string lpKeyName, string lpString, string lpFileName);
        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        /*
        When calling the class need to give .ini file path
        var _ini = new EasINI("C:\Users\admin\Desktop\service.ini")
         */
        public EasINI(string INIFilePath)
        {
            Path = INIFilePath;
        }
        /*
         WRITE USAGE        
         EasINI ini = new EasINI(INI);
         WriteINI("SETTINGS", "URL", "www.google.com", ".\service.ini");
         */
        public void Write(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, this.Path);
        }

        /* 
         READ USAGE
         EasINI ini = new EasINI(INI);
        
         var a = ini.ReadINI("SETTINGS", "URL");
         ini.ReadINI("SETTINGS", "URL");          
        */
        public string Read(string Section, string Key)
        {
            StringBuilder buffer = new StringBuilder(255);
            GetPrivateProfileString(Section, Key, "", buffer, 255, this.Path);
            return Convert.ToString(buffer);
        }


    }

    public class EasLog
    {
        public static string DirCurrent = Directory.GetCurrentDirectory();//gets current directory 
        public static string DirLog = DirCurrent + "\\Logs\\"; //log file directory

        /*
        Interval value        
        0 => Daily (Default)
        1 => Hourly 
        2 => Every Minute
        */
        public void Create(string log, bool doConsoleLog = false,int interval = 0)
        {
            string IntervalFormat = "";
            string LogContent = $"[{DateTime.Now}] {log}\n" ;
            
            //Creates log file in current directory
            if (!Directory.Exists(DirLog)) Directory.CreateDirectory(DirLog);

            switch (interval)
            {
                case 0:
                    IntervalFormat = "MM.dd.yyyy";
                    break;
                case 1:
                    IntervalFormat = "MM.dd.yyyy HH";
                    break;
                case 2:
                    IntervalFormat = "MM.dd.yyyy HH.mm";
                    break;
            }
            
            string LogPath = DirLog + DateTime.Now.ToString(IntervalFormat) + " -log.txt";
            if (doConsoleLog) Console.Write(LogContent);
            File.AppendAllText(LogPath, LogContent);           

        }
    }

    public class EasDel
    {
        

        EasLog _log = new EasLog();
        public static string DirCurrent = Directory.GetCurrentDirectory();//gets current directory 
        public static string DirUser = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);//gets user directory C:\Users\%username%
        public static string DirDesktop = DirUser + "\\Desktop";//gets desktop directory C:\Users\%username%\desktop
        public static string DirAppData = DirUser + "\\AppData\\Roaming";//gets appdata directory C:\Users\%username%\AppData\\Roaming
        public static string DirSystem = Environment.GetFolderPath(Environment.SpecialFolder.System);//gets system32 directory C:\Windows\System32
        public static string DirLog = DirCurrent + "\\Logs\\"; //log file directory

        public static bool isEnableLogging = true;

        //When calling the class give bool value to determine to enable or disable logging         
        public EasDel(bool _isEnableLogging)
        {
            isEnableLogging = _isEnableLogging;
        }

        public void DeleteAllFiles(string path)
        {
            if (Directory.Exists(path))
            {
                string[] files = Directory.GetFiles(path);
                string[] subdirs = Directory.GetDirectories(path);
                foreach (string file in files)
                {
                    try
                    {
                        File.Delete(file);
                        if (isEnableLogging) _log.Create("FILE DELETED: " + file);
                    }
                    catch
                    {
                        if (isEnableLogging) _log.Create("FILE FAILED: " + file);
                    }
                    
                }
                foreach (string subdir in subdirs)
                {
                    DeleteAllFiles(subdir);
                }
                try
                {
                    Directory.Delete(path);
                    if (isEnableLogging) _log.Create("FOLDER DELETED: " + path);
                }
                catch
                {
                    if (isEnableLogging) _log.Create("FOLDER FAILED: " + path);
                }
            }
            else
            {
                try
                {
                    File.Delete(path);
                    if (isEnableLogging) _log.Create("FILE DELETED: " + path);
                }
                catch
                {
                    if (isEnableLogging) _log.Create("FILE FAILED: " + path);
                }
                
            }

        }
        
        


    }
}
