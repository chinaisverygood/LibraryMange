using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using server;
using entity;
using Common;
using MyVideo;



namespace LibraryManagerPro
{
    public partial class FrmAddBook : Form
    {
        private BookServer bookService = new BookServer();
        private Video video = null;//定义摄像头操作的成员变量
        private List<Books> bookList = new List<Books>();//保存当前已经添加到数据库的图书对象
        public FrmAddBook()
        {
            InitializeComponent();
            //初始化图书分类下拉框
            this.cboBookCategory.DataSource = bookService.GetAllCategory();
            cboBookCategory.DisplayMember = "CategoryName";
            cboBookCategory.ValueMember = "CategoryId";
            cboBookCategory.SelectedIndex = -1;//默认不选中
            
            //初始化出版社下拉框
            this.cboPublisher.DataSource = bookService.GetAllPublishers();
            cboPublisher.DisplayMember = "PublisherName";
            cboPublisher.ValueMember = "PublisherId";
            cboPublisher.SelectedIndex = -1;//默认不选中

            //初始化时禁用控制摄像头的关闭和拍照按钮
            this.btnTake.Enabled = false;
            this.btnCloseVideo.Enabled = false;

            //禁止dgv表格自动生成列名
            this.dgvBookList.AutoGenerateColumns = false;

            
       
        }
        //启动摄像头
        private void btnStartVideo_Click(object sender, EventArgs e)
        {
            try
            {
                //创建摄像头操作类
                this.video = new Video(this.pbImage.Handle,this.pbImage.Left,this.pbImage.Top,this.pbImage.Width,(short)this.pbImage.Height);
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
        //关闭摄像头
        private void btnCloseVideo_Click(object sender, EventArgs e)
        {
            this.video.CloseVideo();
            this.btnCloseVideo.Enabled = false;
            this.btnStartVideo.Enabled = true;
            this.btnTake.Enabled = false;
         
        }
        //开始拍照
        private void btnTake_Click(object sender, EventArgs e)
        {
            this.pbCurrentImage.Image = video.CatchVideo();//拍照

        }
        //选择图片
        private void btnChoseImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            DialogResult result = openFile.ShowDialog();
            if (result==DialogResult.OK)//如果用户选择了图片
            {
                //可以通过文件的后缀名来判断可以存放那些图片

                this.pbCurrentImage.Image = Image.FromFile(openFile.FileName);
            }
        }
        //清除图片
        private void btnClear_Click(object sender, EventArgs e)
        {
            this.pbCurrentImage.Image = null;
        }
        
        //按回车键触发
        private void txtBarCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue==13)
            {
                txtBarCode_Leave(null,null);
            }

        }
        //判断图书条码是否已经存在（焦点离开触发）
        private void txtBarCode_Leave(object sender, EventArgs e)
        { 
                if (this.txtBarCode.Text.Trim().Length > 0)
                {
                    if (bookService.BarCodeIsExisted(this.txtBarCode.Text.Trim()))
                    {
                        MessageBox.Show("改条码已存在！", "条码提示");
                        this.txtBarCode.SelectAll();
                        this.txtBarCode.Focus();
                    }
                }
            }
        
        //确认添加
        private void btnAdd_Click(object sender, EventArgs e)
        {
            #region 数据验证
            if (this.txtBookName.Text.Trim()==null || "".Equals(this.txtBookName.Text.Trim()))
            {
                MessageBox.Show("图书名称不能为空","提示");
                return;
            }
           
            if (this.cboBookCategory.SelectedIndex==-1)
            {
                MessageBox.Show("请选择一种类别","提示");
                return;
            }
            if (this.cboPublisher.SelectedIndex==-1)
            {
                MessageBox.Show("请选择出版社","提示");
                return;
            } 
            
            if (this.txtAuthor.Text.Trim()==null || "".Equals(this.txtAuthor.Text.Trim()))
            {
                MessageBox.Show("作者不能为空","提示");
                return;
            } 
            if (this.txtBookPosition.Text.Trim()==null || "".Equals(this.txtBookPosition.Text.Trim()))
            {
                MessageBox.Show("图书位置不能为空","提示");
                return;
            }
           
             if (this.txtUnitPrice.Text.Trim()==null || "".Equals(this.txtUnitPrice.Text.Trim()))
            {
                MessageBox.Show("图书原价不能为空","提示");
                return;
            }
             if (this.txtBookCount.Text.Trim()==null || "".Equals(this.txtBookCount.Text.Trim()))
            {
                MessageBox.Show("图书总量不能为空","提示");
                return;
            }
            if (this.txtBarCode.Text.Trim()==null || "".Equals(this.txtBarCode.Text.Trim()))
            {
                MessageBox.Show("图书编码不能为空","提示");
                return;
            }
            if (!DataValidate.IsInteger(this.txtUnitPrice.Text.Trim()))
            {
                MessageBox.Show("原价必须是正整数");
                return;
            }
             if (!DataValidate.IsInteger(this.txtBookCount.Text.Trim()))
            {
                MessageBox.Show("图书总量必须是正整数");
                return;
            } 
           
            if (this.pbCurrentImage.Image==null)
            {
                MessageBox.Show("图片不能为空", "提示");
                return;
            }




            #endregion
            //封装图书对象
            Books book = new Books()
            {
                BookName = this.txtBookName.Text.Trim(),
                barCode = this.txtBarCode.Text.Trim(),
                bookCategory = Convert.ToInt32(this.cboBookCategory.SelectedValue),
                publisherId = Convert.ToInt32(this.cboPublisher.SelectedValue),
                publishDate = Convert.ToDateTime(dtpPublishDate.Text),
                unitPrice = Convert.ToDouble(txtUnitPrice.Text.Trim()),
                BookCount = Convert.ToInt32(txtBookCount.Text.Trim()),
                author = this. txtAuthor.Text.Trim(),
                Remainder = Convert.ToInt32( txtBookCount.Text.Trim()),
                bookPosition = txtBookPosition.Text.Trim(),
                bookImage= new Common.SerializeObjectToString().SerializeObject(this.pbCurrentImage.Image),
                PublisherName=this.cboPublisher.Text
            };
            //调用后台添加图书信息
            try
            {
                bookService.AddBook(book);
                //同步添加到表格中
                this.bookList.Add(book);
                this.dgvBookList.DataSource = null;
                this.dgvBookList.DataSource = bookList;

                //清空用户输入的数据，
                foreach (Control item in this.gbBook.Controls)
                {
                    if (item is TextBox)
                    {
                        item.Text = "";
                    }
                    else if (item is ComboBox)
                    {
                        ((ComboBox)item).SelectedIndex = -1;
                    }
                    
                }
                this.txtBookName.Focus();
                this.pbCurrentImage.Image = null;

            }
            catch (Exception)
            {

                throw;
            }
        }
        //退出窗口
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //添加行号
        private void dgvBookList_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Common.DataGridViewStyle.DgvRowPostPaint(this.dgvBookList,e);
        }
    }
}
