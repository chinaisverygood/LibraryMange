using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entity
{
    [Serializable]
  public  class Books
    {
        public int bookId { get; set; }
        public string barCode  { get; set; }
        public string BookName { get; set; }
        public string  author { get; set; }
        public int  publisherId { get; set; }
        public DateTime publishDate { get; set; }
        public int bookCategory { get; set; }
        public double unitPrice { get; set; }
        public string  bookImage { get; set; }
        public int BookCount { get; set; }
        public string bookPosition { get; set; }
        public DateTime RegTime { get; set; }
        public int Remainder { get; set; }

        //扩展属性
        public string   PublisherName { get; set; }
        public string  CategoryName { get; set; }
    }
}
