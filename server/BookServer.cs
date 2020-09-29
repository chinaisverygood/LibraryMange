using dao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using entity;
using System.Runtime.CompilerServices;

namespace server
{
 public  class BookServer
    {
        private static BookDao bookDao = new BookDao();

        public List<Categories> GetAllCategory()
        {
            return bookDao.GetAllCategory();
        }
        public List<PubliShers> GetAllPublishers()
        {
            return bookDao.GetAllPublisher();
        }
        public int AddBook(Books book)
        {
            return bookDao.AddBook(book);
        }

        public bool BarCodeIsExisted(string barCode)
        {
            int count= bookDao.GetCountByCarCode(barCode);
            if (count == 1)
            { 
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 根据图书条码更新图书总量
        /// </summary>
        /// <param name="barCode"></param>
        /// <param name="bookCount"></param>
        /// <returns></returns>
        public bool AddBookCount(string barCode, int bookCount)
        {
          int resutl=  bookDao.AddBookCount(barCode,bookCount);
            if (resutl==1)
            {
                return true;

            }
            else
            {
                return false;
            }
        }

        public Books GetBookByBarCode(string barCode)
        {
            return bookDao.GetBookByBarCode(barCode);
        }
        /// <summary>
        /// 根据组合查询条件查询图书信息
        /// </summary>
        /// <param name="categoryId">图书分类编码</param>
        /// <param name="barCode">图书条码</param>
        /// <param name="bookName">图书对象</param>
        /// <returns></returns>
        public List<Books> GetBooks(int categoryId,string barCode,string bookName)
        {
          List<Books> bookList= bookDao.GetBooks(categoryId,barCode,bookName);
            return bookList;
        }
        /// <summary>
        /// 图书信息修改
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>
        public int EditBook(Books book)
        {
            return bookDao.EditBook(book);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="bookId"></param>
        /// <returns></returns>
        public int DeleteBook(string bookId)
        {
            return bookDao.DeleteBook(bookId);
        }
    }
}
