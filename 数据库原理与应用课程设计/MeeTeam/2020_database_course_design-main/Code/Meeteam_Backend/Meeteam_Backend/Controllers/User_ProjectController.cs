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
        public string get_by_id(string pid, string uid)
        {
            User_Project project = new User_Project();
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接
                                                 
            project = db.Queryable<User_Project>().Where(it => it.project_id == pid && it.user_id == uid).First();
            return JsonSerializer.Serialize(project);
        }
        //查询申请者列表
        [HttpGet]
        public List<User_Project> get_member(string pid, string duty)
        {
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接
            try
            {
                List<User_Project> agrlist = db.Queryable<User_Project>().Where(it => it.project_id == pid && it.duty == duty).ToList();
                return agrlist;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        //查询发布者
        [HttpGet]
        public string get_leader(string id)
        {
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接
            try
            {
                string name = db.Queryable<User_Project>().Where(it => it.project_id == id && it.duty == "发布者").First().user_id;
                return name;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        //发布者审核
        [HttpPost]
        public bool Audit(string pid, string uid,string dy)
        {
            User_Project project = new User_Project();
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接
            try
            {
                int result = db.Updateable<User_Project>().SetColumns(it => it.duty == dy).Where(it => it.project_id == pid && it.user_id == uid).ExecuteCommand();
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
        //删除申请
        [HttpDelete]
        public bool DeleteApply(string pid,string uid)
        {
            //获取数据库连接
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();
            //删除
            try
            {
                int count = db.Deleteable<User_Project>().Where(it => it.project_id == pid && it.user_id == uid).ExecuteCommand();
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
