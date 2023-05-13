using System;
using System.Data; //to user the plug to introduce the 'using'
using Oracle.ManagedDataAccess.Client;
using System.Data.SqlClient;
using SqlSugar;


namespace Meeteam_Backend
{
    [SugarTable("user_info")]
    public class User_Info
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string user_id { get; set; }
        public string password { get; set; }
        public string register_time { get; set; }
        public string account_status { get; set; }
        public string user_name { get; set; }
        public string gender { get; set; }//oracle的char对应的是string
        public string contact_info { get; set; }
        public string institution { get; set; }
        public string major { get; set; }
        public string grade { get; set; }
        public string introduction { get; set; }
        public int point { get; set; }
        public string head_img { get; set; }
    }
}
