using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Interaction
    {
        public int Id { get; set; }
        public User User1 { get; set; }
        public User User2 { get; set; }
        public string Status { get; set; } //  выбор из вариантов ответа
    }
}
