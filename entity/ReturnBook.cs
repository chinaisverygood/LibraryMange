using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entity
{
    [Serializable]
    public class ReturnBook
    {
        public int ReturnID { get; set; }
        public int BorrowDetailId { get; set; }
        public int  ReturnCount { get; set; }
        public DateTime ReturnDate { get; set; }
        public string  AdimName_R { get; set; }
        
    }
}
