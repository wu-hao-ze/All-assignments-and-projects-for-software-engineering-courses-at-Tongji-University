using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using Model;

namespace DAL
{
    //在MVC Web框架中，路由模块会对接收到的请求进行匹配并转交由对应的控制器（Controller）进行处理。
    //控制器的作用就是处理接收到的请求，解析用户输入并执行对应程序理逻辑，然后返回对应的输出
    [ApiController]
    [Route("api/[controller]")]
    public class noticeController : ControllerBase
    {
        public SqlSugarClient db;
        public noticeController(SqlSugarClient _db)
        {
            db = _db;
        }
        [HttpPut]
        //todo : 从前端得到notice的信息之后构造Notice Class实体
        public bool Add(Notice n)
        {
            int effectedLens = db.Insertable<Notice>(n).ExecuteCommand();
            return (effectedLens > 0);
        }
        [HttpPut]
        //todo : 从前端得到主键notice id
        public bool Delete(string notice_id)
        {
            return db.Deleteable<Notice>().In(notice_id).ExecuteCommandHasChange();
        }
        [HttpPut]
        //todo : 从前端得到主键notice id
        public bool Publish(string notice_id)
        {
            Notice n = db.Queryable<Notice>().First(it => it.notice_id == notice_id);
            if(n == null)
            {
                return false;
            }
            else
            {
                n.visible = true;
                int effectedLens = db.Updateable(n).ExecuteCommand();
                return (effectedLens > 0);
            }
        }
    }
}
