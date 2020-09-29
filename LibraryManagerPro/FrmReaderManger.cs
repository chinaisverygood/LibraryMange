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
using System.Windows.Forms.VisualStyles;
using MyVideo;

namespace LibraryManagerPro
{
    public partial class FrmReaderManger : Form
    {
        private ReaderService readerService = new ReaderService();
       //private Video ovjVideo = null;
        private Readers objCurrentReader = null;

        public FrmReaderManger()
        {
            InitializeComponent();
            //初始化办证角色下拉框
         DataTable dt= readerService.GetAllReaderRole();
            this.cboReaderRole.DataSource = dt;
            this.cboReaderRole.DisplayMember = "RoleName";
            this.cboReaderRole.ValueMember = "RoleId"; 
            
            //用于查询角色（初始化）
            this.cboRole.DataSource = dt.Copy();//复制后在复制防止选择时同步变化
            this.cboRole.DisplayMember = "RoleName";
            this.cboRole.ValueMember = "RoleId";


            //初始化按钮禁用
            this.btnEdit.Enabled = false;
            this.btnEnable.Enabled = false;
            

        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
       /// <summary>
       /// 提交查询
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
        private void btnQuery_Click(object sender, EventArgs e)
        {
            //验证
            if (this.txt_IDCard.Text.Trim().Length==0 && this.txt_ReadingCard.Text.Trim().Length==0)
            {
                MessageBox.Show("请选择一个查询内容并输入查询条件","查询提示");
                return;
            }
            if (this.rdoIDCard.Checked && this.txt_IDCard.Text.Trim().Length!=0)
            {
                this.objCurrentReader = readerService.GetReaderByIDCard(this.txt_IDCard.Text.Trim());


            }
            else this.objCurrentReader = readerService.GetReaderByReadingCard(this.txt_ReadingCard.Text.Trim());
            //判读是否查询到信息
            if (objCurrentReader!=null)
            {
                if (objCurrentReader.StatusId!=0)
                {
                    this.btnEnable.Enabled = true;
                }
                this.btnEdit.Enabled = true;
                //显示读者信息
                this.lblAddress.Text = objCurrentReader.ReaderAddress;
                this.lblPhone.Text = objCurrentReader.PhoneNumber;
                this.lblPostCode.Text = objCurrentReader.PostCode;
                this.lblReaderName.Text = objCurrentReader.ReaderName;
                this.lblRoleName.Text = objCurrentReader.RoleName;
                this.lblReadingCard.Text = objCurrentReader.ReadingCard;
                this.lblGender.Text = objCurrentReader.Gender;
                this.pbReaderImg.Image = objCurrentReader.ReaderImage != "" ? (Image)new Common.SerializeObjectToString().DeserializeObject(objCurrentReader.ReaderImage) : null ;
            }
            else
            {
                MessageBox.Show("当前读者不存在","提示");
                this.btnEnable.Enabled = false;
                this.btnEnable.Enabled = false;
                return;
            }


        }
        /// <summary>
        /// 按照角色查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQueryReader_Click(object sender, EventArgs e)
        {
            //清除所有内容
            this.lvReader.Items.Clear();
            int readerCount = 0;//提前定义一个接收输出参数的变量
           List<Readers> readerList= readerService.GetReaderByRole(this.cboRole.SelectedValue.ToString(),out readerCount);
            //给ListView绑定数据源
            foreach (Readers reader in readerList)
            {
                //[1]创建工业ListView对象参数具有唯一性
                ListViewItem lvltem = new ListViewItem(reader.ReaderId.ToString());
                //[2]把List View对象添加到ListView中
                this.lvReader.Items.Add(lvltem);
                //[3]在当前listViewItem对象中添加子项
                lvltem.SubItems.AddRange(new string[] { 
                reader.ReadingCard,
                reader.ReaderName,
                reader.Gender,
                reader.PhoneNumber,
                reader.RegTime.ToShortDateString(),
                reader.StatusDesc,
                reader.ReaderAddress
                });
            }
            //显示当前角色对应总数
            this.lblReaderCount.Text = readerCount.ToString();

        }
        /// <summary>
        /// 会员添加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            //[1]数据验证

            //[2]封装对象
            Readers objReader = new Readers()
            {
                ReaderName = this.txtReaderName.Text.Trim(),
                ReadingCard = this.txtReadingCard.Text.Trim(),
                Gender = this.rdoMale.Checked ? "男" : "女",
                RoleId = Convert.ToInt32(this.cboReaderRole.SelectedValue),
                PostCode = this.txtPostcode.Text.Trim(),
                PhoneNumber = this.PhoneNumber.Text.Trim(),
                ReaderAddress = this.txtAddress.Text.Trim(),
                ReaderImage = this.pbReaderPhoto.Image != null ? new Common.SerializeObjectToString().SerializeObject(this.pbReaderPhoto.Image) : "",
                IDCard = this.txtIdCard.Text.Trim()

            };

            //调用底层
            try
            { 
                readerService.AddReader(objReader);
                MessageBox.Show("添加成功","添加提示");
                //清空数据
                this.pbReaderImg.Image = null;
                this.rdoMale.Checked = true;
                this.cboReaderRole.SelectedIndex = -1;

                //清空text Box
                foreach (Control item in this.tabPage2.Controls)
                {
                    if (item is TextBox)
                    {
                        item.Text = "";
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                }
        }
        /// <summary>
        /// 挂失借阅证
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEnable_Click(object sender, EventArgs e)
        {
           DialogResult result= MessageBox.Show("确认挂失当前借阅证？","借阅号挂失提示",MessageBoxButtons.OKCancel,MessageBoxIcon.Question);
            if (result==DialogResult.OK)
            {
                readerService.ForbiddenReaderCard(objCurrentReader.ReaderId.ToString());
                MessageBox.Show("挂失成功","挂失提示");
            }
        }
        /// <summary>
        /// 修改信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEdit_Click(object sender, EventArgs e)
        {
            FrmEidtReader frmEdit = new FrmEidtReader(this.objCurrentReader);
            frmEdit.Show();

        }
    }
}
