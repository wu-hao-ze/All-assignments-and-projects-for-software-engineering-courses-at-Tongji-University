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
        const string mypath = @"C:\workspace\storage\";
        //const string mypath = @"D:\work_space\storage\";
        [HttpPost]
        public void PostFile([FromForm] IFormCollection formCollection)
        {
            FormFileCollection fileCollection = (FormFileCollection)formCollection.Files;
            string id = formCollection["id"];//表id
            string target = formCollection["target"];//目标表（project或user）
            //遍历文件列表，向磁盘指定路径存储文件
            foreach (IFormFile file in fileCollection)
            {
                StreamReader reader = new StreamReader(file.OpenReadStream());
                string content = reader.ReadToEnd();
                string name = file.FileName;
                string path = mypath;
                if (target == "user")
                    path += @"head_img\";
                else if (target == "project")
                    path += @"project_img\";
                else if (target == "temporary")
                    path += @"temporary\";
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
                if (target == "temporary")
                    return;
                //将文件路径存入数据库
                dbORM dborm = new dbORM();
                SqlSugarClient db = dborm.getInstance();//获取数据库连接
                if (target == "user")
                {
                    //修改user表，即用户头像head_img
                    db.Updateable<User_Info>()
                        .SetColumns(it => new User_Info
                        {
                            head_img = name
                        })
                        .Where(it => it.user_id == id)
                        .ExecuteCommand();
                }
                if (target == "project")
                {
                    //修改project表，即项目宣传图
                    var entity = new Project_Img();
                    entity.project_id = id;
                    entity.img_path = name;
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
                filename = mypath+@"head_img\"+q.head_img;
                if (filename == null)
                    return null;
            }
            if (target == "project")
            {
                //修改project表，即项目宣传图
                var q = db.Queryable<Project_Img>().First(it => it.project_id == id);
                if (q == null)
                    return null;
                filename = mypath + @"project_img\" + q.img_path;
                if (filename == null)
                    return null;
            }
            if (target == "temporary")
            {
                filename = mypath + @"temporary\" + id;
            }
            FileStream fs = System.IO.File.OpenRead(filename);
            var ms = new MemoryStream();
            fs.CopyTo(ms);
            byte[] b = ms.ToArray();
            return Convert.ToBase64String(b);
        }


        [HttpPost]
        public int CopyFile(string project_id,string filename)
        {
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接
            var entity = new Project_Img();
            entity.project_id = project_id;
            entity.img_path = filename;
            if (!System.IO.File.Exists(mypath + @"project_img\" + entity.img_path))
            {
                using (FileStream fs = System.IO.File.Create(mypath+@"project_img\"+entity.img_path))
                {
                    string src = mypath + @"temporary\" + filename;
                    FileStream file = System.IO.File.OpenRead(src);
                    // 复制文件
                    file.CopyTo(fs);
                    // 清空缓冲区数据
                    fs.Flush();
                }
            }
            return db.Storageable(entity).ExecuteCommand();
        }


        [HttpPost]
        public int CopyHead(string user_id, string filename)
        {
            dbORM dborm = new dbORM();
            SqlSugarClient db = dborm.getInstance();//获取数据库连接
            if (!System.IO.File.Exists(mypath + @"head_img\" + filename))
            {
                using (FileStream fs = System.IO.File.Create(mypath + @"head_img\" + filename))
                {
                    string src = mypath + @"temporary\" + filename;
                    FileStream file = System.IO.File.OpenRead(src);
                    // 复制文件
                    file.CopyTo(fs);
                    // 清空缓冲区数据
                    fs.Flush();
                }
            }
            return db.Updateable<User_Info>().SetColumns(it => new User_Info
            {
                head_img = filename
            }).Where(it => it.user_id == user_id).ExecuteCommand();
        }
    }
    public class view_user
    {
        public string User_Infohead_img { get; set; }
    }
}
