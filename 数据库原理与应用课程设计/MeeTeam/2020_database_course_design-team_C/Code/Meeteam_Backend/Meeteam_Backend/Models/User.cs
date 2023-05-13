using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using SqlSugar;

namespace MeeTeam_Backend
{
    [SugarTable("user_info")]
    public class User_Info
    {
        //主码需要特殊说明
        [SugarColumn(IsPrimaryKey = true)]
        public string user_id { get; set; }
        public string password { get; set; }
        [SugarColumn(IsJson = true)]
        public string register_time { get; set; }//数据类型存疑，先这么写着，date似乎需要转换成json？
        public bool account_status { get; set; }
        public string user_name { get; set; }
        public bool gender { get; set; }
        public string contact_info { get; set; }
        public string institution { get; set; }
        public string major { get; set; }
        public string grade { get; set; }
        public string instruction { get; set; }
        public int point { get; set; }
    }

    public class UserInfo
    {
        public string get_Password(string user_id)
        {
            string password = null;
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接
            //查询
            var list = db.Queryable<User_Info>()
                .Select(it => it.password)
                .Where("user_id=@id", new { id = user_id })
                .ToList();
            password = list[0].ToString();
            return password;
        }
    }

}
