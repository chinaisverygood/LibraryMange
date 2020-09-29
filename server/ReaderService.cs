using dao;
using entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
namespace server
{
    /// <summary>
    /// 读者业务逻辑
    /// </summary>
    public  class ReaderService
    {
        private ReaderDao readerDao = new ReaderDao();

        /// <summary>
        /// 会员办证(添加读者信息)
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public int AddReader(Readers reader)
        {
            return readerDao.AddReader(reader);
        }
        /// <summary>
        /// 修改读者信息
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public int EditReader(Readers reader)
        {
            return readerDao.EditReader(reader);
        }
        /// <summary>
        /// 借阅证挂失(修改读者状态)
        /// </summary>
        /// <param name="readerId"></param>
        /// <returns></returns>
        public int ForbiddenReaderCard(string readerId)
        {
            return readerDao.ForbiddenReaderCard(readerId);
        }
        
        /// <summary>
        ///查询全部会员角色
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllReaderRole()
        {
            return readerDao.GetAllReaderRole();
        }
        
        /// <summary>
        /// 根据借阅证号查询读者信息
        /// </summary>
        /// <param name="readingCard"></param>
        /// <returns></returns>
        public Readers GetReaderByReadingCard(string readingCard)
        {
            return readerDao.GetReaderByReadingCard(readingCard);
        }
        
        /// <summary>
        /// 根据身份证号查询读者信息
        /// </summary>
        /// <param name="idCard"></param>
        /// <returns></returns>
        public Readers GetReaderByIDCard(string idCard)
        {
            return readerDao.GetReaderByIDCard(idCard);
        }

        /// <summary>
            ///根据身会员角色查询读者信息（同时找到对应该角色的会员总数）
            /// </summary>
            /// <param name="roleId">角色Id</param>
            /// <param name="readerCount">当前角色总数</param>
            /// <returns>返回对应会员信息以及总数</returns>
        public List<Readers> GetReaderByRole(string roleId,out int readerCount)
    {
        List<Readers> readerList= readerDao.GetReaderByRole(roleId,out readerCount);
        //根据借阅证状态编号修改成对应名称
        for (int i = 0; i < readerList.Count; i++)
        {
            switch (readerList[i].StatusId)
            {
                case 1:
                    readerList[i].StatusDesc = "正常";
                    break;
                case 0:
                    readerList[i].StatusDesc = "禁用";
                    break;
            }
        }
        return readerList;
    }
    }
}
