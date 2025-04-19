using Domain.Enums;

namespace Domain
{
    public class Reaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PostId { get; set; }
        public ReactionType Type { get; set; }
    }
}
