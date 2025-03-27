using Domain.Enums;

namespace Application.Dto
{
    public class ReactionDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PostId { get; set; }
        public ReactionType Type { get; set; }
    }
}
