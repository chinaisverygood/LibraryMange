using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using entity;
using Common;
using server;
using MyVideo;


namespace LibraryManagerPro
{
    public partial class FrmBookManage : Form
    {
        private static BookServer bookService = new BookServer();
        private List<Books> bookList = new List<Books>();//保存查询结果
        private Video video = null;//定义摄像头操作的成员变量
        /// <summary>
        /// 初始化操作
        /// </summary>
        public FrmBookManage()
        {
            InitializeComponent();
            //初始化图书分类(上)
            cboCategory.DataSource = bookService.GetAllCategory();
            cboCategory.DisplayMember = "CategoryName";
            cboCategory.ValueMember = "CategoryId";
            cboCategory.SelectedIndex = -1;


            //初始化图书分类下拉框（下）
            this.cbo_BookCategory.DataSource = bookService.GetAllCategory();
            cbo_BookCategory.DisplayMember = "CategoryName";
            cbo_BookCategory.ValueMember = "CategoryId";

            //初始化出版社下拉框
            this.cbo_Publisher.DataSource = bookService.GetAllPublishers();
            cbo_Publisher.DisplayMember = "PublisherName";
            cbo_Publisher.ValueMember = "PublisherId";
           

            //初始化时禁用控制摄像头的关闭和拍照按钮
            this.btnTake.Enabled = false;
            this.btnCloseVideo.Enabled = false;


            //禁止表格自动生成列
            this.dgvBookList.AutoGenerateColumns = false;
            

        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        /// <summary>
        ///组合查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, EventArgs e)
        {
            //首先断开选择改变事件（防止有些情况异常）
            this.dgvBookList.SelectionChanged -= new EventHandler(this.dgvBookList_SelectionChanged);
            
            //判断用户是否输入查询条件
            if (this.cboCategory.SelectedValue.ToString()=="-1" && this.txtBarCode.Text.Trim().Length==0 && this.txtBookName.Text.Trim().Length==0)
            {
                MessageBox.Show("请至少选择一个查询条件！","查询提示");
                return;
            }
            else
            {
                //根据条件查询
                bookList = bookService.GetBooks(Convert.ToInt32(this.cboCategory.SelectedValue),this.txtBarCode.Text.Trim(),this.txtBookName.Text.Trim());
                //在列表中显示查询结果
                this.dgvBookList.DataSource = bookList;
                //根据查询结果禁用删除按钮
                if (bookList.Count==0)
                {
                    this.btnDel.Enabled = false;
                    this.btnSave.Enabled = false;
                }
                else
                {
                    this.btnDel.Enabled = true;
                    this.btnSave.Enabled = true;
                }
            }
            //开启选择改变事件（防止有些情况异常）
            this.dgvBookList.SelectionChanged += new EventHandler(this.dgvBookList_SelectionChanged);
            dgvBookList_SelectionChanged(null,null);
        }
        /// <summary>
        /// 表格发生选中改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvBookList_SelectionChanged(object sender, EventArgs e)
        {
            if (this.dgvBookList.Rows.Count==0)
            {
                return;
            }
            //找到要修改的图书对象
            string barCode = this.dgvBookList.CurrentRow.Cells["BarCode"].Value.ToString();
            //通过上面得到的BarCode从集合中查找对应的对象数据
            Books book = (from b in bookList where b.barCode.Equals(barCode) select b).First<Books>();
            //显示当前图书对象
            this.lbl_BarCode.Text = book.barCode;
            this.txt_BookName.Text = book.BookName;
            this.lbl_BookCount.Text = book.BookCount.ToString();
            this.txt_Author.Text = book.author;
            this.txt_BookPosition.Text = book.bookPosition;
            this.txt_UnitPrice.Text = book.unitPrice.ToString();
            this.cbo_BookCategory.SelectedValue = book.bookCategory;
            this.cbo_Publisher.SelectedValue = book.publisherId;
            this.lbl_BookId.Text = book.bookId.ToString();
            if (book.bookImage.Length==0)
            {
                this.pbCurrentImage.Image = null;
                return;
            }
            this.pbCurrentImage.Image =(Image) new Common.SerializeObjectToString().DeserializeObject(book.bookImage);
        }
        /// <summary>
        /// 启用摄像头
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStartVideo_Click(object sender, EventArgs e)
        {
            try
            {
                //创建摄像头操作类
                this.video = new Video(this.pbImage.Handle, this.pbImage.Left, this.pbImage.Top, this.pbImage.Width, (short)this.pbImage.Height);
                //打开摄像头
                video.OpenVideo();
                //同时禁用开启按钮
                this.btnStartVideo.Enabled = false;
                this.btnCloseVideo.Enabled = true;
                this.btnTake.Enabled = true;
            }
            catch (Exception)
            {
                MessageBox.Show("");
                throw;
            }

        }
        /// <summary>
        /// 关闭摄像头
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCloseVideo_Click(object sender, EventArgs e)
        {
            this.video.CloseVideo();
            this.btnCloseVideo.Enabled = false;
            this.btnStartVideo.Enabled = true;
            this.btnTake.Enabled = false;
        }
        /// <summary>
        /// 开始拍照
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTake_Click(object sender, EventArgs e)
        {
            this.pbCurrentImage.Image = video.CatchVideo();
        }

        //选择图片
        private void btnChoseImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            DialogResult result = openFile.ShowDialog();
            if (result == DialogResult.OK)//如果用户选择了图片
            {
                //可以通过文件的后缀名来判断可以存放那些图片
                this.pbCurrentImage.Image = Image.FromFile(openFile.FileName);
            }
        }
        /// <summary>
        /// 保存修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            //验证

