using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using System.Data;
using System.Collections;
using SqlSugar;
using MeeTeam_Backend;

namespace Meeteam_Backend.Controllers
{
    [Route("/[controller]/[action]")]
    [ApiController]
    [EnableCors("any")]
    public class UserProjectController : ControllerBase
    {

        [HttpGet]
        public List<User_Project> GetProjectID(string ID)
        {
            User_Project my = new User_Project();
            User_Project my1 = new User_Project();
            dbORM dborm = new dbORM();
            List<User_Project> agrlist=new List<User_Project>();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接
            try
            {
                agrlist = db.Queryable<User_Project>().Where(
                                                        it => it.user_id == ID
                                                        ).ToList();
                return agrlist;
            }
            catch (Exception)
            {
                return null;
            }
        }


        [HttpGet]
        public string GetProjectName(string project_id)
        {
            Project my = new Project();
            dbORM dborm = new dbORM();
            my.project_id = project_id;
            my.project_name = "项目" + project_id;
            return my.project_name;
        }


        [HttpGet]
        public List<Facorite> GetFacorite(string ID)
        {
            dbORM dborm = new dbORM();
            List<Facorite> agrlist = new List<Facorite>();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接
            try
            {
                agrlist = db.Queryable<Facorite>().Where(it => it.owner_id == ID).ToList();
                return agrlist;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
