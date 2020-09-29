using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using entity;
using DButil;
using System.Data.SqlClient;
using System.Data;

namespace dao
{
    /// <summary>
    /// 读者数据层访问类
    /// </summary>
    public class ReaderDao
    {
        //会员办证(添加读者信息)
        public int AddReader(Readers reader)
        {
            string sql = "insert into Readers(ReadingCard,ReaderName,Gender,IDCard,ReaderAddress,PostCode,PhoneNumber,RoleId,ReaderImage)";
            sql += " values(@ReadingCard,@ReaderName,@Gender,@IDCard,@ReaderAddress,@PostCode,@PhoneNumber,@RoleId,@ReaderImage)";

            SqlParameter[] param = new SqlParameter[]
            {
                new SqlParameter("@ReadingCard",reader.ReadingCard),
                new SqlParameter("@ReaderName",reader.ReaderName),
                new SqlParameter("@Gender",reader.Gender),
                new SqlParameter("@IDCard",reader.IDCard),
                new SqlParameter("@ReaderAddress",reader.ReaderAddress),
                new SqlParameter("@PostCode",reader.PostCode),
                new SqlParameter("@PhoneNumber",reader.PhoneNumber),
                new SqlParameter("@RoleId",reader.RoleId),
                new SqlParameter("@ReaderImage",reader.ReaderImage),
                
            };
            return SqlDB.Update(sql, param);
        }

        //修改读者信息
        public int EditReader(Readers reader)
        {
            string sql = "update Readers set ReaderName=@ReaderName,Gender=@Gender,ReaderAddress=@ReaderAddress,PostCode=@PostCode,PhoneNumber=@PhoneNumber,ReaderImage=@ReaderImage,RoleId=@RoleId ";
            sql += " where ReaderId=@ReaderId";

            SqlParameter[] param = new SqlParameter[] {

                new SqlParameter("@ReaderName",reader.ReaderName),
                new SqlParameter("@Gender",reader.Gender),
                new SqlParameter("@ReaderAddress",reader.ReaderAddress),
                new SqlParameter("@PostCode",reader.PostCode),
                new SqlParameter("@PhoneNumber",reader.PhoneNumber),
                new SqlParameter("@RoleId",reader.RoleId),
                new SqlParameter("@ReaderImage",reader.ReaderImage),
                new SqlParameter("@ReaderId",reader.ReaderId)

            };

            return SqlDB.Update(sql, param);
        }

        //借阅证挂失(修改读者状态)
        public int ForbiddenReaderCard(string readerId)
        {
            string sql = "update Readers set StatusId=0 where ReaderId=@ReaderId";
            SqlParameter[] param = new SqlParameter[] {
            new SqlParameter("@ReaderId",readerId)
            };
            return SqlDB.Update(sql, param);
        }

        //查询全部会员角色
        public DataTable GetAllReaderRole()
        {
            string sql = "select RoleId,RoleName from ReaderRoles";
            return SqlDB.getDataSet(sql).Tables[0];
        }
        //根据借阅证号查询读者信息
        public Readers GetReaderByReadingCard(string readingCard)
        {
            string sql = " where ReadingCard=@ReadingCard";
            return GetReader(sql,new SqlParameter[] {new SqlParameter("@ReadingCard", readingCard)});

        }

        //根据身份证号查询读者信息
        public Readers GetReaderByIDCard(string idCard)
        {
            string sql = " where IDCard=@IDCard";
            return GetReader(sql,new SqlParameter[] { new SqlParameter("@IDCard",idCard)});
        }

        /// <summary>
        /// 根据某某查询读者信息
        /// </summary>
        /// <param name="whereSql">条件sql语句</param>
        /// <param name="param">参数</param>
        /// <returns>返回查询得到的结果</returns>
        private Readers GetReader(string whereSql,SqlParameter[] param)
        {
            string sql = "select ReaderId,ReadingCard,ReaderName,Gender,IDCard,ReaderAddress,PostCode,PhoneNumber,Readers.RoleId,ReaderImage,StatusId,AllowDay,AllowCounts,ReaderRoles.RoleName,Readers.Gender from Readers";
            sql += " inner join ReaderRoles on ReaderRoles.RoleId=Readers.RoleId";
            sql +=whereSql;
            SqlDataReader objReade= SqlDB.Query(sql,param);
            Readers reader = null;
            if (objReade.Read())
            {
                reader = new Readers()
                {
                    ReaderId = Convert.ToInt32(objReade["ReaderId"]),
                    ReadingCard = objReade["ReadingCard"].ToString(),
                    RoleId = Convert.ToInt32(objReade["RoleId"]),
                    RoleName = objReade["RoleName"].ToString(),
                    ReaderImage = objReade["ReaderImage"] is DBNull ? "" : objReade["ReaderImage"].ToString(),
                    StatusId = Convert.ToInt32(objReade["StatusId"]),
                    PhoneNumber = objReade["PhoneNumber"].ToString(),
                    ReaderAddress = objReade["ReaderAddress"].ToString(),
                    PostCode = objReade["PostCode"].ToString(),
                    Gender = objReade["Gender"].ToString(),
                    ReaderName = objReade["ReaderName"].ToString(),
                    AllowDay = Convert.ToInt32(objReade["AllowDay"]),
                    AllowCounts = Convert.ToInt32(objReade["AllowCounts"]),
                };
            }
            objReade.Close();
            return reader;
        }

        //根据身会员角色查询读者信息（同时找到对应该角色的会员总数）
        public List<Readers> GetReaderByRole(string roleId,out int readerCount)
        {
            string sql = "select ReaderId,ReadingCard,ReaderName,Gender,PostCode,PhoneNumber,ReaderAddress,RegTime,StatusId from Readers";
            sql += " where RoleId=@RoleId;";
            sql += "select readerCount=count(*) from Readers where RoleId=@RoleId";
            List<Readers> readerList = new List<Readers>();
            SqlParameter[] param = new SqlParameter[] {
            new SqlParameter("@RoleId",roleId),
            };
            SqlDataReader objReader= SqlDB.Query(roleId,param);

            while (objReader.Read())
            {
                readerList.Add(new Readers()
                {
                    ReaderId = Convert.ToInt32(objReader["ReaderId"]),
                    RegTime = Convert.ToDateTime(objReader["RegTime"]),
                    ReadingCard = objReader["ReadingCard"].ToString(),
                    StatusId = Convert.ToInt32(objReader["StatusId"]),
                    PhoneNumber = objReader["PhoneNumber"].ToString(),
                    ReaderAddress = objReader["ReaderAddress"].ToString(),
                    PostCode = objReader["PostCode"].ToString(),
                    Gender = objReader["Render"].ToString(),
                    ReaderName = objReader["ReaderName"].ToString()

                });
            }
            if (objReader.NextResult())//跳转到下一个结果集
            {
                if (objReader.Read())
                {
                    readerCount = Convert.ToInt32(objReader["readerCount"]);

                }
                else
                {
                    readerCount = 0;
                }
            }
            else
            {
                readerCount = 0;
            }
            objReader.Close();
            return readerList;
        }
    }
}
