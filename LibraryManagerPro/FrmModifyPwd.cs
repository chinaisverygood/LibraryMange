using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using server;
using Common;

namespace LibraryManagerPro
{
    public partial class FrmModifyPwd : Form
    {
        private SysAdminService adminService = new SysAdminService();
        public FrmModifyPwd()
        {
            InitializeComponent();
            //main.FrmMain_FormClosing -= new FormClosingEventHandler(main.FrmMain_FormClosing(null,null));
           
        }

        private void btnModify_Click(object sender, EventArgs e)
        {
            #region 验证

            if (txtOldPwd.Text.Trim().Length == 0)
            {
                MessageBox.Show("原密码不能为空", "密码提示");
                return;
            }
            if (txtNewPwd.Text.Trim().Length==0)
            {
                MessageBox.Show("新密码不能为空","密码提示");
                return;
            }
            if (txtNewPwdConfirm.Text.Trim().Length==0)
            {
                MessageBox.Show("确认密码不能为空","密码提示");
                return;
            }

            if (txtOldPwd.Text.Trim()!=Program.admin.LoginPwd)
            {
                MessageBox.Show("原密码不正确","修改密码提示");
                return;
            }

            if (txtNewPwd.Text.Trim()==txtOldPwd.Text.Trim())
            {
                MessageBox.Show("新密码与旧密码一致，不可更改","修改密码提示");
                return;
            }
            if (txtNewPwd.Text.Trim() != txtNewPwdConfirm.Text.Trim())
            {
                MessageBox.Show("新密码与确认密码不一致","修改密码提示");
                return;

            }
            #endregion

            //调用底层
            bool result = adminService.ModifyPwd(Program.admin.AdminId, this.txtNewPwd.Text.Trim());
              if (result)
                {
                MessageBox.Show("修改成功,请重新登录", "修改密码提示");
                System.Windows.Forms.Application.Exit();//退出整个应用程序
                }  
            }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
