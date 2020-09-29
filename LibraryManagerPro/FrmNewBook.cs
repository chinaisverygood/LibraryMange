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


namespace LibraryManagerPro
{
    public partial class FrmNewBook : Form
    {
        private BookServer bookService = new BookServer();
        private List<Books> bookList = new List<Books>();

        public FrmNewBook()
        {
            InitializeComponent();
            this.txtAddCount.Enabled = false;
            this.dgvBookList.AutoGenerateColumns = false;


        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 增加图书总量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            //数据验证
            if (this.txtAddCount.Text.Trim().Length==0)
            {
                MessageBox.Show("请输入新增图书总数！","提示信息");
                this.txtAddCount.Focus();
                return;
            }
            if (!Common.DataValidate.IsInteger(this.txtAddCount.Text.Trim()))
            {
                MessageBox.Show("新增图书必须是一个正整数！","提示信息");
                return;
            }
            //提交给数据库
            try
            {
               bool result=  bookService.AddBookCount(this.txtBarCode.Text.Trim(),Convert.ToInt32(this.txtAddCount.Text.Trim()));
                if (result)
                {
                    //在dgv中显示当前图书数量和其他信息
                    Books book = (from b in bookList where b.barCode == this.txtBarCode.Text.Trim() select b).First<Books>();
                    book.BookCount = book.BookCount + Convert.ToInt32(this.txtAddCount.Text.Trim());
                    this.dgvBookList.Refresh();//刷新表

                    //清空上一条数据
                    this.gbinfo.Text = "";
                    this.pbImage.Image = null;
                    this.txtAddCount.Text = "";
                    this.txtAddCount.Enabled = false;
                    this.txtBarCode.Focus();
                   
                }
            }
            catch (Exception)
            {

                throw;
            }


        }
        //键盘事件,根据图书条码查询图书信息
        private void txtBarCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                Books book = bookService.GetBookByBarCode(this.txtBarCode.Text.Trim());
                if (book != null)
                {
                    //显示图书信息
                    this.lblBookName.Text = book.BookName;
                    this.lblBookCount.Text = Convert.ToString(book.BookCount);
                    this.lblBookId.Text = Convert.ToString(book.bookId);
                    this.lblBookPosition.Text = book.bookPosition;
                    this.lblCategory.Text = book.CategoryName;
                    this.pbImage.Image = book.bookImage.Length == 0 ? null : (Image)new Common.SerializeObjectToString().DeserializeObject(book.bookImage);

                    //开启新增图书数量文本
                    this.txtAddCount.Enabled = true;
                    this.txtAddCount.Focus();

                    //同步显示在列表中(判断当前列表中是否已经存在该图书对象)
                    int count = (from b in bookList where b.bookId == book.bookId select b).Count();
                    if (count == 0)
                    {
                        this.bookList.Add(book);
                        this.dgvBookList.DataSource = null;
                        this.dgvBookList.DataSource = bookList;
                    }

                }
                else
                {
                    MessageBox.Show("图书不存在！！", "查询提示");
                    this.txtAddCount.Focus();
                    this.txtAddCount.SelectAll();
                }
            }
        }
    }
}
