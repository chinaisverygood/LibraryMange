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
    public partial class FrmEidtReader : Form
    {
        private ReaderService readerService = new ReaderService();
        private Readers readerEndit = null;
        public FrmEidtReader(Readers reader)
        {
            InitializeComponent();
            //初始化角色下拉框
            this.cboReaderRole.DataSource = readerService.GetAllReaderRole();
            this.cboReaderRole.DisplayMember = "RoleName";
            this.cboReaderRole.ValueMember = "RoleId";

            //显示读者信息
            this.txtAddress.Text = reader.ReaderAddress;
            this.txtPhone.Text = reader.PhoneNumber;
            this.txtPostcode.Text = reader.PostCode;
            if (reader.Gender=="男")
            {
                this.rdoMale.Checked=true;
            }
            else
            {
                this.rdoFemale.Checked = true;
            }
            this.txtReaderName.Text= reader.ReaderName;
            this.txtReadingCard.Text = reader.ReadingCard;
            this.cboReaderRole.Text = reader.RoleName;
            this.pbReaderPhoto.Image = reader.ReaderImage != "" ? (Image)new Common.SerializeObjectToString().DeserializeObject(reader.ReaderImage) : null;

            readerEndit = reader;//保存当前读者对象，为后面的修改使用





        }

        private void FrmEidtReader_Load(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 保存修改信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            //数据验证
           

            //封装对象
            Readers objReader = new Readers()
            {
                ReaderId = readerEndit.ReaderId,
                ReaderName = txtReaderName.Text.Trim(),
                Gender=this.rdoMale.Checked ? "男": "女", 
                RoleId = Convert.ToInt32(cboReaderRole.SelectedValue.ToString()),
                PostCode = txtPostcode.Text.Trim(),
                PhoneNumber = txtPhone.Text.Trim(),
                ReaderAddress = txtAddress.Text.Trim(),
                ReaderImage = this.pbReaderPhoto.Image != null ? new Common.SerializeObjectToString().SerializeObject(this.pbReaderPhoto.Image) : "",
            };  

           

            //提交修改后的数据
            try
            {
                readerService.EditReader(objReader);
                MessageBox.Show("提交成功","提示信息");
                this.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }

        }
    }
}
