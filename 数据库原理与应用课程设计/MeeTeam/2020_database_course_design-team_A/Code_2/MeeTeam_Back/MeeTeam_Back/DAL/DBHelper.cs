using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using System.Data.SqlClient;

namespace MeeTeam_Back
{
    class DBHelper
    {
        // 数据库连接字符串        
        public static string conStr = "Data Source = LAPTOP-OL164KED; Initial Catalog = Ctgu; User ID = sa; Password = 199812";
        //private static readonly string conStr = ConfigurationManager.ConnectionStrings["Ctgu"].ToString();
        // 数据库连接对象
        public static SqlConnection connection = new SqlConnection(conStr);
    }
}
