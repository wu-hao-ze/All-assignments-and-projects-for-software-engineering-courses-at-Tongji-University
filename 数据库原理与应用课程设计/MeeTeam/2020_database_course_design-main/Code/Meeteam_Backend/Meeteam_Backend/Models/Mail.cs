using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Meeteam_Backend
{
    public class Mail
    {
            /// <summary>
            /// 发送者
            /// </summary>
            public string mailFrom { get; set; }

            /// <summary>
            /// 收件人
            /// </summary>
            public string[] mailToArray { get; set; }

            /// <summary>
            /// 抄送
            /// </summary>
            public string[] mailCcArray { get; set; }

            /// <summary>
            /// 标题
            /// </summary>
            public string mailSubject { get; set; }

            /// <summary>
            /// 正文
            /// </summary>
            public string mailBody { get; set; }

            /// <summary>
            /// 发件人密码
            /// </summary>
            public string mailPwd { get; set; }

            /// <summary>
            /// SMTP邮件服务器
            /// </summary>
            public string host { get; set; }

            /// <summary>
            /// 正文是否是html格式
            /// </summary>
            public bool isbodyHtml { get; set; }

            /// <summary>
            /// 附件
            /// </summary>
            public string[] attachmentsPath { get; set; }
        }
}
