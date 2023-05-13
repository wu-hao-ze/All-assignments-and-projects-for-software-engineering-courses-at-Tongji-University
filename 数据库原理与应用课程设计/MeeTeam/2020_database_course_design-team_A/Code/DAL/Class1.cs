using System;
using Model;
using System.Data; //to user the plug to introduce the 'using'
using Oracle.ManagedDataAccess.Client;
using System.Data.SqlClient;
//using System.Windows.Forms;

/**
 * 这是数据访问层，用于实现和数据库的交互
 * 
 */
namespace DAL
{
    public class UserDB
    {
        private static string connString = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=106.14.193.8)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=orcl)));Persist Security Info=True;User ID=normal;Password=TJUsoftware2020;";
        private OracleConnection con;

        public bool connectOracle()
        {
            try
            {
                // 建立SqlConnection对象
                con = new OracleConnection(connString);
                // 打开连接
                con.Open();
                Console.WriteLine("连接成功");
                return true;

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
 

        public bool login(UserInfo info)
        {
            if (!connectOracle())
            {
                return false;
            }
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "select USER_ID,PASSWORD from USER_INFO where USER_ID='" + info.UserID + "' and PASSWORD='" + info.PassWord+ "'";

            OracleDataReader odr = cmd.ExecuteReader();
            bool iflogin = false;

            while (odr.Read())
            {
                string currusername = odr["USER_ID"].ToString();
                string currpwd = odr["PASSWORD"].ToString();

                if (currusername == info.UserID && currpwd == info.PassWord)
                {
                    iflogin = true;
                }
            }
            
            con.Close();
            
            return iflogin;
        }

        public bool IsEqual(UserInfo info)
        {
     
            if (!connectOracle())
            {
                return false; //true比较好？
            }
            bool already = false;

            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "select USER_ID from USER_INFO where USER_ID='" + info.UserID + "'";
            OracleDataReader odr = cmd.ExecuteReader();

            while (odr.Read())
            {
                string currusername = odr["USER_ID"].ToString();
            //    string currpwd = odr["PASSWARD"].ToString();

                if (currusername == info.UserID)
                {
                    already = true;
                }
            }

            con.Close();

            return already;
        }

        
        public int AddUser(UserInfo info)
        {
            if (!connectOracle())
            {
                return -1; 
            }

            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "insert into USER_INFO(USER_ID,PASSWORD) values('" + info.UserID + "','" + info.PassWord + "')";
           // OracleDataReader odr = cmd.ExecuteReader();

            int result = cmd.ExecuteNonQuery();
            if (result > 0)
            {
                Console.WriteLine("插入成功!");
                return 0;
            }
            else
            {
                Console.WriteLine("插入失败！");
                return -1;
            }


        }

    }
}
