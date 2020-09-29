using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using entity;
using DButil;
using System.Data;
using System.Data.SqlClient;

namespace dao
{
    /// <summary>
    /// 借书数据访问类
    /// </summary>
  public  class BorrowDao
    {
        /// <summary>
        /// 根据借阅证查询当前读者借书总数
        /// </summary>
        /// <param name="readingCard"></param>
        /// <returns></returns>
        public  int GetBorrowCount(string readingCard)
        {
          object count= SqlDB.GetsingleResultByProcedure("usp_QueryBorrowCount",new SqlParameter[] {new SqlParameter("@ReadingCard", readingCard)});
          return Convert.ToInt32(count);

        }
        /// <summary>
        /// 保存借书信息
        /// </summary>
        /// <param name="main"></param>
        /// <param name="detail"></param>
        /// <returns></returns>
        public bool BorrowBook(BorrowInfo main ,List<BorrowDetail> detail )
        {
            //借书主表插入的Sql语句
            string mainSql = "insert into BorrowInfo (BorrowId,ReaderId,AdminName_B) values (@BorrowId,@ReaderId,@AdminName_B)";
            //借书明细表信息插入sql语句
            StringBuilder sb = new StringBuilder();
            sb.Append("insert into BorrowDetail (BorrowId,BookId,BorrowCount,ReturnCount,NonReturnCount,Expire)");
            sb.Append(" values(@BorrowId,@BookId,@BorrowCount,@ReturnCount,@NonReturnCount,@Expire)");

            //创建主表参数数组
            SqlParameter[] mainParam = new SqlParameter[] {
                new SqlParameter("@BorrowId",main.BorrowId),
                new SqlParameter("@ReaderId",main.ReaderId),
                new SqlParameter("@AdminName_B",main.AdminName_B)
            };

            //创建借阅详细表参数数组
            List<SqlParameter[]> detailParam = new List<SqlParameter[]>();
            foreach (BorrowDetail item in detail)
            {
                detailParam.Add(new SqlParameter[] {
                new SqlParameter("@BorrowId",item.BorrowId),
                new SqlParameter("@BookId",item.BookId),
                new SqlParameter("@BorrowCount",item.BorrowCount),
                new SqlParameter("@ReturnCount",item.ReturnCount),
                new SqlParameter("@NonReturnCount",item.NonReturnCount),
                new SqlParameter("@Expire",item.Expire),
                }) ;
            }
            //启动事务提交多个对象
            return SqlDB.updateByTran(mainSql,mainParam,sb.ToString(),detailParam);

        }

        /// <summary>
        /// 根据借阅证号查询读者借书信息列表
        /// </summary>
        /// <param name="readingCard">借阅证号</param>
        /// <returns></returns>
        public List<BorrowDetail> GetBorrowDetailByReadingCard(string readingCard)
        {
           SqlParameter[] param = new SqlParameter[] { new SqlParameter("@ReadingCard",readingCard) };
            List<BorrowDetail> detailList = new List<BorrowDetail>();

           SqlDataReader objReader= SqlDB.QueryByProcedure("usp_QueryBookReadingCard",param);

            while (objReader.Read())
            {
                detailList.Add(new BorrowDetail()
                {
                    ReadingCard=objReader["ReadingCard"].ToString(),
                    BorrowDetailId=Convert.ToInt32(objReader["BorrowDetailId"]),
                    BookId=Convert.ToInt32( objReader["BookId"]),
                    BorrowId=objReader["BorrowId"].ToString(),
                    BookName=objReader["BookName"].ToString(),
                    BarCode=objReader["BarCode"].ToString(),
                    BorrowCount=Convert.ToInt32(objReader["BorrowCount"]),
                    ReturnCount=Convert.ToInt32(objReader["ReturnCount"]),
                    NonReturnCount=Convert.ToInt32(objReader["NonReturnCount"]),
                    BorrowDate=Convert.ToDateTime(objReader["BorrowDate"]),
                    Expire=Convert.ToDateTime( objReader["Expire"]),
                    StatusDesc=objReader["StatusDesc"].ToString(),
                });
            }
            objReader.Close();
            return detailList;
        }
        #region 还书操作
        /// <summary>
        /// 根据还书集合完成还书操作
        /// </summary>
        /// <param name="bookList"></param>
        /// <returns></returns>
        public bool ReturnBook(List<ReturnBook> bookList)
        {
            List<SqlParameter[]> param = new List<SqlParameter[]>();
            foreach (ReturnBook item in bookList)
            {
                param.Add(new SqlParameter[] { 
                new SqlParameter("@BorrowDetailId",item.BorrowDetailId),
                new SqlParameter("ReturnCount",item.ReturnCount),
                new SqlParameter("AdminName_R",item.AdimName_R),
              });  
            }
            return SqlDB.updateByTran("usp_ReturnBook",param);
        }
        #endregion
    }
}
