using System;
using System.Data; //to user the plug to introduce the 'using'
using Oracle.ManagedDataAccess.Client;
using System.Data.SqlClient;
using SqlSugar;


namespace Meeteam_Backend
{
    [SugarTable("grouping_requirement")]
    public class Grouping_Requirement
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string require_id { get; set; }

        [SugarColumn(IsJson = true)]
        public string release_time { get; set; }
        public string purpose { get; set; }
        public string team_type { get; set; }
        public string team_limit { get; set; }
        public string details { get; set; }
        public string require_status { get; set; }
        public string project_id { get; set; }
        public string region { get; set; }
        public string team_type_details { get; set; }
    }
}

