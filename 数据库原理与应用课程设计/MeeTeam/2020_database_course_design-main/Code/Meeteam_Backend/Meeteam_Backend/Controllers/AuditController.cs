using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using SqlSugar;
using MeeTeam_Backend;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace Meeteam_Backend.Controllers
{
    [Route("/[Controller]/[action]")]
    [ApiController]
    [EnableCors("any")]
    public class AuditController : ControllerBase
    {
        [HttpPost]
        //修改项目的审核状态
        public int Project_Audit(string project_id, string admin_id, string result, string reason)
        {
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接
            bool have_p = db.Queryable<Project>().Any(it => it.project_id == project_id);
            bool have_a = db.Queryable<Administrator>().Any(it => it.admin_id == admin_id);
            if (!have_p || !have_a)
                return 0;
            string p_id = project_id;
            List<Grouping_Requirement> have_r = db.Queryable<Grouping_Requirement>().Where(it => it.project_id == p_id).ToList();
            string time = DateTime.Now.ToString();
            if (have_r.Count!=0)
            {
                return db.Updateable<Project>()
                    .SetColumns(it => new Project
                    {
                        project_progress = "招募中",
                        audit_result = result,
                        audit_time = time,
                        admin_id = admin_id,
                        audit_reason = reason,
                        audit_status = "1"
                    })
                    .Where(it => it.project_id == project_id)
                    .ExecuteCommand();
            }
            else
            {
                return db.Updateable<Project>()
                     .SetColumns(it => new Project
                     {
                         project_progress = "已发布",
                         audit_result = result,
                         audit_time = time,
                         admin_id = admin_id,
                         audit_reason = reason,
                         audit_status = "1"
                     })
                     .Where(it => it.project_id == project_id)
                     .ExecuteCommand();
            }
        }
    }
}
