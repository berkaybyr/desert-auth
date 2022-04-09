using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desert_auth.Class
{
    class EasQL
    {
        
        public DataTable GetTable(string connection, string query, int timeout = 0)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connection))
            {
                try
                {
                    
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    da.SelectCommand.CommandTimeout = timeout; //Set time out 0 for big data pulls can be added as option as well
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
        public int ExecQuery(string connection,SqlCommand cmd,int timeout = 0)
        {
            int rowsEffected = 0;
            using (SqlConnection conn = new SqlConnection(connection))
            {
                try
                {
                    cmd.Connection = conn;
                    cmd.CommandTimeout = timeout;
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
        public Object ExecScalar(string connection, SqlCommand cmd)
        {
            var obj = new Object();
            using (SqlConnection conn = new SqlConnection(connection))
            {
                try
                {
                    cmd.Connection = conn;
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
        public void BackupDatabase(string connection,string dbname, string path) //Path value must not contain new backup file name it is generated in the function
        {
            string query = $@"BACKUP DATABASE {dbname} TO DISK = '{path}\{dbname + DateTime.Now.ToString(" H-mm-ss dd-MM-yyyy")}.bak'";
            var cmd = new SqlCommand(query);
            ExecQuery(connection,cmd);
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
            return 0;
        }
        public int TruncateAllTables(string connection)
        {
            return 0;
        }
        public int DeleteTable(string connection, string table)
        {
            return 0;
        }
        public int DeleteDatabase(string connection, string dbname)
        {
            return 0;
        }
        //exec READER
        //get DATASET 
        //public List<string> GetStringColunmn()
        //{

        //}
        //public List<int> GetIntColunmn()
        //{

        //}
        //public List<long> GetLongColunmn()
        //{

        //}

    }
}
