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
            //if (ID == "sbw")
            //{
            //    my.user_id = "0";
            //    my.project_id = "1";
            //    my.duty = "组长";
            //    my1.user_id = "2";
            //    my1.project_id = "3";
            //    my1.duty = "组员";
            //    agrlist.Add(my);
            //    agrlist.Add(my1);
            //}
            //return agrlist;
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
            //SqlSugarClient db = dborm.getInstance();//获取数据库连接
            //try
            //{
            //    my = db.Queryable<Project>().Where(it => it.project_id == project_id).First();
            //    return my.project_name;
            //}
            //catch (Exception)
            //{
            //    return null;
            //}
        }

        [HttpGet]
        public List<Facorite> GetFacorite(string ID)
        {
            dbORM dborm = new dbORM();
            List<Facorite> agrlist = new List<Facorite>();
            //Facorite my = new Facorite();
            //my.facorite_id = "1";
            //my.owner_id = ID;
            //my.project_id = "number1";
            //my.facorite_time = "2022-8-30";
            //agrlist.Add(my);
            //Facorite my1 = new Facorite();
            //my1.facorite_id = "2";
            //my1.owner_id = ID;
            //my1.project_id = "number2";
            //my1.facorite_time = "2022-8-31";
            //agrlist.Add(my1);
            //return agrlist;
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
