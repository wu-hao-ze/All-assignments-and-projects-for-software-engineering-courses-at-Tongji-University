using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeeTeam_Back
{
    public class UserManager
    {
        private UserDB userdb1 = new UserDB();
        public bool ifadd(UserInfo userinfo, out string messageStr)
        {
            messageStr = "";
            bool ifsuccess = false; //state begin with "false"
            if (userinfo.UserID.Trim().Length != 0)
            {
                if (!userdb1.IsEqual(userinfo))
                {
                    //no same name already
                    userdb1.AddUser(userinfo);
                    ifsuccess = true;
                }
                else
                {
                    messageStr = "already have the value";
                }
            }
            else
            {
                messageStr = "cannot be blank";
            }

            return ifsuccess;
        }
    }
}
