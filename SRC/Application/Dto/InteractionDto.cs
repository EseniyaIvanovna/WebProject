using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto
{
    public class InteractionDto
    {
        public int Id { get; set; }
        public UserDto User1 { get; set; }
        public UserDto User2 { get; set; }
        public Status Status { get; set; }
    }
}
