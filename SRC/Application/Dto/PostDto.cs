using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto
{
    public class PostDto
    {
        public int Id { get; set; }
        public UserDto User { get; set; }
        public string Text { get; set; }
        public DateTime dateTime { get; set; }
    }
}
