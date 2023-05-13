using System;
using System.Data; //to user the plug to introduce the 'using'
using Oracle.ManagedDataAccess.Client;
using System.Data.SqlClient;
using SqlSugar;


namespace Meeteam_Backend
{
    [SugarTable("Project")]
    public class Project
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string project_id { get; set; }

        [SugarColumn(IsJson = true)]
        public string project_name { get; set; }
        public string project_background { get; set; }
        public string project_introduction { get; set; }
        public string project_content { get; set; }
        public string project_status { get; set; }
        public string create_time { get; set; }
        public string due { get; set; }
        public string project_progress { get; set; }
        public string admin_id { get; set; }
        public string audit_result { get; set; }
        public string audit_time { get; set; }
    }
}
