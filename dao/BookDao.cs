using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using entity;
using DButil;

namespace dao
{
  public  class BookDao
    {
        /// <summary>
        /// 获取全部图书分类
        /// </summary>
        /// <returns></returns>
        public List<Categories> GetAllCategory()
        {
            string sql = "select * from Categories";
            List<Categories> list = new List<Categories>();
            SqlDataReader reader = SqlDB.Query(sql);
            while (reader.Read())
            {
                list.Add(new Categories()
                {
                    CategoryId=Convert.ToInt32(reader["CategoryId"]),
                    CategoryName=reader["CategoryName"].ToString()
                });
            }
            reader.Close();
            return list;
        }
        /// <summary>
        /// 获取所有的出版社信息
        /// </summary>
        /// <returns></returns>
        public List<PubliShers> GetAllPublisher()
        {
            string sql = "select PublisherId,PublisherName  from publishers";
            List<PubliShers> list = new List<PubliShers>();
            SqlDataReader reader = SqlDB.Query(sql);
            while(reader.Read()){
                list.Add(new PubliShers()
                {
                    PublisherId = Convert.ToInt32(reader["PublisherId"]),
                    PublisherName = reader["PublisherName"].ToString()
                });
            }
            reader.Close();
            return list;
        }
        /// <summary>
        /// 判断条码是否已经存在
        /// </summary>
        /// <param name="barCode"></param>
        /// <returns></returns>
        public int GetCountByCarCode(string barCode)
        {
            string sql = "select COUNT(*) from Books where BarCode=@BarCode";
            SqlParameter[] param = new SqlParameter[]{new SqlParameter("@BarCode",barCode)}; 
            return Convert.ToInt32( SqlDB.GetSingleResult(sql,param));
        }
        /// <summary>
        /// 使用存储过程添加图书
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>
        public int AddBook(Books book)
        {
            //封装参数
            SqlParameter[] param = new SqlParameter[]
            {
                new SqlParameter("@BarCode",book.barCode),
                new SqlParameter("@BookName",book.BookName),
                new SqlParameter("@Author",book.author),
                new SqlParameter("@PublisherId",book.publisherId),
                new SqlParameter("@PublishDate",book.publishDate),
                new SqlParameter("@BookCategory",book.bookCategory),
                new SqlParameter("@UnitPrice",book.unitPrice),
                new SqlParameter("@BookImage",book.bookImage),
                new SqlParameter("@BookCount",book.BookCount),
                new SqlParameter("@BookPosition",book.bookPosition),
                new SqlParameter("@Remainder",book.Remainder),
                
                
            };
            //调用通用数据访问方法提交对象
            return SqlDB.UpdateByProcedure("usp_AddBook", param);
        }
        /// <summary>
        /// 根据图书条码查询图书对象
        /// </summary>
        /// <param name="barCode">图书条码</param>
        /// <returns></returns>
        public Books GetBookByBarCode(string barCode)
        {
            string sql = "select BookId, BarCode, BookName, Author,Publishers.PublisherId, PublishDate, BookCategory, UnitPrice, BookImage, BookCount, Remainder, BookPosition,RegTime,PublisherName,CategoryName from Books";
            sql += " inner join Publishers on Publishers.PublisherId=Books.PublisherId ";
            sql += "inner join Categories on Categories.CategoryId=Books.BookCategory ";
            sql += "where BarCode=@BarCode";

            SqlParameter[] param = new SqlParameter[]
            {
                new SqlParameter("@BarCode",barCode),
            };
            SqlDataReader reader= SqlDB.Query(sql,param);
            Books book = null;
            if (reader.Read())
            {
                book = new Books()
                {

                    author = reader["Author"].ToString(),

                    barCode = reader["BarCode"].ToString(),

                    bookCategory = Convert.ToInt32(reader["BookCategory"]),

                    CategoryName = reader["CategoryName"].ToString(),

                    BookCount = Convert.ToInt32(reader["BookCount"]),

                    bookId = Convert.ToInt32(reader["BookId"]),

                    BookName = reader["BookName"].ToString(),

                    bookPosition = reader["BookPosition"].ToString(),

                    publishDate = Convert.ToDateTime(reader["PublishDate"]),

                    publisherId = Convert.ToInt32(reader["PublisherId"]),

                    PublisherName = reader["PublisherName"].ToString(),

                    unitPrice = Convert.ToDouble(reader["UnitPrice"]),

                    RegTime = Convert.ToDateTime(reader["RegTime"]),

                    bookImage = reader["BookImage"] is DBNull? "" : reader["BookImage"].ToString(),


                };

            }
            reader.Close();
            return book;
        }
        /// <summary>
        /// 根据图书条码更新图书总量
        /// </summary>
        /// <param name="barCode"></param>
        /// <param name="bookCount"></param>
        /// <returns></returns>
        public int AddBookCount(string barCode,int bookCount)
        {
            string sql = "update books set BookCount=BookCount+@BookCount where BarCode=@BarCode";
            SqlParameter[] param = new SqlParameter[]
            {
                new SqlParameter("@BookCount",bookCount),
                new SqlParameter("@BarCode",barCode),
            };
            return SqlDB.Update(sql,param);
        }
        /// <summary>
        /// 根据组合条件查询图书信息
        /// </summary>
        /// <param name="cateGoryId">图书类别编号</param>
        /// <param name="barCode">图书条码</param>
        /// <param name="bookName">图书名称</param>
        /// <returns>返回数据集合</returns>
        public List<Books> GetBooks(int cateGoryId,string barCode,string bookName)
        {
            //定义集合用来存参数
            List<SqlParameter> paramList = new List<SqlParameter>();

            //定义查询sql语句
            string sql = "select BookId,BarCode,BookName,Author,Books.PublisherId,PublishDate,BookCategory,UnitPrice,BookImage,BookCount,Remainder,BookPosition,RegTime,PublisherName,CategoryName from Books";
            sql += " inner join Publishers on Publishers.PublisherId=Books.PublisherId";
            sql += " inner join Categories on Categories.CategoryId=Books.BookCategory";
            sql += " where 1=1";

            //根据条件添加查询参数
            if (barCode!=null && !"".Equals(barCode))
            {
                sql += " and BarCode=@BarCode";
                paramList.Add(new SqlParameter("@BarCode",barCode));
                
            }
            else
            {
                if (cateGoryId!=-1)
                {
                    sql += " and CategoryId=@CategoryId";
                    paramList.Add(new SqlParameter("@CategoryId",cateGoryId));
                    
                }
                if (bookName!=null && !"".Equals(bookName))
                {
                    sql += " and BookName Like '%'+@BookName+'%'";
                    paramList.Add(new SqlParameter("@BookName",bookName));
                }
                
            }
            //执行查询
            SqlDataReader reader = SqlDB.Query(sql,paramList.ToArray());
            List<Books> bookList = new List<Books>();
            while (reader.Read())
            {
                bookList.Add(new Books() {
                    author = reader["Author"].ToString(),

                    barCode = reader["BarCode"].ToString(),

                    bookCategory = Convert.ToInt32(reader["BookCategory"]),

                    CategoryName = reader["CategoryName"].ToString(),

                    BookCount = Convert.ToInt32(reader["BookCount"]),

                    bookId = Convert.ToInt32(reader["BookId"]),

                    BookName = reader["BookName"].ToString(),

                    bookPosition = reader["BookPosition"].ToString(),

                    publishDate = Convert.ToDateTime(reader["PublishDate"]),

                    publisherId = Convert.ToInt32(reader["PublisherId"]),

                    PublisherName = reader["PublisherName"].ToString(),

                    unitPrice = Convert.ToDouble(reader["UnitPrice"]),

                    RegTime = Convert.ToDateTime(reader["RegTime"]),

                    bookImage = reader["BookImage"] is DBNull ? "" : reader["BookImage"].ToString()

                });
            }
            reader.Close();
            return bookList;

        }
        /// <summary>
        /// 图书修改
        /// </summary>
        /// <param name="book">修改的图书对象</param>
        /// <returns>反回影响行数</returns>
        public int EditBook(Books book)
        {
            //封装参数
            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@BookId",book.bookId),
                new SqlParameter("@BookName",book.BookName),
                new SqlParameter("@Author",book.author),
                new SqlParameter("@PublisherId",book.publisherId),
                new SqlParameter("@PublishDate",book.publishDate),
                new SqlParameter("@BookCategory",book.bookCategory),
                new SqlParameter("@UnitPrice",book.unitPrice),
                new SqlParameter("@BookImage",book.bookImage),
                new SqlParameter("@BookPosition",book.bookPosition),      
            };

            //调用通用数据访问类方法实现修改（使用存储过程）
            return SqlDB.UpdateByProcedure("usp_EditBook", param);
        }
        /// <summary>
        /// 图书删除
        /// </summary>
        /// <param name="bookId"></param>
        /// <returns></returns>
        public int DeleteBook(string bookId)
        {
            string sql= "delete from Books where BookId=@BookId";
            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@BookId",bookId)
            };
            try
            {
                return SqlDB.Update(sql,param);

            }
            catch (SqlException ex)
            {
                if (ex.Number==547)//547当前对象已经被其他数据表引用
                    throw new Exception("当前图书已经被其他数据表,不能直接删除");
                else
                    throw new Exception("删除图书出现异常：" + ex.Message);
            }
            catch (Exception ex)
            {
                throw ex;

            }

        }
    }
}
