
using entity;
using server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace LibraryManagerPro
{
    public partial class FrmReturnBook : Form
    {
        private ReaderService readerService = new ReaderService();
        private BorrowService borrowService = new BorrowService();

        private Readers objReader = null;//当前借阅证所对应的读者

        private List<BorrowDetail> nonReturnList = new List<BorrowDetail>();//未还图书列表
        private List<BorrowDetail> returnList = new List<BorrowDetail>();//还书扫描列表

        private int retrunCount = 0;//全部还书总数

        #region 初始化
        public FrmReturnBook()
        {
            InitializeComponent();
            this.txtBarCode.Enabled = false;
            this.btnConfirmReturn.Enabled = false;
            this.dgvNonReturnList.AutoGenerateColumns = false;
            this.dgvReturnList.AutoGenerateColumns = false;

            //加粗显示相关的借书数据
            this.dgvNonReturnList.Columns["BorrowCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dgvNonReturnList.Columns["BorrowCount"].DefaultCellStyle.Font = new Font("微软雅黑", 14);

            this.dgvNonReturnList.Columns["ReturnCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dgvNonReturnList.Columns["ReturnCount"].DefaultCellStyle.Font = new Font("微软雅黑", 14);

            this.dgvNonReturnList.Columns["NonReturnCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dgvNonReturnList.Columns["NonReturnCount"].DefaultCellStyle.Font = new Font("微软雅黑", 14);

            this.dgvReturnList.Columns["ReturnCount_"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dgvReturnList.Columns["ReturnCount_"].DefaultCellStyle.Font = new Font("微软雅黑", 14);
            new Common.DataGridViewStyle().DgvStyle1(this.dgvReturnList);

        }

        #endregion

        #region 显示读者信息（个人信息+图书借阅信息）

        #endregion

        #region 显示还书列表
        private void txtBarCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.txtBarCode.Text.Trim().Length != 0 && e.KeyValue == 13)
            {
                //【1】检查当前借书列表中是否存在该图书
                if ((from b in this.nonReturnList where b.BarCode.Equals(this.txtBarCode.Text.Trim()) select b).Count() == 0)
                {
                    MessageBox.Show("当前图书不在借书列表中，请和读者确认该图书的信息", "还书提示");
                    this.txtBarCode.SelectAll();
                    return;
                }
                //【2】从还书集合中查询扫描的图书总数（用来决定是新增一个还书对象还是只是更新还书总数）
                int count = (from b in this.returnList where b.BarCode == this.txtBarCode.Text.Trim() select b).Count();
                if (count == 0)//如果图书不存在，则添加一个新的还书对象
                {
                    this.returnList.Add(new BorrowDetail()
                    {
                        BarCode = this.txtBarCode.Text.Trim(),
                        ReturnCount = 1,
                        BookName = (from b in this.nonReturnList where b.BarCode.Equals(this.txtBarCode.Text.Trim()) select b).First<BorrowDetail>().BookName

                    });
                    //显示要还的图书信息
                    this.dgvReturnList.DataSource = null;
                    this.dgvReturnList.DataSource = returnList;
                }
                else//存在则更新数据
                {
                    //从还书集合中找到该还书对象
                    BorrowDetail bojReturnDetail = (from b in this.returnList where b.BarCode == this.txtBarCode.Text.Trim() select b).First<BorrowDetail>();

                    //从未还书集合中找到该对象的借书总数(注意不是第一条数据，而是他们相同的和)
                    /*BorrowDetail bojNonReturnDetail =
                    (from b in this.nonReturnList where b.BarCode==this.txtBarCode.Text.Trim() select b).First<BorrowDetail>();*/

                    //u=>u.NonRetrunConuent求和的意思
                    int nonReturnCount = (from b in this.nonReturnList where b.BarCode.Equals(this.txtBarCode.Text.Trim()) select b).Sum(u => u.NonReturnCount);


                    //判断当前还书总数是否等于总借书数
                    if (nonReturnCount<bojReturnDetail.ReturnCount)
                    {
                        MessageBox.Show("还书总数不能大于借书总数","还书提示");
                        this.txtBarCode.Clear();
                        return;

                    }
                    else
                    {
                        bojReturnDetail.ReturnCount += 1;//还书总数加一
                        this.dgvReturnList.Refresh();
                        


                    }
                }
                //同步显示还书总数
                retrunCount++;
                this.lblReturnCount.Text = retrunCount.ToString();
                //开启按钮
                this.btnConfirmReturn.Enabled = true;
                //清空条码输入
                this.txtBarCode.Clear();
                this.txtReadingCard.Focus();
               
            }

        }

        #endregion

        #region 还书操作
        /// <summary>
        /// 图书归还
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConfirmReturn_Click(object sender, EventArgs e)
        {
            
            try
            {
                //【1】调用业务逻辑操作后台
                borrowService.ReturnBook(this.returnList,this.nonReturnList,Program.admin.AdminName);

                //【2】清除数据
                this.groupBox1.Text = "";
                this.pbReaderImage.Image = null;
                this.dgvNonReturnList.DataSource = null;
                this.dgvReturnList.DataSource = null;
                this.btnConfirmReturn.Enabled = false;
                this.txtBarCode.Clear();
                this.txtBarCode.Enabled = false;
                this.txtReadingCard.Text = "";
                this.lblReaderName.Text = "";
                this.lblRoleName.Text = "";
                this.lblAllowCounts.Text = "0";
                this.lbl_Remainder.Text = "0";
                this.lblBorrowCount.Text = "0";
                this.txtReadingCard.Focus();

                MessageBox.Show("还书成功","还书提示");



            }
            catch (Exception)
            {

                throw;
            }




        }
        #endregion

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

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

                        //开启图书条码扫描文本框已经还书按钮
                        this.txtBarCode.Enabled = true;
                        this.btnConfirmReturn.Enabled = true;
                        //显示读者未归还的图书
                        nonReturnList = borrowService.GetBorrowDetailByReadingCard(this.txtReadingCard.Text.Trim());
                        this.dgvNonReturnList.DataSource = null;
                        this.dgvNonReturnList.DataSource = nonReturnList;

                        //显示已借阅图书总数和剩余可借图书总数
                        /* int borrowCount = borrowService.GetBorrowCount(this.txtReadingCard.Text.Trim());
                         this.lblBorrowCount.Text = borrowCount.ToString();
                         this.lbl_Remainder.Text = (objReader.AllowCounts - borrowCount).ToString();*/

                        //基于linq实现
                        int borrowCount = (from n in nonReturnList select n).Sum(u => u.NonReturnCount);
                        int remainder = objReader.AllowCounts - borrowCount;
                        this.lblBorrowCount.Text = borrowCount.ToString();
                        this.lbl_Remainder.Text = remainder.ToString();
                    }
                    else
                    {
                        MessageBox.Show("当前借阅证已经被挂失，目前不能拿继续使用！", "还书提示");
                    }
                }
                else
                {
                    MessageBox.Show("当前借阅证号不存在！！！", "查询提示");
                    this.txtReadingCard.SelectAll();
                }

            }

        }
    }
}
