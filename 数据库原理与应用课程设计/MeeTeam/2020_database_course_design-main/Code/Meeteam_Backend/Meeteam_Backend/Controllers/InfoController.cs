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
            SqlSugarClient db = dborm.getInstance();//获取数据库连接
            List<User_Info> agrlist;
            //info.user_id = "admin";
            //info.user_name = "sbw";
            //info.point = 1;
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
        [HttpPost]
        //更新用户信息
        public bool ChangeInfo(string ID, string gender, string grade, string contact_info,
                                        string institution, string major, string introduction)
        {
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接
            try
            {
                User_Info info = db.Queryable<User_Info>()
                   .Where(t => t.user_id == ID).First();
                info.gender = gender;
                info.contact_info = contact_info;
                info.institution = institution;
                info.major = major;
                info.introduction = introduction;
                info.grade = grade;

                int count = db.Updateable(info).ExecuteCommand();

 /*               var count = db.Updateable<User_Info>()
                    .SetColumns(it => it.gender == gender)//SetColumns是可以叠加的 写2个就2个字段赋值
                    .Where(it => it.user_id == ID)
                    .ExecuteCommand();

                List<User_Info> agrlist;
                agrlist = db.Queryable<User_Info>().Where(it => it.user_id == ID).ToList();
                User_Info info = agrlist[0];
                info.gender = gender;
                info.contact_info = contact_info;
                info.institution = institution;
                info.major = major;
                info.introduction = introduction;
                info.grade = grade;

                int count = db.Updateable(info)
                  .UpdateColumns(p => new { p.gender, p.contact_info, p.institution, p.grade, p.major, p.introduction })
                  .WhereColumns(it => new { it.user_id })
                  .ExecuteCommand();

                var count = db.Updateable<User_Info>()
                    .UpdateColumns(it => new User_Info() { gender = gender, grade = grade, contact_info = contact_info, institution = institution, major = major, introduction = introduction })
                     .Where(it => it.user_id == ID)
                     .ExecuteCommand();*/
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
        //加分
        [HttpPost]
        public bool UpdatePoint(string user_id)
        {
            //获取数据库连接
            User_Info user = new User_Info();
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接
            //搜索条件，感觉可以改成检索的方式
            user = db.Queryable<User_Info>().Where(it => it.user_id == user_id).First();
            user.point +=10;
            try
            {
                int result = db.Updateable<User_Info>(user).Where(it => it.user_id == user_id).ExecuteCommand();
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
    }
}
