using System;
using System.Data; //to user the plug to introduce the 'using'
using Oracle.ManagedDataAccess.Client;
using System.Data.SqlClient;
using SqlSugar;


namespace Meeteam_Backend
{
    [SugarTable("Project_Evaluation")]
    public class Project_Evaluation
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string project_eva_id { get; set; }
        public string evaluator_id { get; set; }
        public string project_id { get; set; }
        public string eva_type { get; set; }
        public string eva_time { get; set; }
        public string eva_content { get; set; }
        public string eva_status7 { get; set; }//7?
    }
}
