using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using entity;
using server;
using Common;



namespace LibraryManagerPro
{
    public partial class FrmAdminLogin : Form
    {
        private static SysAdminService adminService = new SysAdminService();
      
        public FrmAdminLogin()
        {
            InitializeComponent();

        }
        //登陆
        private void btnLogin_Click(object sender, EventArgs e)
        {
            string adminId = txtAdminId.Text.Trim();
            string Pwd= txtLoginPwd.Text.Trim();

            if (Pwd==null || "".Equals(Pwd))
            {
                MessageBox.Show("密码不能为空");
                txtLoginPwd.Focus();
                return;
            }
            //封装对象
            SysAdmins admin = new SysAdmins()
            {
                AdminId = Convert.ToInt32(adminId),
                LoginPwd = Pwd
        };

            try
            {
                SysAdmins admins = adminService.adminLogin(admin);
                if (admins != null)
                {
                    if (admins.StatusId == 1)//账号正常
                    {
                        Program.admin = admins;//保存用户信息
                        this.DialogResult = DialogResult.OK;//设置窗体的返回值

                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("账号异常，请联系管理员", "登陆警告");
                    }
                }
                else
                {
                    MessageBox.Show("账号或密码错误", "登陆提示");

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("登陆出现异常："+ex.Message,"登陆提示");
                
            }

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
            Console.WriteLine("我退出了");
        }
        /// <summary>
        /// 鼠标离开触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtAdminId_MouseLeave(object sender, EventArgs e)
        {
            string adminId = txtAdminId.Text.Trim();
            if (adminId==null || "".Equals(adminId))
            {
               // errortext.Text = "账号不能为空";
                txtAdminId.Focus();//获得焦点
                return;
            }
            if (DataValidate.IsInteger(adminId))
            {
                //errortext.Text = "账号必须是4为整数";
                txtAdminId.Focus();
                return;

            }

            

        }

       
    }
}
