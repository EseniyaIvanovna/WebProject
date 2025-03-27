using Domain.Enums;

namespace Domain
{
    public class Interaction
    {
        public int Id { get; set; }
        public int User1Id { get; set; }
        public int User2Id { get; set; }
        public Status Status { get; set; } 
    }
}
