using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace LibraryManagerPro
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
            //显示当前登陆账号名称
            this.tssl_AdminName.Text = Program.admin.AdminName;
            
        }
        //新增图书
        private void btnAddBook_Click(object sender, EventArgs e)
        {
            FrmAddBook addBook = new FrmAddBook();
            EmbedForm(addBook);
            this.lblOperationName.Text = "新增图书";
        }
        //图书管理
        private void btnBookManage_Click(object sender, EventArgs e)
        {
           FrmBookManage bookManage = new FrmBookManage();
           EmbedForm(bookManage);
           this.lblOperationName.Text = "图示管理";
        }
        //图书出借
        private void btnBorrowBook_Click(object sender, EventArgs e)
        {
          FrmBorrowBook borrowBook = new FrmBorrowBook();
            EmbedForm(borrowBook);
            this.lblOperationName.Text = "图书出借";
            
        }
        //新书上架
        private void btnBookNew_Click(object sender, EventArgs e)
        {
            FrmNewBook newBook = new FrmNewBook();
            EmbedForm(newBook);
            this.lblOperationName.Text = "新书上架";
        }
        //图书归还
        private void btnReturnBook_Click(object sender, EventArgs e)
        {
            FrmReturnBook returnBook = new FrmReturnBook();
            EmbedForm(returnBook);
            this.lblOperationName.Text = "图书归还";
        }
        //会员管理
        private void btnReaderManager_Click(object sender, EventArgs e)
        {
            FrmReaderManger readerManage = new FrmReaderManger();
            EmbedForm(readerManage);
            this.lblOperationName.Text = "会员管理";

        }
        //修改密码
        private void btnModifyPwd_Click(object sender, EventArgs e)
        {
            FrmModifyPwd modifyPwd = new FrmModifyPwd();
            modifyPwd.ShowDialog();
        }
       
        private void EmbedForm(Form form)
        {
            //判断容器中是否含有子窗体（通过容器中的属性：Controls，它主要用来添加控件）
            foreach (Control item in this.spContainer.Panel2.Controls)
            {
                if (item is Form)
                {
                    ((Form)item).Close();
                }
            }
            //嵌入子窗体
            form.TopLevel = false;//将子窗体设置为顶置
            form.FormBorderStyle = FormBorderStyle.None;//去掉子窗体的边框
            form.Parent = this.spContainer.Panel2;//指定子窗体显示的容器
            form.Dock = DockStyle.Fill;//随着容器大小自动调整窗体大小
            form.Show();

        } 
        //退出系统
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
            
            
        }
        //窗体关闭之前的事件
        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("确认退出系统吗？","退出询问",MessageBoxButtons.OKCancel,MessageBoxIcon.Question);
            if (result==DialogResult.Cancel)
            {
                e.Cancel = true;//取消窗体关闭操作
            }
            

        }
    }
}
