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
    public class FollowController : ControllerBase
    {
        [HttpGet]
        public string getRe(string MyID, string ID)
        {
            User_User my = new User_User();
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接
            var relation = db.Queryable<User_User>()
                .First(it => it.main_user_id == MyID && it.related_user_id == ID);
            if (relation == null)
                return null;
            return relation.relation;
        }
        

        [HttpGet]
        public int pullRe(string MyID, string ID, string relationship)
        {
            User_User my = new User_User();
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接
            my.main_user_id = MyID;
            my.related_user_id = ID;
            if (relationship == "关注")
            {//选择关注对方
                my.relation = "关注";
                return db.Storageable(my).ExecuteCommand();
            }
            else if (relationship == "无")
            {
                //删除数据库中本条消息
                return db.Deleteable<User_User>().Where(it => it.main_user_id == MyID && it.related_user_id == ID)
                    .ExecuteCommand();
            }
            else
                return 1;
        }
    }
}
