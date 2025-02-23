using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Message
    {
        public int Id { get; set; }
        public User Sender { get; set; }
        public User Recipient { get; set; }
        public string Text { get; set; }
        public DateTime DateTime { get; set; }
    }
}
