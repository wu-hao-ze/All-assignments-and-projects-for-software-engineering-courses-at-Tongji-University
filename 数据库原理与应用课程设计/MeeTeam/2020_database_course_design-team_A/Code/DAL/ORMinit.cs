using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL
{
    public class DBClient
    {
        private const string connetInfo = 
            "Data Source = LAPTOP-OL164KED; Initial Catalog = Ctgu; User ID = sa; Password = 199812"; //照着DBHelper.cs写的
        SqlSugarClient db;
        public void initConnection()
        {
            ConnectionConfig config = new ConnectionConfig()
            {
                ConnectionString = connetInfo,
                DbType = DbType.Oracle,
                IsAutoCloseConnection = true, //自动关闭数据库连接
            };
            db = new SqlSugarClient(config);
        }
    }
}
