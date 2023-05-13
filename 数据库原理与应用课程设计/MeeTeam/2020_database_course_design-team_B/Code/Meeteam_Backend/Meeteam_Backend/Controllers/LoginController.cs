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
    //路由规则http://根目录/[Controller]/[action]
    //返回的，开头2代表正确，最后4表示没有数据
    [Route("/[Controller]/[action]")]
    [ApiController]
    [EnableCors("any")]
    public class LoginController : ControllerBase

    {
        //查询全部用户，返回一个对象
        [HttpGet]
        public List<User_Info> SelectAlluser()
        {
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接
            try
            {
                List<User_Info> agrlist = db.Queryable<User_Info>().ToList();
                return agrlist;
            }
            catch(Exception ex)
            {
                return null;
            }
        }
        /*//得到用户名，返回用户id
        [HttpGet]
        public string GetUserid(string user_name)
        {
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接
            //查询
            var list = db.Queryable<User_Info>()
                .Select(it => it.user_id)
                .Where("user_name=@name", new { name = user_name })
                .ToList();
            return list[0].ToString();
        }
        //得到用户id，返回用户名
        [HttpGet]
        public string GetUsername(string user_id)
        {
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接
            //查询
            User_Info info = new User_Info();
            var list = db.Queryable<User_Info>()
               .Select(it => it.user_name)
               .Where("user_id=@id", new { id = user_id })
               .ToList();
            return list[0].ToString();
        }*/
        //查询此用户名是否存在
        [HttpGet]
        public bool IsUserUni(string user_id)
        {
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接
            try
            {
                List<User_Info> agrlist = db.Queryable<User_Info>().Where(it => it.user_id == user_id).ToList();
                int len = agrlist.Count();
                if (len == 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        //用户登陆
        [HttpGet]
        public bool login(string user_id, string password)
        {
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接
            try
            {
                var list = db.Queryable<User_Info>()
               .Select(it => it.password)
               .Where("user_id=@id", new { id = user_id })
               .ToList();
                if (password == list[0].ToString())
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        //管理员登陆
        [HttpGet]
        public bool adminlogin(string admin_id, string password)
        {
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接
            try
            {
                var list = db.Queryable<Administrator>()
                .Select(it => it.password)
                .Where("admin_id=@id", new { id = admin_id })
                .ToList();
                if (password == list[0])
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        //添加普通用户
        [HttpPost]
        public bool addnormal(string user_id, string password, string user_name, char gender, string contact_info, string institution, string major, string introduction)
        {
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接
            try
            {
                User_Info pos = new User_Info();
                pos.user_id = user_id;
                pos.password = password;
                pos.register_time = DateTime.Now.ToString("g"); //2009/10/30 20:40
                pos.user_name = user_name;
                pos.account_status = '1';
                pos.gender = gender;
                pos.contact_info = contact_info;
                pos.institution = institution;
                pos.major = major;
                pos.grade = "0";
                pos.introduction = introduction;
                pos.point = 0;
                int count = db.Insertable(pos).ExecuteCommand();
                if (count == 1)
                    return true;
                else
                    return false;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
        //添加管理员
        [HttpPost]
        public bool addadmin(string admin_id, string password, string user_name, char gender, string contact_info)
        {
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接
            try
            {
                Administrator pos = new Administrator();
                pos.admin_id = admin_id;
                pos.password = password;
                pos.register_time = DateTime.Now.ToString("g"); //2009/10/30 20:40
                pos.user_name = user_name;
                pos.account_status = '1';
                pos.gender = gender;
                pos.contact_info = contact_info;
                int count = db.Insertable(pos).ExecuteCommand();
                if (count == 1)
                    return true;
                else
                    return false;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
        //删除用户
        [HttpDelete]
        public bool deleteUser(string user_id)
        {
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接                                    //删除
            try
            {
                int count = db.Deleteable<User_Info>().In(user_id).ExecuteCommand();
                if (count == 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        //删除用户
        [HttpDelete]
        public bool deleteAdmin(string admin_id)
        {
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接                                    //删除
            try
            {
                int count = db.Deleteable<Administrator>().In(admin_id).ExecuteCommand();
                if (count == 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
