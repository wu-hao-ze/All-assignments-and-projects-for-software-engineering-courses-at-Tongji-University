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
    public class NoticeController : ControllerBase
    {

        //创建公告
        [HttpPost]
        public bool post(string not)
        {
            //判断管理员是否存在
            //创建公告
            Notice notice = JsonSerializer.Deserialize<Notice>(not);
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接
            notice.operate_time = DateTime.Now.ToString();
            if (db.Insertable(notice).ExecuteCommand() > 0)
                return true;
            else
                return false;
        }


        //获取单条公告
        [HttpGet]
        public string get_by_id(string id)
        {
            Notice notice = new Notice();
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接
            //搜索条件，感觉可以改成检索的方式
            notice = db.Queryable<Notice>().Where(it => it.notice_id==id).First();
            return JsonSerializer.Serialize(notice);
        }


        //获取全部公告
        [HttpGet]
        public string get_all()
        {
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接
            //搜索条件，感觉可以改成检索的方式
            var notice = db.Queryable<Notice>().ToList();
            return JsonSerializer.Serialize(notice);
        }


        //删除公告
        [HttpDelete]
        public bool delete_by_id(string id)
        {
            Notice notice = new Notice();
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接
            //搜索条件，感觉可以改成检索的方式
            if (db.Deleteable<Notice>().Where(it => it.notice_id == id).ExecuteCommand() > 0)
                return true;
            else
                return false;
        }

        [HttpPut]
        public bool put(string not)
        {
            Notice notice = JsonSerializer.Deserialize<Notice>(not);
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接
            notice.operate_time = DateTime.Now.ToString();
            if (db.Updateable(notice).ExecuteCommand() > 0)
                return true;
            else
                return false;
        }


        [HttpGet]
        public string notice_query(string s)
        {
            Notice_Query q = JsonSerializer.Deserialize<Notice_Query>(s);
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();
            var query = db.Queryable<Notice>();
            if (q.admin_id != null && q.admin_id != "")
                query.Where(it => it.admin_id.Contains(q.admin_id));
            if (q.notice_title != null && q.notice_title != "")
                query.Where(it => it.notice_title.Contains(q.notice_title));
            if (q.operate == "0")
                query.Where(it => it.operate_type == OPERATE_TYPE.SAVE);
            else if (q.operate == "1")
                query.Where(it => it.operate_type == OPERATE_TYPE.PUBLISH);
            var json = query.Clone().Distinct().ToJson();
            return json;
        }
    }
}
