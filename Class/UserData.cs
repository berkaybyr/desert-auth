using EasMe;
using System.Data;
using System.Data.SqlClient;

namespace desert_auth.Class
{
    public class UserData
    {
        public long GetUserNobyUsername(string username)
        {
            var cmd = new SqlCommand("SELECT _userNo FROM PaGamePrivate.TblUserInformation WHERE _userId LIKE @username");
            cmd.Parameters.AddWithValue("@username", $"{username},%");
            var userNo = EasQL.ExecScalar(Service.WorldConn, cmd);
            if (userNo != null)
            {
                return long.Parse(userNo.ToString());
            }
            return 0;
        }
        public long GetUserNobyFamName(string familyname)
        {
            string sql = "Select _userNo from PaGamePrivate.TblUserInformation where _userNickname = @familyname ";
            long userid = -1;
            using (SqlConnection conn = new SqlConnection(Service.WorldConn))
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@familyname", SqlDbType.NVarChar)).Value = familyname;
                try
                {
                    conn.Open();
                    var scalar = cmd.ExecuteScalar();
                    if (scalar != null)
                        userid = long.Parse(scalar.ToString());
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return userid;
            }
        }
        public int GetUserNobyCharName(string charname)
        {
            string sql = "Select _userNo from PaGamePrivate.TblCharacterInformation where _characterName = @charName ";
            int userid = -1;
            using (SqlConnection conn = new SqlConnection(Service.GameConn))
            {

                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.Add(new SqlParameter("@charName", charname));
                    var scalar = cmd.ExecuteScalar();
                    if (scalar != null)
                        userid = Int32.Parse(scalar.ToString());
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            return userid;
        }

        public string GetFamNamebyUserNo(long userNo)
        {

            string sql = "Select _userNickname from PaGamePrivate.TblUserInformation where _userNo = @userid ";
            using (SqlConnection conn = new SqlConnection(Service.WorldConn))
            {

                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.Add(new SqlParameter("@userid", userNo));
                    var scalar = cmd.ExecuteScalar();
                    if (scalar != null)
                        return scalar.ToString();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }


            return "";
        }
        public string GetCharNamebyCharNo(long charno)
        {

            string sql = "SELECT _characterName from PaGamePrivate.TblCharacterInformation WHERE _characterNo = @charid ";
            using (SqlConnection conn = new SqlConnection(Service.GameConn))
            {

                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.Add(new SqlParameter("@charid", charno));
                    var scalar = cmd.ExecuteScalar();
                    if (scalar != null)
                        return scalar.ToString();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }


            return "";
        }
        public string GetFamNamebyCharNo(long charno)
        {

            string sql = "SELECT _userNo from PaGamePrivate.TblCharacterInformation WHERE _characterNo = @charid ";
            using (SqlConnection conn = new SqlConnection(Service.GameConn))
            {

                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.Add(new SqlParameter("@charid", charno));
                    var scalar = cmd.ExecuteScalar();
                    if (scalar != null)
                        return GetFamNamebyUserNo(long.Parse(scalar.ToString()));
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }


            return "";
        }
        public DateTime GetPremiumEndTimebyID(long userNo)
        {
            string sql = "SELECT _starterPackageBuffExpiration from PaGamePrivate.TblBriefUserInformation WHERE _userNo = @userid ";
            using (SqlConnection conn = new SqlConnection(Service.GameConn))
            {

                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.Add(new SqlParameter("@userid", userNo));
                    var scalar = cmd.ExecuteScalar();
                    if (scalar != null)
                        return DateTime.Parse(scalar.ToString());
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return DateTime.Parse("1900-01-01");
            }
        }
        public string GetPassword(string FAMILYNAME)
        {
            string sql = "SELECT _realPassword from PaGamePrivate.TblUserInformation WHERE _userNickname = @famname";
            using (SqlConnection conn = new SqlConnection(Service.WorldConn))
            {

                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.Add(new SqlParameter("@famname", FAMILYNAME));
                    var scalar = cmd.ExecuteScalar();
                    if (scalar != null)
                        return scalar.ToString();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            return "";
        }
        public long GetBalancebyUsername(string USERNAME, string PASSWORD)
        {
            try
            {
                var cmd = new SqlCommand("SELECT _balance FROM PaGamePrivate.TblUserInformation WHERE _userId = @userId AND _realPassword = @password AND _userName = @username AND _isValid = 1");
                cmd.Parameters.AddWithValue("@userId", $"{USERNAME},{PASSWORD}");
                cmd.Parameters.AddWithValue("@password", PASSWORD);
                cmd.Parameters.AddWithValue("@username", USERNAME);
                var Balance = EasQL.ExecScalar(Service.WorldConn, cmd);
                if (Balance != null)
                {
                    return long.Parse(Balance.ToString());
                }
                return -1;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }
        public long UpdateBalance(string FAMILYNAME, long BALANCE)
        {
            try
            {
                long oldBalance = GetBalancebyUsername(FAMILYNAME, GetPassword(FAMILYNAME));
                if (oldBalance == -1)
                    return -1;
                if (BALANCE < 0)
                {
                    if (oldBalance + BALANCE < 0)
                    {
                        return -2;
                    }
                }
                string query = "UPDATE PaGamePrivate.TblUserInformation SET _balance = @balance WHERE _userNickname = @familyname AND _isValid = 1";
                var cmd = new SqlCommand(query);
                cmd.Parameters.AddWithValue("@balance", oldBalance + BALANCE);
                cmd.Parameters.AddWithValue("@familyname", FAMILYNAME);
                var result = EasQL.ExecNonQuery(Service.WorldConn, cmd);
                if (result > 0)
                {
                    return oldBalance + BALANCE;
                }
                return -1;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }
        public long TransferBalance(string fromFamilyName, string fromPassword, string toFamilyname, long CASH)
        {
            var dbPassword = GetPassword(fromFamilyName);
            long fromOldBalance = GetBalancebyUsername(fromFamilyName, fromPassword);
            if (dbPassword != fromPassword)
                return -1;
            if (fromOldBalance == -1)
                return -1;
            if (CASH < 0)
                return -1;
            if (fromOldBalance - CASH < 0)
                return -2;

            string query = "UPDATE PaGamePrivate.TblUserInformation SET _balance = @balance WHERE _userNickname = @familyname AND _isValid = 1";
            var cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@balance", fromOldBalance - CASH);
            cmd.Parameters.AddWithValue("@familyname", fromFamilyName);
            var result = EasQL.ExecNonQuery(Service.WorldConn, cmd);
            if (result > 0)
            {
                long toOldBalance = GetBalancebyUsername(toFamilyname, GetPassword(toFamilyname));
                query = "UPDATE PaGamePrivate.TblUserInformation SET _balance = @balance WHERE _userNickname = @familyname AND _isValid = 1";
                cmd.Parameters.Clear();
                cmd = new SqlCommand(query);
                cmd.Parameters.AddWithValue("@balance", toOldBalance + CASH);
                cmd.Parameters.AddWithValue("@familyname", toFamilyname);
                result = EasQL.ExecNonQuery(Service.WorldConn, cmd);
                if (result > 0)
                    return 0;

            }
            return -1;
        }
    }
}
