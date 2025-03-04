using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto
{
    public class MessageDto
    {
        public int Id { get; set; }
        public UserDto Sender { get; set; }
        public UserDto Reciever { get; set; }
        public string Text { get; set; }
        public DateTime DateTime { get; set; }
    }
}
