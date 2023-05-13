using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SqlSugar;

namespace Meeteam_Backend
{
    [SugarTable("user_user")]
    public class User_User
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string main_user_id { get; set; }

        [SugarColumn(IsPrimaryKey = true)]
        public string related_user_id { get; set; }
        
        public string relation { get; set; }
    }
}
