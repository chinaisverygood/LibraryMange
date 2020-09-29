using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using entity;
using dao;


namespace server
{
   public class BorrowService
    {
        private BorrowDao borrowDao = new BorrowDao();
        /// <summary>
        /// 根据借阅证查询当前读者借书总数
        /// </summary>
        /// <param name="readingCard"></param>
        /// <returns></returns>
        public int GetBorrowCount(string readingCard)
        {
            return borrowDao.GetBorrowCount(readingCard);
        }
        /// <summary>
        /// 保存借书信息
        /// </summary>
        /// <param name="main"></param>
        /// <param name="detail"></param>
        /// <returns></returns>
        public bool BorrowBook(BorrowInfo main, List<BorrowDetail> detail)
        {
            return borrowDao.BorrowBook(main,detail);
        }


        /// <summary>
        /// 根据借阅证号查询当前读者借书总数
        /// </summary>
        /// <param name="readingCard"></param>
        /// <returns></returns>
       public List<BorrowDetail> GetBorrowDetailByReadingCard(string readingCard)
        {
          return  borrowDao.GetBorrowDetailByReadingCard(readingCard);
        }
        /// <summary>
        /// 还书操作
        /// </summary>
        /// <param name="ReturnList">还书对象集合</param>
        /// <param name="nonReturnList">未还图书对象集合</param>
        /// <param name="adminName">管理员</param>
        /// <returns></returns>
        public bool ReturnBook(List<BorrowDetail> ReturnList,List<BorrowDetail> nonReturnList,string adminName)
        {
            #region 还书算法核心业务
            //【1】创建一个能够提交给数据库的还书集合(方法中的参数returnList不能直接提交给数据库)
            List<ReturnBook> returnBookList = new List<ReturnBook>();

            //【2】遍历还书集合，将还书和借书对应起来
            foreach (BorrowDetail returnItem in ReturnList)
            {
                //1 取出当前图书本次归还的总数（比如本次还3本）
                int returnCount = returnItem.ReturnCount;
                //2 在未还图书结合中找到该书《出借记录》（可能是一次借书或者多次借书）
                List<BorrowDetail> borrowList = (from b in nonReturnList where b.BarCode.Equals(returnItem.BarCode) select b).ToList<BorrowDetail>();
                //3 遍历当前图书<出借记录>
                foreach (BorrowDetail borrowItem in borrowList)
                {
                    if (borrowItem.NonReturnCount==returnCount || borrowItem.NonReturnCount>returnCount)
                    {
                        //如果当前借书记录中的未还书总数==本次还书总数，或者未还书总数>本次还书总数
                        returnBookList.Add(new ReturnBook() { 
                        BorrowDetailId=borrowItem.BorrowDetailId,
                        ReturnCount=returnCount,

                        });
                        
                        break;
                    }
                    else //如果还书总数>当前图书未还书总数
                    {
                        returnBookList.Add(new ReturnBook() { 

                        BorrowDetailId=borrowItem.BorrowDetailId,
                        ReturnCount=borrowItem.NonReturnCount,

                        });

                        returnCount -= borrowItem.NonReturnCount;//减掉当前冲掉的还书总数，
                        //得到剩余的还书总数，再继续找到当前图书的借书记录
                    }  
                }
            }

            #endregion
            //【3】添加还书操作的管理员
            for (int i = 0; i < returnBookList.Count; i++)
            {
                returnBookList[i].AdimName_R = adminName;

            }
            //【4】调用后台数据访问方法

            return borrowDao.ReturnBook(returnBookList);
        }
    }
}
