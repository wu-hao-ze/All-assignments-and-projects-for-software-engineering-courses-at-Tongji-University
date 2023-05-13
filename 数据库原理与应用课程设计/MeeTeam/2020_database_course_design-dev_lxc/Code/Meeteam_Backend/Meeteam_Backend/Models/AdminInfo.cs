using System;
using System.Data; //to user the plug to introduce the 'using'
using Oracle.ManagedDataAccess.Client;
using System.Data.SqlClient;
using SqlSugar;


namespace Meeteam_Backend
{
    [SugarTable("administrator")]
    public class Administrator
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string admin_id { get; set; }
        public string password { get; set; }
        public string register_time { get; set; }
        public string account_status { get; set; }
        public string user_name { get; set; }
        public string gender { get; set; }
        public string contact_info { get; set; }
    }
}
