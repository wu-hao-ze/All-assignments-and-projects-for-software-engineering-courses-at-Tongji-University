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

    public class GroupRequirementController : ControllerBase
    {
        //添加组队需求
        [HttpPost]
        public bool AddGroupRequirement(string purpose, string team_type, string team_limit, string details, string require_status,  string project_id, string region, string team_type_details)
        {
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接
            try
            {
                Grouping_Requirement pos = new Grouping_Requirement();
                pos.release_time = DateTime.Now.ToString("g"); //2009/10/30 20:40
                pos.purpose = purpose;
                pos.team_type = team_type;
                pos.team_limit = team_limit;
                pos.details = details;
                pos.require_status = require_status;
                pos.project_id = project_id;
                pos.region = region;
                pos.team_type_details = team_type_details;
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


        //查询全部组队需求，返回一个对象
        [HttpGet]
        public List<Grouping_Requirement> SelectAllGroupRequirement()
        {
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接
            try
            {
                List<Grouping_Requirement> agrlist = db.Queryable<Grouping_Requirement>().ToList();
                return agrlist;
            }
            catch (Exception ex)  
            {
                return null;
            }
        }


        //查询id的需求
        [HttpGet]
        public string get_by_id(string id)
        {
            Grouping_Requirement re = new Grouping_Requirement();
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接
            re = db.Queryable<Grouping_Requirement>().Where(it => it.project_id == id).First();
            return JsonSerializer.Serialize(re);
        }


        //编辑更新需求
        [HttpPost]
        public bool Changerequirelist(string purpose, string team_type, string team_limit, string details, string require_status, string project_id, string region, string team_type_details)
        {
            //获取数据库连接
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();

            Grouping_Requirement pos = new Grouping_Requirement();
            pos.purpose = purpose;
            pos.team_type = team_type;
            pos.team_limit = team_limit;
            pos.details = details;
            pos.require_status = require_status;
            pos.project_id = project_id;
            pos.region = region;
            pos.team_type_details = team_type_details;
            try
            {
                int result = db.Updateable<Grouping_Requirement>(pos).Where(it => it.project_id == project_id).ExecuteCommand();
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


        //删除组队需求
        [HttpDelete]
        public bool deleteGroupRequirement(string require_id)
        {
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接
            //删除操作
            try
            {
                int count = db.Deleteable<Grouping_Requirement>().In(require_id).ExecuteCommand();
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

