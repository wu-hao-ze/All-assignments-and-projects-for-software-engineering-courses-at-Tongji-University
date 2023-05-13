using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;

namespace MeeTeam_Back.Controllers
{
    //路由规则http://根目录/[Controller]/[action]
    //返回的，开头2代表正确，最后4表示没有数据
    [Route("/[Controller]/[action]")]
    [ApiController]
    [EnableCors("any")]
    public class LoginController : ControllerBase
    {
        //get/post/put/delete
        //登陆
        [HttpGet]
        public bool login(string ID, string Word)
        {
            UserDB userlogin = new UserDB();
            UserInfo info = new UserInfo();
            info.PassWord = Word;
            info.UserID = ID;
            bool res = false;//是否登陆成功
            /*bool res1 = false;//是否连接数据库
            res1 = userlogin.connectOracle();
            if (res1 == true)
            {
                res = userlogin.login(info);
            }*/
            if (Word == "admin" && ID == "admin")
                res = true;
            return res;
        }
    }
}
