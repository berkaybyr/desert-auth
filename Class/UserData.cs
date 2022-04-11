using EasMe;
using System.Data;
using System.Data.SqlClient;

namespace desert_auth.Class
{
    public class UserData
    {
        Service _s = new Service(true);
        EasQL _sql = new EasQL();
        public long GetUserNobyUsername(string username)
        {
            var cmd = new SqlCommand("SELECT _userNo FROM PaGamePrivate.TblUserInformation WHERE _userId LIKE @username");
            cmd.Parameters.AddWithValue("@username", $"{username},%");
            var userNo = _sql.ExecScalar(_s.WorldConn,cmd);
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
            using (SqlConnection conn = new SqlConnection(_s.WorldConn))
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
            using (SqlConnection conn = new SqlConnection(_s.GameConn))
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
            using (SqlConnection conn = new SqlConnection(_s.WorldConn))
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
            using (SqlConnection conn = new SqlConnection(_s.GameConn))
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
            using (SqlConnection conn = new SqlConnection(_s.GameConn))
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
    }
}
