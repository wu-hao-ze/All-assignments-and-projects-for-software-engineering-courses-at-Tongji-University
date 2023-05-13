using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using SqlSugar;

namespace MeeTeam_Backend
{
    [SugarTable("Browse_history")]
    public class Browsehistory
    {
        //主码需要特殊说明
        [SugarColumn(IsPrimaryKey = true)]
        public string user_id { get; set; }

        [SugarColumn(IsPrimaryKey = true)]
        public string project_id { get; set; }

        public string browse_time { get; set; }

    }
}