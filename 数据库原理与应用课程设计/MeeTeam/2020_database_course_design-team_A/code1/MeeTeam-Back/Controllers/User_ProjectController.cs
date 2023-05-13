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
    public class User_ProjectController : Controller
    {
        //上传Project_User
        [HttpPost]
        public bool AddUser_Project(string project_id, string user_id, string duty)
        {
            //获取数据库连接
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();

            User_Project pos = new User_Project();
            pos.project_id = project_id;
            pos.user_id = user_id;
            pos.duty = duty;

            int count = db.Insertable(pos).ExecuteCommand();
            if (count == 1)
                return true;
            else
                return false;
        }
        //查询id的项目
        [HttpGet]
        public string get_by_id(string id)
        {
            User_Project project = new User_Project();
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接
                                                 
            project = db.Queryable<User_Project>().Where(it => it.project_id == id).First();
            return JsonSerializer.Serialize(project);
        }
    }
}
