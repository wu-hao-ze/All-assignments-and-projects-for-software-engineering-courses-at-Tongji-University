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
    public class ProjectController : ControllerBase
    {
        //上传项目
        [HttpGet]
        public string AddProject(string project_name, string project_background, string project_introduction, string project_content, string project_status, string due, string project_progress)
        {
            //获取数据库连接
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();

            Project pos = new Project();
            pos.project_name = project_name;
            pos.project_background = project_background;
            pos.project_introduction = project_introduction;
            pos.project_content = project_content;
            pos.project_status = project_status;
            pos.create_time = DateTime.Now.ToString("g"); //2009/10/30 20:40;
            pos.admin_id = "system";
            pos.audit_result = "0";
            pos.audit_status = "0";
            pos.audit_reason = null;
            pos.recommend = "0";
            pos.due = due;
            pos.project_progress = project_progress;

            long re_id = db.Insertable(pos).ExecuteReturnBigIdentity();//返回自增列

            string resdata = re_id+"";
            return resdata;
        }
        //查询全部组队需求，返回一个对象
        [HttpGet]
        public List<Project> SelectAllProject()
        {
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接
            try
            {
                List<Project> agrlist = db.Queryable<Project>().ToList();
                return agrlist;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        //查询id的项目
        [HttpGet]
        public string get_by_id(string id)
        {
            Project project = new Project();
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接
            //搜索条件，感觉可以改成检索的方式
            project = db.Queryable<Project>().Where(it => it.project_id == id).First();
            return JsonSerializer.Serialize(project);
        }
        //编辑更新项目
        [HttpPost]
        public bool Changeprojectlist(string project_id, string project_name, string project_background, string project_introduction, string project_content, string due)
        {
            //获取数据库连接
            Project project = new Project();
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接
            //搜索条件，感觉可以改成检索的方式
            project = db.Queryable<Project>().Where(it => it.project_id == project_id).First();
            project.project_name = project_name;
            project.project_background = project_background;
            project.project_introduction = project_introduction;
            project.project_content = project_content;
            project.due = due;
            try
            {
                int result = db.Updateable<Project>(project).Where(it => it.project_id == project_id).ExecuteCommand();
                if (result == 1)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        //结束项目
        [HttpPost]
        public bool UpdateProgress(string project_id,string project_progress)
        {
            //获取数据库连接
            Project project = new Project();
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接
            //搜索条件，感觉可以改成检索的方式
            project = db.Queryable<Project>().Where(it => it.project_id == project_id).First();
            project.project_progress = project_progress;
            try
            {
                int result = db.Updateable<Project>(project).Where(it => it.project_id == project_id).ExecuteCommand();
                if (result == 1)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        //删除项目
        [HttpDelete]
        public bool deleteProject(string project_id)
        {
            //获取数据库连接
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();
            //删除
            try
            {
                int count = db.Deleteable<Project>().In(project_id).ExecuteCommand();
                if (count == 1)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        [HttpPost]
        //修改推荐状态
        public int Recommend_Project(string project_id)
        {
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接
            bool have_p = db.Queryable<Project>().Any(it => it.project_id == project_id);
            if (!have_p)
                return 0;
            string time = DateTime.Now.ToString();
            return db.Updateable<Project>()
                .SetColumns(it => new Project
                {
                    recommend_time = time,
                    recommend = "1"
                })
                .Where(it => it.project_id == project_id)
                .ExecuteCommand();
        }

        [HttpGet]
        public List<Project> SelectAllRecommend()
        {
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接
            var rec = db.Queryable<Project>().Where(it => it.recommend == "1").ToList();
            return rec;
        }

        //查询项目
        [HttpGet]

        /*测试用例
{
  "project_id": "",
  "project_name": "",
  "publisher": "",
  "project_progess": "规划阶段",
  "audit_status": "1",
  "hav_require": "",
  "require_id": "",
  "require_status": "",
  "team_type": ""
}
         */
        public string project_query(string s)
        {
            Project_Query q = JsonSerializer.Deserialize<Project_Query>(s);
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();
            List<Project> res = new List<Project>();
            //三表联查
            var query = db.Queryable<Project, User_Project, Grouping_Requirement,Project_Img>(
                (p, up, gr,pi) => new JoinQueryInfos(
                    JoinType.Left, p.project_id == up.project_id,
                    JoinType.Full, p.project_id == gr.project_id,
                    JoinType.Full, p.project_id == pi.project_id
                    ));
            if (q.project_id != null && q.project_id != "")
                query.Where((p, up, gr) => p.project_id == q.project_id);
            if (q.project_name != null && q.project_name != "")
                query.Where((p, up, gr) => p.project_name.Contains(q.project_name));
            if (q.publisher != null && q.publisher != "")
                query.Where((p, up, gr) => up.user_id.Contains(q.publisher) && up.duty == "发布者");
            if (q.project_progress != null && q.project_progress != "")
                query.Where((p, up, gr) => p.project_progress == q.project_progress);
            if (q.hav_require == "1")
                query.Where((p, up, gr) => SqlFunc.HasValue(gr.project_id));
            else if (q.hav_require == "0")
                query.Where((p, up, gr) => SqlFunc.IsNullOrEmpty(gr.project_id));
            if (q.require_id != null && q.require_id != "")
                query.Where((p, up, gr) => gr.require_id == q.require_id);
            if (q.require_status != null && q.require_status != "")
                query.Where((p, up, gr) => gr.require_status == q.require_status);
            if (q.team_type != null && q.team_type != "")
                query.Where((p, up, gr) => gr.team_type == q.team_type);
            if (q.audit_status != null && q.audit_status != "")
                query.Where((p, up, gr) => p.audit_status == q.audit_status);
            if (q.audit_result != null && q.audit_result != "")
                query.Where((p, up, gr) => p.audit_result == q.audit_result);
            if(q.recommend != null && q.recommend != "")
                query.Where((p, up, gr) => p.recommend == q.recommend);
            var json = query.Select<ViewMode>().MergeTable().PartitionBy(it=>it.project_id).ToJson();
            return json;
        }


        [HttpGet]
        public List<Project> my_project(string user_id)
        {
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();
            var query= db.Queryable<Project, User_Project>(
                (p, up) => new JoinQueryInfos(
                    JoinType.Left, p.project_id == up.project_id
                    ));
            List<Project> res = new List<Project>();
            res = query.Where((p, up) => up.user_id == user_id && p.audit_result == "1").Select<Project>().Distinct().ToList();
            return res;
        }
    }

    public class ViewMode:Project
    {
        public string User_Projectuser_id { get; set; }//按发布者查询
        public string User_Projectduty { get; set; }
        public string Grouping_Requirementrequire_id { get; set; }//按需求id查询（唯一）
        public string Grouping_Requirementrequire_status { get; set; }//按需求状态查询
        public string Grouping_Requirementteam_type { get; set; }//按组队类型查询
        public  string Project_Imgimg_path { get; set; }
    }
}