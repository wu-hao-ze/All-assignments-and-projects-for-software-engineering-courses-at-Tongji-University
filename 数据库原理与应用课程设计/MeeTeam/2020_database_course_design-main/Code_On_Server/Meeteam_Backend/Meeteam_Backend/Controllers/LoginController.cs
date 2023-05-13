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
using System.Security;
using System.Security.Cryptography;
using System.Net.Mail;
using System.Net;

namespace Meeteam_Backend.Controllers
{
    //路由规则http://根目录/[Controller]/[action]
    //返回的，开头2代表正确，最后4表示没有数据
    [Route("/[Controller]/[action]")]
    [ApiController]
    [EnableCors("any")]
    public class LoginController : ControllerBase

    {
        //16位的MD5加密
        public static string MD5Encrypt16(string password)
        {
            var md5 = new MD5CryptoServiceProvider();
            string t2 = BitConverter.ToString(md5.ComputeHash(System.Text.Encoding.Default.GetBytes(password)), 4, 8);
            t2 = t2.Replace("-", "");
            return t2;
        }

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
            catch (Exception ex)
            {
                return null;
            }
        }


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
                if (MD5Encrypt16(password) == list[0].ToString())//将新的密码加密比对
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
                if (MD5Encrypt16(password) == list[0])
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
        public bool addnormal(string user_id, string password, string user_name, string gender, string contact_info, string institution, string major, string introduction)
        {
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接
            try
            {
                User_Info pos = new User_Info();
                pos.user_id = user_id;
                pos.password = MD5Encrypt16(password);
                pos.register_time = DateTime.Now.ToString("g"); //2009/10/30 20:40
                pos.user_name = user_name;
                pos.account_status ="1";
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
            catch (Exception ex)
            {
                return false;
            }
        }


        //添加管理员
        [HttpPost]
        public bool addadmin(string admin_id, string password, string user_name, string gender, string contact_info)
        {
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接
            try
            {
                Administrator pos = new Administrator();
                pos.admin_id = admin_id;
                pos.password = MD5Encrypt16(password);
                pos.register_time = DateTime.Now.ToString("g"); //2009/10/30 20:40
                pos.user_name = user_name;
                pos.account_status = "1";
                pos.gender = gender;
                pos.contact_info = contact_info;
                int count = db.Insertable(pos).ExecuteCommand();
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
        public bool deleteUser(string user_id)
        {
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接
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


        //删除管理员
        [HttpDelete]
        public bool deleteAdmin(string admin_id)
        {
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接
            //删除操作
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


        //发送邮箱
        [HttpGet]
        public string SendMailUseZj(string aa)
        {
            MailMessage message = new MailMessage();

            //设置发件人,发件人需要与设置的邮件发送服务器的邮箱一致
            MailAddress fromAddr = new MailAddress("2039129107@qq.com");
            message.From = fromAddr;
            //设置收件人,可添加多个,添加方法与下面的一样
            message.To.Add(aa);
            /*            //设置抄送人
                        message.CC.Add("2965372061@qq.com");*/
            //设置邮件标题
            message.Subject = "密码修改通知";
            //设置邮件内容
            Random r = new Random();
            string ts = r.Next(1000, 10000).ToString();
            message.Body = "您好，需要修改密码，验证码为" + ts;

            //设置邮件发送服务器,服务器根据你使用的邮箱而不同,可以到相应的 邮箱管理后台查看
            SmtpClient client = new SmtpClient("smtp.qq.com", 25);
            //设置发送人的邮箱账号和授权码
            client.Credentials = new NetworkCredential("2039129107@qq.com", "bbdsekrrtjqqbggf");

            //启用ssl,也就是安全发送
            client.EnableSsl = true;
            //发送邮件
            client.Send(message);
            return ts;//返回验证码
        }


        //更新密码
        [HttpPost]
        public bool UpdateUser(string user_id, string password)
        {
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接
            try
            {
                var count = db.Updateable<User_Info>()
                    .SetColumns(it => it.password == MD5Encrypt16(password))//SetColumns是可以叠加的 写2个就2个字段赋值
                    .Where(it => it.user_id == user_id)
                    .ExecuteCommand();
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


        //验证用户名和邮箱是否正确
        [HttpGet]
        public bool Iftruemail(string user_id, string contact_info)
        {
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接
            try
            {
                var list = db.Queryable<User_Info>()
               .Select(it => it.contact_info)
               .Where("user_id=@id", new { id = user_id })
               .ToList();
                if (contact_info == list[0].ToString())//将新的密码加密比对
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
