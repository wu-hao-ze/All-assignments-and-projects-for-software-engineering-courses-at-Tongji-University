using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using SqlSugar;

namespace MeeTeam_Backend
{
    public enum OPERATE_TYPE { SAVE, PUBLISH };
    [SugarTable("notice")]
    public class Notice
    {
        [SugarColumn(IsPrimaryKey = true, OracleSequenceName = "NOTICE_SEQ")]
        public string notice_id { get; set; }
        public string notice_title { get; set; }
        public string notice_content { get; set; }
        public string admin_id { get; set; }
        public OPERATE_TYPE operate_type { get; set; }
        public string operate_time { get; set; }
    }

    public class Notice_Helper
    {
        public string search_by_title(string title)
        {
            string id = null;
            return id;
        }

        public string search_by_content(string content)
        {
            string id = null;
            return id;
        }

        public string search_by_admin(string a_id)
        {
            string id = null;
            return id;
        }


    }
}

