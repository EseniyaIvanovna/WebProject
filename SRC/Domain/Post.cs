using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Post
    {
        public int Id { get; set; }
        public User User { get; set; }
        public string Text { get; set; }
        public DateTime dateTime { get; set; }
    }
}
