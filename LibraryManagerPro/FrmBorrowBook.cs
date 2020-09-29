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
using MyVideo;



namespace LibraryManagerPro
{
    public partial class FrmBorrowBook : Form
    {
        private ReaderService readerService = new ReaderService();

        private BorrowService borrowService = new BorrowService();

        private BookServer bookService = new BookServer();

        Readers objReader = null;//保存当前读者对象

        private List<BorrowDetail> detailList = new List<BorrowDetail>();//保存当前借书明细的集合

        public FrmBorrowBook()
        {
            InitializeComponent();
            //初始化按钮
            this.btnSave.Enabled = false;
            this.btnDel.Enabled = false;
            this.txtBarCode.Enabled = false;

            this.dgvBookList.AutoGenerateColumns = false;//禁止自动生成列

        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //数据验证

            //封装对象【主表】
            BorrowInfo main = new BorrowInfo()
            {
                ReaderId = this.objReader.ReaderId,
                BorrowId = DateTime.Now.ToString("yyyyMMddhhmmssms"),
                AdminName_B = Program.admin.AdminName,

            };
            //封装明细对象（将当前明细对象未封装的属性）
            for (int i = 0; i < detailList.Count; i++)
            {
                detailList[i].BorrowId = main.BorrowId;
                detailList[i].Expire = DateTime.Now.AddDays(objReader.AllowDay);
                detailList[i].NonReturnCount = detailList[i].BorrowCount;
            }

            try
            {
                
             borrowService.BorrowBook(main,detailList);//将封装的对象传递对象给业务逻辑完成数据保存
                //各种数据复位
                this.txtBarCode.Clear();
                this.txtBarCode.Enabled = false;
                this.btnDel.Enabled = false;
                this.btnSave.Enabled = false;
                this.txtReadingCard.Clear();
                this.lblReaderName.Text="";
                this.lblRoleName.Text="";
                this.lblAllowCounts.Text = "0";
                this.lbl_Remainder.Text = "0";
                this.lblBorrowCount.Text = "0";
                dgvBookList.DataSource = null;
                this.objReader = null;
                MessageBox.Show("借书成功","借书提示");
                this.txtReadingCard.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"借书提示");
                throw;
            }
            
            


        }

        /// <summary>
        /// 键盘事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtReadingCard_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.txtReadingCard.Text.Length != 0 && e.KeyValue == 13)
            {
                objReader = readerService.GetReaderByReadingCard(this.txtReadingCard.Text.Trim());
                if (objReader != null)
                {
                    if (objReader.StatusId == 1)
                    {
                        //显示读者信息
                        this.lblReaderName.Text = objReader.ReaderName;
                        this.lblRoleName.Text = objReader.RoleName;
                        this.lblAllowCounts.Text = objReader.AllowCounts.ToString();
                        this.pbReaderImage.Image = objReader.ReaderImage != "" ? (Image)new Common.SerializeObjectToString().DeserializeObject(this.objReader.ReaderImage) : null;
                        //显示已借阅图书总数和剩余可借图书总数
                        int borrowCount = borrowService.GetBorrowCount(this.txtReadingCard.Text.Trim());
                        this.lblBorrowCount.Text = borrowCount.ToString();
                        this.lbl_Remainder.Text = (objReader.AllowCounts - borrowCount).ToString();
                        //开启图书条码扫描文本框
                        if (objReader.AllowCounts > borrowCount)
                        {
                            this.txtBarCode.Enabled = true;
                            this.txtBarCode.Focus();
                        }
                        else
                        {
                            MessageBox.Show("当前读者借书总数已经达到上限！", "借书提示");
                        }

                    }
                    else
                    {
                        MessageBox.Show("当前借阅证已经被挂失，不能继续借书！", "借书提示");
                    }
                }
                else
                {
                    MessageBox.Show("当前借阅证号不存在！！！", "查询提示");
                    this.txtReadingCard.SelectAll();
                }

            }

        }
        /// <summary>
        /// 图书条码扫描并显示图书信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBarCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.txtBarCode.Text.Trim().Length!=0 && e.KeyValue==13)
            {
                //[4]检查当前借书总数是否已经到达上限
                if (Convert.ToInt32(this.lbl_Remainder.Text)==0)
                    {
                        MessageBox.Show("当前读者借书总数已达上限","借书提示");
                        return;
                    }
            //[1]根据条形码从数据库里面查询
            Books objBook = bookService.GetBookByBarCode(this.txtBarCode.Text.Trim());
            if (objBook != null)
            {
                //[2]判断当前集合是否已经存在该图书对象（如果不存在就添加一个，存在则更新图书数量）
                int count = (from b in this.detailList where b.BarCode.Equals(objBook.barCode) select b).Count();
                if (count == 0)//不存在添加一个对象
                {
                    //封装对象
                    BorrowDetail borrowDetail = new BorrowDetail()
                    {
                        BarCode = objBook.barCode,
                        BookId = objBook.bookId,
                        BookName = objBook.BookName,
                        Expire = DateTime.Now.AddDays(objReader.AllowDay),
                        BorrowCount = 1

                    };
                    detailList.Add(borrowDetail);
                    //【3】同步更新列表数据
                    this.dgvBookList.DataSource = null;
                    this.dgvBookList.DataSource = detailList;
                }
                else//(如果存在更新图书数量)
                {
                    
                    BorrowDetail borrowDetail = (from b in this.detailList where b.BarCode.Equals(objBook.barCode) select b).First<BorrowDetail>();
                    borrowDetail.BorrowCount += 1;
                    this.dgvBookList.Refresh();//刷新

                }
                    //同步显示上面的信息,开启保存与删除按钮
                    this.lblBorrowCount.Text = (Convert.ToInt32(this.lblBorrowCount.Text)+1).ToString();
                    this.lbl_Remainder.Text = (Convert.ToInt32(this.lbl_Remainder.Text)-1).ToString();
                    this.txtBarCode.Clear();//清除当前的条码
                    this.btnSave.Enabled = true;
                    this.btnDel.Enabled = true;
            }
            else
            {
                MessageBox.Show("当前图书不存在", "查询提示");
            }
            
            }

            }
        /// <summary>
        /// 删除图书
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDel_Click(object sender, EventArgs e)
        {
            //【1】根据图书条码找到借书明细对象
            string barCode = this.dgvBookList.CurrentRow.Cells["BarCode"].Value.ToString();
            BorrowDetail borrowDetail = (from b in detailList where b.BarCode.Equals(barCode) select b).First<BorrowDetail>();
            //【2】删除对象(也可以根据需要添加删除确认)
            this.detailList.Remove(borrowDetail);
            //【3】同步显示（列表、借书数据）
            this.dgvBookList.DataSource = null;
            this.dgvBookList.DataSource = this.detailList;
            this.lblBorrowCount.Text = (Convert.ToInt32(this.lblBorrowCount.Text) - borrowDetail.BorrowCount).ToString();
            this.lbl_Remainder.Text = (Convert.ToInt32(this.lbl_Remainder.Text) + borrowDetail.BorrowCount).ToString();
            //【4】根据剩余对象个数禁用相关按钮（保存、删除）
            if (this.detailList.Count==0)
            {
                this.btnSave.Enabled = false;
                this.btnDel.Enabled = false;
            }
        }
    }

        
        

    }
