using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using Microsoft.AspNetCore.Cors;
using SqlSugar;
using MeeTeam_Backend;

namespace Meeteam_Backend.Controllers
{
    [Route("/[Controller]/[action]")]
    [ApiController]
    [EnableCors("any")]
    public class FileController : ControllerBase
    {
        [HttpPost]
        public void PostFile([FromForm] IFormCollection formCollection)
        {
            FormFileCollection fileCollection = (FormFileCollection)formCollection.Files;
            string path = formCollection["path"];//存储路径
            string id = formCollection["id"];//表id
            string target = formCollection["target"];//目标表（project或user）
            //遍历文件列表，向磁盘指定路径存储文件
            foreach (IFormFile file in fileCollection)
            {
                StreamReader reader = new StreamReader(file.OpenReadStream());
                string content = reader.ReadToEnd();
                string name = file.FileName;
                path = @"D:\work_space\storage\" + path + @"\";
                string filename = path + name;
                if (!System.IO.File.Exists(filename))
                {
                    using (FileStream fs = System.IO.File.Create(filename))
                    {
                        // 复制文件
                        file.CopyTo(fs);
                        // 清空缓冲区数据
                        fs.Flush();
                    }
                }
                //将文件路径存入数据库
                dbORM dborm = new dbORM();
                SqlSugarClient db = dborm.getInstance();//获取数据库连接
                if (target == "user")
                {
                    //修改user表，即用户头像head_img
                    db.Updateable<User_Info>()
                        .SetColumns(it => new User_Info
                        {
                            head_img = filename
                        })
                        .Where(it => it.user_id == id)
                        .ExecuteCommand();
                }
                if (target == "project")
                {
                    //修改project表，即项目宣传图
                    var entity = new Project_Img();
                    entity.project_id = id;
                    entity.img_path = filename;
                    db.Storageable(entity).ExecuteCommand();
                }
            }
        }

        [HttpGet]
        public string OpenFile(string target, string id)
        {
            string filename = null;
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接
            if (target == "user")
            {
                //取用户头像
                var q = db.Queryable<User_Info>().First(it => it.user_id == id);
                filename = q.head_img;
                if (filename == null)
                    return null;
            }
            if (target == "project")
            {
                //修改project表，即项目宣传图
                var q = db.Queryable<Project_Img>().First(it => it.project_id == id);
                if (q == null)
                    return null;
                filename = q.img_path;
                if (filename == null)
                    return null;
            }
            FileStream fs = System.IO.File.OpenRead(filename);
            var ms = new MemoryStream();
            fs.CopyTo(ms);
            byte[] b = ms.ToArray();
            return Convert.ToBase64String(b);
        }
    }
    public class view_user
    {
        public string User_Infohead_img { get; set; }
    }
}
