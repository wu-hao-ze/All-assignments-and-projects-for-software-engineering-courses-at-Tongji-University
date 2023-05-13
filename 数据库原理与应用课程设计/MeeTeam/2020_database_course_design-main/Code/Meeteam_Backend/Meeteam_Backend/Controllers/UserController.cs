using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MeeTeam_Back;
using Oracle.ManagedDataAccess.Client;
using Microsoft.AspNetCore.Cors;
using SqlSugar;
using System.Data;
using System.Collections;
using MeeTeam_Backend;

namespace Meeteam_Backend.Controllers
{
    [Route("/[Controller]/[action]")]
    [ApiController]
    [EnableCors("any")]
    public class UserController : ControllerBase
    {
        [HttpPost]
        //添加浏览历史
        public bool AddViewHistory(string user_id, string project_id)
        {
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接

            try
            {
                Browsehistory bh = new Browsehistory();
                bh.user_id = user_id;
                bh.project_id = project_id;
                bh.browse_time = DateTime.Now.ToString();

                int count = db.Storageable(bh).ExecuteCommand(); //自动判断是insert还是update
                if (count == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
                /*
                int count = db.Insertable(bh).ExecuteCommand();
                if (count == 1)
                    return true;
                else
                {
                    
                     * int count2 = db.Updateable<Browsehistory>()
                        .SetColumns(p=>new Browsehistory() { browse_time = bh.browse_time })
                        .Where(it => it.user_id == user_id && it.project_id == project_id)
                        .ExecuteCommand();
                     
                    int count2 = db.Updateable<Browsehistory>()
                        .SetColumns(it => new Browsehistory { browse_time = bh.browse_time })
                        .Where(it => it.project_id == project_id&&it.user_id==user_id)
                        .ExecuteCommand();

                    //int count2 = db.Updateable(bh).ExecuteCommand();
                    if (count2 == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                
                }
                    */
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [HttpPost]
        public bool BlockUser(string user_id)
        {

            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接
            try
            {
                int result = db.Updateable<User_Info>().SetColumns(p=>new User_Info() { account_status = "0" }).Where(it => it.user_id == user_id).ExecuteCommand();
                if (result == 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [HttpPost]
        public bool UnblockUser(string user_id)
        {
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接
            try
            {
                int result = db.Updateable<User_Info>().SetColumns(p => new User_Info() { account_status = "1" }).Where(it => it.user_id == user_id).ExecuteCommand();
                if (result == 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //查询全部用户，返回一个对象
        [HttpGet]
        public List<User_Info> SelectAllStatus()
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
    }
}
