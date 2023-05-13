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
        [HttpPost]
        public bool AddProject(string project_id, string project_name, string project_background, string project_introduction, string project_content, string project_status, string due, string project_progress)
        {
            //获取数据库连接
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();

            Project pos = new Project();
            pos.project_id = project_id;
            pos.project_name = project_name;
            pos.project_background = project_background;
            pos.project_introduction = project_introduction;
            pos.project_content = project_content;
            pos.project_status = project_status;
            pos.create_time = DateTime.Now.ToString("g"); //2009/10/30 20:40;
            pos.admin_id = "system";
            pos.audit_result = "0";
            pos.due = due;
            pos.project_progress = project_progress;

            int count = db.Insertable(pos).ExecuteCommand();
            if (count == 1)
                return true;
            else
                return false;
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
    }
}