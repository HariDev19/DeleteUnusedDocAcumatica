using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using DeleteUnusedDocAcumatica.ServicesAcum;

namespace DeleteUnusedDocAcumatica.DAC
{
    public class DataAccessClass
    {
        private static LoginResult serviceResult;

        public static DataTable getDataTable(string server, string dbName, string userDb, string passDb, string strSelect)
        {
            using (SqlConnection con = new SqlConnection("server = " + server + "; database = " + dbName + "; user = " + userDb + "; password = " + passDb + ""))
            using (SqlCommand com = new SqlCommand(strSelect, con))
            {
                con.Open();
                com.CommandTimeout = 0;
                SqlDataAdapter sda = new SqlDataAdapter(com);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                con.Close();
                return dt;
            }
        }

        public static bool executeQuery(string server, string dbName, string userDb, string passDb, string query)
        {
            bool bl;
            using (SqlConnection con = new SqlConnection("server = " + server + "; database = " + dbName + "; user = " + userDb + "; password = " + passDb + ""))
            using (SqlCommand com = new SqlCommand(query, con))
            {
                con.Open();
                try
                {
                    com.ExecuteNonQuery();
                    bl = true;
                }
                catch (Exception x)
                {
                    string msg = x.Message;
                    bl = false;
                }
                finally
                {
                    con.Close();
                }
            }
            return bl;
        }

        public static bool getLoginAcumatica(Screen context, string url, string login, string pass)
        {
            bool ret = true;
            try
            {
                context.CookieContainer = new System.Net.CookieContainer();
                //context.AllowAutoRedirect = true;
                //context.EnableDecompression = true;
                //context.Timeout = 100000000;
                context.Url = url;
                serviceResult = context.Login(login, pass);
            }
            catch(Exception ex)
            {
                ret = false;
                string msg = ex.Message;
            }
            
            return ret;
        }

        public static bool getLogoutAcumatica(Screen context)
        {
            bool ret = true;
            try
            {
                context.Logout();
            }
            catch(Exception ex)
            {
                ret = false;
                string msg = ex.Message;
            }

            return ret;
        }
    }
}
