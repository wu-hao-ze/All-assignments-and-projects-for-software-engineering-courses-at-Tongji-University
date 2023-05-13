using System;
using System.Data; //to user the plug to introduce the 'using'
using Oracle.ManagedDataAccess.Client;
using System.Data.SqlClient;
using SqlSugar;


namespace Meeteam_Backend
{
    [SugarTable("User_Project")]
    public class User_Project
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string project_id { get; set; }
        public string user_id { get; set; }
        public string duty { get; set; }
    }
}
