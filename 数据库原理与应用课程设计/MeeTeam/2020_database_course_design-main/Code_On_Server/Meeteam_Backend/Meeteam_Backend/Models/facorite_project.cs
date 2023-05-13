using System;
using System.Data; //to user the plug to introduce the 'using'
using Oracle.ManagedDataAccess.Client;
using System.Data.SqlClient;
using SqlSugar;


namespace Meeteam_Backend
{
    [SugarTable("facorite_project")]
    public class facorite_project
    {
        public string facorite_id { get; set; }
        public string project_id { get; set; }
    }
}
