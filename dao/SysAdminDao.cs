using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using entity;
using DButil;
using System.Data.SqlClient;

namespace dao
{
  public  class SysAdminDao
    {
        /// <summary>
        /// 根据账号密码从数据库获取管理员信息
        /// </summary>
        /// <param name="admins">管理员对象实体类用来存放管理员信息</param>
        /// <returns>返回管理员对象信息</returns>
        public SysAdmins adminLogin(SysAdmins admins)
        {
            //定义sql语句
            string sql = "select AdminName,StatusId,RoleId from SysAdmins where AdminId=@AdminId and loginPwd=@Pwd;";
            //封装参数
            SqlParameter[] param = new SqlParameter[]
            {
                new SqlParameter("@AdminId",admins.AdminId),
                new SqlParameter("@Pwd",admins.LoginPwd),
            };
            //执行查询
            SqlDataReader reader = SqlDB.Query(sql,param);
            //处理查询结果
            if (reader.Read())
            {
                admins.AdminName = reader["AdminName"].ToString();
                admins.StatusId = Convert.ToInt32(reader["StatusId"]);
                admins.RoleId = Convert.ToInt32(reader["RoleId"]);
            }
            else
            {
                admins = null;//如果账号密码不正确，则清空当前对象
            }
            reader.Close();

            return admins;

        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="AdminId">管理员ID</param>
        /// <param name="NewPwd">新密码</param>
        /// <returns></returns>
        public int ModifyPwd(int AdminId,string NewPwd)
        {
            string sql = "update SysAdmins set LoginPwd=@LoginPwd where AdminId=@AdminId";
            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@AdminId",AdminId),
                new SqlParameter("LoginPwd",NewPwd)
            
            };
            return SqlDB.Update(sql,param);
        }

    }
}
