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
    [Route("/[Controller]/[action]")]
    [ApiController]
    [EnableCors("any")]

    public class HistoryController : ControllerBase
    {
        //查询某一个人的浏览历史
        [HttpGet]
        public List<Browsehistory> Gethistory(string user_id)
        {
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接
            try
            {
                List<Browsehistory> agrlist = db.Queryable<Browsehistory>().Where(it=>it.user_id==user_id).ToList();
                return agrlist;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}