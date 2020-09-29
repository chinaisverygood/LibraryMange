using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using entity;


namespace LibraryManagerPro
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            FrmAdminLogin adminLogin = new FrmAdminLogin();
            DialogResult resutl = adminLogin.ShowDialog();

            if (resutl==DialogResult.OK)
            {
                Application.Run(new FrmMain());
            }
            else
            {
                Application.Exit();//退出整个应用程序
            }
        }

        public static SysAdmins admin=null;
        public static int Count=0;
    }
}
