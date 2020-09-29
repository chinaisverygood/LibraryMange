using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entity
{
    [Serializable]
    public   class BorrowInfo
    {
        public string BorrowId { get; set; }
        public int ReaderId { get; set; }
        public DateTime BorrowDate { get; set; }
        public string  AdminName_B { get; set; }

    }
}
