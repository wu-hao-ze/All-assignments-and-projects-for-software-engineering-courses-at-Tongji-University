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
            if (ID == "sbw")
            {
                my.user_id = "0";
                my.project_id = "1";
                my.duty = "组长";
                my1.user_id = "2";
                my1.project_id = "3";
                my1.duty = "组员";
                agrlist.Add(my);
                agrlist.Add(my1);
            }
            return agrlist;
            //SqlSugarClient db = dborm.getInstance();//获取数据库连接
            //try
            //{
            //    agrlist = db.Queryable<User_Project>().Where(
            //                                            it => it.user_id == ID
            //                                            ).ToList();
            //    return agrlist;
            //}
            //catch (Exception)
            //{
            //    return null;
            //}
        }

        [HttpGet]
        public Project GetProjectName(string project_id)
        {
            Project my = new Project();
            dbORM dborm = new dbORM();
            my.project_id = project_id;
            my.project_name =  "组长"+project_id;
            return my;
            //SqlSugarClient db = dborm.getInstance();//获取数据库连接
            //try
            //{
            //    my = db.Queryable<Project>().Where(it => it.project_id == ID).First();
            //    return my.project_name;
            //}
            //catch (Exception)
            //{
            //    return null;
            //}
        }
    }
}
