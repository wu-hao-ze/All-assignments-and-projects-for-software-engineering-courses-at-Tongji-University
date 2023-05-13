using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SqlSugar;

namespace Meeteam_Backend.Models
{
    [SugarTable("administartor")]
    public class Administartor
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string admin_id { get; set; }

        [SugarColumn(IsJson = true)]
        public string password { get; set; }

    }
}
