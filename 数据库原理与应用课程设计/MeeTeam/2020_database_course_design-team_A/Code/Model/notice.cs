using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    //对应数据库中的notice表
    public class Notice
    {
        public string notice_id { get; set; }

        public string notice_title { get; set; }

        public string notice_content { get; set; }

        public string admin_id { get; set; }

        public int operate_type { get; set; }

        public DateTime date { get; set; }

        //对应着“公告发布之后才会被显示”的业务逻辑
        public bool visible { get; set; }
    }
}
