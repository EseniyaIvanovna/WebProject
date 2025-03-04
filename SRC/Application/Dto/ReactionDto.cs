using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto
{
    public class ReactionDto
    {
        public int Id { get; set; }
        public UserDto User { get; set; }
        public PostDto Post { get; set; }
        public ReactionType Type { get; set; }
    }
}
