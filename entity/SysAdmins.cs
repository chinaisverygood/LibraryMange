using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entity
{
    [Serializable]
    public class SysAdmins
    {
        public int AdminId { get; set; }
        public string  AdminName { get; set; }
        public string  LoginPwd { get; set; }
        public int  RoleId  { get; set; }
        public int StatusId { get; set; }
    }
}
