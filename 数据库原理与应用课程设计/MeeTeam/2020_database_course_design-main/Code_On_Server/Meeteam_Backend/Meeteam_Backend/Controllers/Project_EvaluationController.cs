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
    public class Project_EvaluationController : ControllerBase
    {
        //上传项目
        [HttpPost]
        public bool AddProject_Evaluation(string project_eva_id, string project_id, string evaluator_id, string eva_content)
        {
            //获取数据库连接
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();

            Project_Evaluation pos = new Project_Evaluation();
            pos.project_eva_id = project_eva_id;
            pos.project_id = project_id;
            pos.evaluator_id = evaluator_id;
            pos.eva_time = DateTime.Now.ToString("g"); //2009/10/30 20:40;
            pos.eva_content = eva_content;

            int count = db.Insertable(pos).ExecuteCommand();
            if (count == 1)
                return true;
            else
                return false;
        }


        //查询全部评论，返回一个对象
        [HttpGet]
        public List<Project_Evaluation> SelectAllProject_Evaluation()
        {
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接
            try
            {
                List<Project_Evaluation> agrlist = db.Queryable<Project_Evaluation>().ToList();
                return agrlist;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        //查询id的项目评论
        [HttpGet]
        public string get_by_id(string id)
        {
            Project_Evaluation project_eva = new Project_Evaluation();
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接
            //搜索条件，感觉可以改成检索的方式
            project_eva = db.Queryable<Project_Evaluation>().Where(it => it.project_id == id).First();
            return JsonSerializer.Serialize(project_eva);
        }


        //删除评论
        [HttpDelete]
        public bool deleteProject(string id)
        {
            //获取数据库连接
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();
            //删除
            try
            {
                int count = db.Deleteable<Project_Evaluation>().In(id).ExecuteCommand();
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