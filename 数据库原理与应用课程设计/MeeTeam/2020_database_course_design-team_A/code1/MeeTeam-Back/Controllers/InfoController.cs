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
    [Route("/[Controller]/[action]")]
    [ApiController]
    [EnableCors("any")]
    public class InfoController : ControllerBase
    {
        [HttpGet]
        public List<User_Info> getInfo(string ID)
        {
            User_Info info = new User_Info();
            dbORM dborm = new dbORM();
            List<User_Info> agrlist;
            //info.user_id = "admin";
            //info.user_name = "sbw";
            //info.point = 1;
            SqlSugarClient db = dborm.getInstance();//获取数据库连接
            try
            {
                agrlist = db.Queryable<User_Info>().Where(it => it.user_id == ID).ToList();
                return agrlist;
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpGet]
        public bool PullInfo(string ID, char gender, string phone_number,
                                string institution, string major, string introduction)
        {
            User_Info info = new User_Info();
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接
            try
            {
                info.gender = gender;
                info.contact_info = phone_number;
                info.institution = institution;
                info.major = major;
                info.introduction = introduction;
                int mount = db.Updateable<User_Info>().Where(it => it.user_id == ID).ExecuteCommand();
                if (mount > 0)
                    return true;
                else return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
