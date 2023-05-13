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
    [Route("api/[controller]")]
    [ApiController]
    public class MyInforController : ControllerBase
    {
        //查询个人用户的组队需求，返回一个对象
        [HttpGet]
        public List<User_Info> GetUserInfor(string user_id)
        {
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接
            try
            {
                List<User_Info> agrlist = db.Queryable<User_Info>().Where(it => it.user_id == user_id).ToList();
                return agrlist;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
