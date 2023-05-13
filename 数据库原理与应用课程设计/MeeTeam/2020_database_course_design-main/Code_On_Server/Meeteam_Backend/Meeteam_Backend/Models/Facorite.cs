using System;
using System.Data; //to user the plug to introduce the 'using'
using Oracle.ManagedDataAccess.Client;
using System.Data.SqlClient;
using SqlSugar;


namespace Meeteam_Backend
{
    [SugarTable("Facorite")]
    public class Facorite
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string facorite_id { get; set; }
        public string owner_id { get; set; }
        public string project_id { get; set; }
        public string facorite_time { get; set; }
    }
}

