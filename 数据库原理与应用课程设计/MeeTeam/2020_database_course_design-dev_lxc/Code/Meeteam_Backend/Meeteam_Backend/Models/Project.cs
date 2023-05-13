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
        public string project_name { get; set; }
        public string project_background { get; set; }
        public string project_introduction { get; set; }
        public string project_content { get; set; }
        public string project_status { get; set; }
        public string create_time { get; set; }
        public string due { get; set; }
        public string project_progress { get; set; }
        public string admin_id { get; set; }
        public string audit_reason { get; set; }
        public string audit_result { get; set; }
        public string audit_time { get; set; }
        public string audit_status { get; set; }
        public string recommend { get; set; }
        public string recommend_time { get; set; }
    }

    public class Project_Query
    {
        public string project_id { get; set; }//按项目id查询（唯一）
        public string project_name { get; set; }//按项目名查询（字符串子串）
        public string publisher { get; set; }//按发布者查询
        public string project_progress { get; set; }//按项目进展查询
        public string audit_status { get; set; }//按审核状态查询
        public string hav_require { get; set; }//是否有组队需求
        public string require_id { get; set; }//按需求id查询（唯一）
        public string require_status { get; set; }//按需求状态查询
        public string team_type { get; set; }//按组队类型查询
    }
}
