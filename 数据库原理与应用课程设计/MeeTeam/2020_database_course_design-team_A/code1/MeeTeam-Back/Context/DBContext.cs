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

    [SugarTable("user_info")]
    public class User_Info
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string user_id { get; set; }

        [SugarColumn(IsJson = true)]
        public string password { get; set; }
        public string purpose { get; set; }
        public string team_type { get; set; }
        public string team_limit { get; set; }
        public string details { get; set; }
        public string require_status { get; set; }
        public string originator_id { get; set; }
        public string project_id { get; set; }
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
