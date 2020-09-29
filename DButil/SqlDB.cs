using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace DButil
{
   public  class SqlDB
    {
        //封装数据库连接字符串
        //private static string connString = ConfigurationManager.ConnectionStrings["connString"].ToString();
        //static  string connString = "Data Source=192.168.31.217;Initial Catalog=LibraryDB;Integrated Security=True";
        static  string connString = "Server=192.168.31.217;database=LibraryDB;uid=sa;pwd=123456";
        //创建连接字符串
        private static SqlConnection conn = new SqlConnection(connString);

        #region 封装格式化sql语句执行的各种方法
        //无参更新
        public static int Update(string sql)
        {
            SqlCommand comm = new SqlCommand(sql,conn);
            try
            {
                conn.Open();
                return comm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                //将异常信息写入日志
                string errorInfo = "调用public static int Update(string sql)时发生异常" + ex.Message;
                WriteLog(errorInfo);
                throw new Exception(errorInfo);
            }
            finally
            {
                conn.Close();
            }
        }
        //查询,返回一个结果集
        public static SqlDataReader Query(string sql)
        {
            
            SqlCommand comm = new SqlCommand(sql, conn);
            try
            {
                conn.Open();
                return comm.ExecuteReader(CommandBehavior.CloseConnection);
                
            }
            catch (Exception ex)
            {
                //将异常信息写入日志
                string errorInfo = "调用public static SqlDataReader Query(string sql)时发生异常" + ex.Message;
                WriteLog(errorInfo);
                throw new Exception(errorInfo);
            }
            
        }

        //dataset写入excel
        public static DataSet getDataSet(string sql)
        {
            SqlConnection con = new SqlConnection(connString);
            SqlCommand comm = new SqlCommand(sql, con);
            SqlDataAdapter da = new SqlDataAdapter(comm);//创建数据适配对象
            DataSet ds = new DataSet();//创建一个内存数据集

            try
            {
                con.Open();
                da.Fill(ds);//使用数据适配器填充数据库集
                return ds;
            }
            catch (Exception ex)
            {
                //将异常信息写入日志
                string errorInfo = "调用public static SqlDataReader Query(string sql)时发生异常" + ex.Message;
                WriteLog(errorInfo);
                throw new Exception(errorInfo);
               
               
            }

        }
        #endregion
        #region 封装带参sql预计执行的各种方法
        //带参更新
        public static int Update(string sql,SqlParameter[] param)
        {
            SqlCommand comm = new SqlCommand(sql, conn);

            try
            {
                conn.Open();
                comm.Parameters.AddRange(param);//封装参数
                return comm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                //将异常信息写入日志
                string errorInfo = "调用 public static int Update(string sql,SqlParameter[] param)时发生异常" + ex.Message;
                WriteLog(errorInfo);
                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }
        public static object GetSingleResult(string sql,SqlParameter[] param)
        {
            SqlCommand comm = new SqlCommand(sql, conn);
            try
            {
                conn.Open();
                comm.Parameters.AddRange(param);//封装参数
                return comm.ExecuteScalar();
            }
            catch (Exception ex)
            {
                //将异常信息写入日志
                string errorInfo = "调用 public static object GetSingleResult(string sql,SqlParameter[] param)时发生异常" + ex.Message;
                WriteLog(errorInfo);
                throw new Exception(errorInfo);
            }
            finally
            {
                conn.Close();
            }
        }

        //带参出现,返回一个结果集
        public static SqlDataReader Query(string sql,SqlParameter[] param)
        {
            SqlConnection con = new SqlConnection(connString);
            SqlCommand comm = new SqlCommand(sql, con);
           
            try
            {
            con.Open();
            comm.Parameters.AddRange(param);
            return comm.ExecuteReader(CommandBehavior.CloseConnection);

            }

            catch (Exception ex)
            {
                //将异常信息写入日志
                string errorInfo = "调用public static SqlDataReader Query(string sql,SqlParameter[] param)时发生异常" + ex.Message;
                WriteLog(errorInfo);
                throw new Exception(errorInfo);
            }
        }

        /// <summary>
        /// 启用事务提交多条带参数的sql语句
        /// </summary>
        /// <param name="mainSql">主表sql语句</param>
        /// <param name="mainParam">主表sql语句对应的参数</param>
        /// <param name="detailSql">明细表sql语句</param>
        /// <param name="detailParam">明细表sql语句对应的参数数组集合</param>
        /// <returns>返回事务是否执行成功</returns>
        public static bool updateByTran(string mainSql,SqlParameter[] mainParam,string detailSql,List<SqlParameter[]> detailParam )
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;//将连接赋给SqlCommand对象的Connection属性
            try
            {
                conn.Open();
                cmd.Transaction = conn.BeginTransaction();//开启事务
                if (mainSql!=null && mainSql.Length!=0)
                {
                    cmd.CommandText = mainSql;
                    cmd.Parameters.AddRange(mainParam);
                    cmd.ExecuteNonQuery();
                }
                foreach (SqlParameter[] param in detailParam)
                {
                    cmd.CommandText = detailSql;
                    cmd.Parameters.Clear();//每循环一次清除之前的参数
                    cmd.Parameters.AddRange(param);
                    cmd.ExecuteNonQuery();
                }
                cmd.Transaction.Commit();//提交事务
                return true;
                
            }
            catch (Exception ex)
            {
                //如果事务已开启
                if (cmd.Transaction!=null)
                {
                    cmd.Transaction.Rollback();//回滚事务

                }
                //将异常信息写入日志
                string errorInfo = "调用事务处理的方法(又主表和明细表)时发生异常" + ex.Message;
                WriteLog(errorInfo);
                throw new Exception(errorInfo);
            }
            finally
            {
                if (cmd.Transaction!=null)
                {
                    cmd.Transaction = null;//如果有事务那就清楚事务
                }
                conn.Close();
            }
            
        }
        /// <summary>
        /// 启用事务调用带参数的存储过程
        /// </summary>
        /// <param name="procedureName">存储过程名称</param>
        /// <param name="paramArray">存储过程参数数组集合</param>
        /// <returns>返回基于事务的存储过程调用是否成功</returns>
        public static bool updateByTran(string procedureName,List<SqlParameter[]> paramArray)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;//将连接赋给SqlCommand对象的Connection属性
            try
            {
                conn.Open();
                cmd.CommandType = CommandType.StoredProcedure;//声明当前操作是调用存储过程
                cmd.CommandText = procedureName;
                cmd.Transaction = conn.BeginTransaction();//开启事务
              
                foreach (SqlParameter[] param in paramArray)
                {
                   
                    cmd.Parameters.Clear();//每循环一次清除之前的参数
                    cmd.Parameters.AddRange(param);
                    cmd.ExecuteNonQuery();
                }
                cmd.Transaction.Commit();//提交事务
                return true;

            }
            catch (Exception ex)
            {
                //如果事务已开启
                if (cmd.Transaction != null)
                {
                    cmd.Transaction.Rollback();//回滚事务

                }
                //将异常信息写入日志
                string errorInfo = "调用事务处理的方法（调用存储过程）时发生异常" + ex.Message;
                WriteLog(errorInfo);
                throw new Exception(errorInfo);
            }
            finally
            {
                if (cmd.Transaction != null)
                {
                    cmd.Transaction = null;//如果有事务那就清楚事务
                }
                conn.Close();
            }

        }
        #endregion
        #region 封装调用存储过程执行的各种方法
        //带参更新存储过程
        public static int UpdateByProcedure(string spName, SqlParameter[] param)
        {
            SqlCommand comm = new SqlCommand(spName, conn);

            try
            {
                conn.Open();
                comm.CommandType = CommandType.StoredProcedure;//声明当前操作时存储过程
                comm.Parameters.AddRange(param);//封装参数
                return comm.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                //将异常信息写入日志
                string errorInfo = "调用 public static int Update(string sql,SqlParameter[] param)时发生异常" + ex.Message;
                WriteLog(errorInfo);
                throw new Exception(errorInfo);
            }
            finally
            {
                conn.Close();
            }

        } 
        public static object GetsingleResultByProcedure(string spName, SqlParameter[] param)
        {
            SqlCommand comm = new SqlCommand(spName, conn);

            try
            {
                conn.Open();
                comm.CommandType = CommandType.StoredProcedure;//声明当前操作时存储过程
                comm.Parameters.AddRange(param);//封装参数
                return comm.ExecuteScalar();

            }
            catch (Exception ex)
            {
                //将异常信息写入日志
                string errorInfo = "调用 public static object GetsingleResultByProcedure(string spName, SqlParameter[] param)时发生异常" + ex.Message;
                WriteLog(errorInfo);
                throw new Exception(errorInfo);
            }
            finally
            {
                conn.Close();
            }
        }

        

        //查询带参,返回一个结果集，存储过程
        public static SqlDataReader QueryByProcedure(string spName, SqlParameter[] param)
        {
            SqlCommand comm = new SqlCommand(spName, conn);
            try
            {
                conn.Open();
                comm.CommandType = CommandType.StoredProcedure;//声明当前操作时存储过程
                comm.Parameters.AddRange(param);
                return comm.ExecuteReader(CommandBehavior.CloseConnection);

            }
            catch (Exception ex)
            {
                //将异常信息写入日志
                string errorInfo = "调用public static SqlDataReader QueryByProcedure(string spName, SqlParameter[] param)时发生异常" + ex.Message;
                WriteLog(errorInfo);
                throw new Exception(errorInfo);

            }

        }
        #endregion

        #region 其他方法
        private static void WriteLog(string log)
        {
            FileStream fs = new FileStream("sqlhelper.log",FileMode.Append);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(DateTime.Now.ToString() + "  ");
            sw.Close();
            fs.Close();
        }
        #endregion

    }
}
