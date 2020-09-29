using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entity
{
    [Serializable]
    public  class Readers
    {
        public int ReaderId { get; set; }
        public string ReadingCard { get; set; }
        public string  ReaderName { get; set; }
        public string Gender { get; set; }
        public string  IDCard { get; set; }
        public string ReaderAddress { get; set; }
        public string  PostCode { get; set; }
        public string PhoneNumber { get; set; }
        public int RoleId { get; set; }//角色编号
        public string ReaderImage { get; set; }
        public DateTime RegTime{ get; set; }
        public string ReaderPwd { get; set; }
        public int AdminId { get; set; }
        public int StatusId { get; set; }

        //扩展属性
        public string  RoleName { get; set; }//读者角色名称

        public string StatusDesc { get; set; }//账号状态描述

        public int AllowDay { get; set; }//当前角色允许借书天数
        public int AllowCounts { get; set; }//当前角色允许借书总数



    }
}
