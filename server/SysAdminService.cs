using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dao;
using entity;


namespace server
{
   public class SysAdminService
    {
        //创建数据访问对象
        private SysAdminDao adminDao = new SysAdminDao();
        public SysAdmins adminLogin(SysAdmins admins)
        {
            return adminDao.adminLogin(admins);
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="AdminId">管理员ID</param>
        /// <param name="NewPwd">行密码</param>
        /// <returns></returns>
        public bool ModifyPwd(int AdminId, string NewPwd)
        {
         int  result= adminDao.ModifyPwd(AdminId,NewPwd);
            if (result>0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
