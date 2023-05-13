using System;
using System.Data; //to user the plug to introduce the 'using'
using Oracle.ManagedDataAccess.Client;
using System.Data.SqlClient;
using SqlSugar;

namespace MeeTeam_Backend
{
    public class dbORM
    {

        public SqlSugarClient getInstance()
        {
            SqlSugarClient db = new SqlSugarClient(connect_info);
            return db;
        }
        public static ConnectionConfig connect_info = new ConnectionConfig()
        {
            ConnectionString = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=106.14.193.8)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=orcl)));Persist Security Info=True;User ID=normal;Password=TJUsoftware2020;",
            DbType = SqlSugar.DbType.Oracle,
            IsAutoCloseConnection = true,
            InitKeyType = SqlSugar.InitKeyType.Attribute
        };
    }
    public class UserDB
    {
        public static string connString = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=106.14.193.8)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=orcl)));Persist Security Info=True;User ID=normal;Password=TJUsoftware2020;";
        public OracleConnection con;
        public bool connectOracle()
        {
            try
            {
                // 建立SqlConnection对象
                con = new OracleConnection(connString);
                // 打开连接
                con.Open();
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