            //封装对象
            Books book = new Books()
            {
                bookId = Convert.ToInt32(this.lbl_BookId.Text.Trim()),
                BookName = this.txt_BookName.Text.Trim(),
                bookCategory = Convert.ToInt32(this.cbo_BookCategory.SelectedValue),
                publisherId = Convert.ToInt32(this.cbo_Publisher.SelectedValue),
                publishDate = Convert.ToDateTime(dtp_PublishDate.Text),
                unitPrice = Convert.ToDouble(txt_UnitPrice.Text.Trim()),
                author = this.txt_Author.Text.Trim(),
                bookPosition = txt_BookPosition.Text.Trim(),
                bookImage = new Common.SerializeObjectToString().SerializeObject(this.pbCurrentImage.Image),
            };

            try
            {
            //调用底层
            int resutl= bookService.EditBook(book);

            if (resutl>0)
            {
                MessageBox.Show("修改成功","添加提示");
                //表格同步更新
                Books EditBook = (from b in bookList where b.bookId.Equals(Convert.ToInt32(this.lbl_BookId.Text)) select b).First<Books>();
                EditBook.BookName = book.BookName;
                EditBook.bookCategory = book.bookCategory;
                EditBook.publisherId = book.publisherId;
                EditBook.publishDate = book.publishDate;
                EditBook.unitPrice = book.unitPrice;
                EditBook.author = book.author;
                EditBook.bookPosition = book.bookPosition;
                EditBook.bookImage = book.bookImage;
                //刷新dgv
                this.dgvBookList.Refresh();
                }
                else
                {
                    MessageBox.Show("修改失败","添加提示");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"错误提示");
                throw;
            }
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDel_Click(object sender, EventArgs e)
        {
            //正式删除前确认（可以根据需要实现删除回收站：给删除的数据添加删除标记）
            DialogResult resutl = MessageBox.Show("确认要删除["+this.txt_BookName.Text.Trim()+"]这本书？","删除询问", MessageBoxButtons.OKCancel,MessageBoxIcon.Question);
            //关闭selectChanged（后台删除了前台没有变，会出发这个事件，导致异常）
            this.dgvBookList.SelectionChanged -= new EventHandler(this.dgvBookList_SelectionChanged);
            if (resutl == DialogResult.Cancel)
            {
                return;
            }
            try
            {
                if (bookService.DeleteBook(this.lbl_BookId.Text)==1)
                {
                    //同步更新显示
                    Books deleteBook = (from b in this.bookList where b.bookId.Equals(Convert.ToInt32(this.lbl_BookId.Text)) select b).First<Books>();
                    this.bookList.Remove(deleteBook);
                    this.dgvBookList.DataSource = null;
                    this.dgvBookList.DataSource = this.bookList;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"删除提示");
                throw;
            }
            //继续开启
            this.dgvBookList.SelectionChanged += new EventHandler(this.dgvBookList_SelectionChanged);
            this.dgvBookList_SelectionChanged(null,null);
        }
    }
}
