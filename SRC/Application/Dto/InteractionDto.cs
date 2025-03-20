using Domain.Enums;

namespace Application.Dto
{
    public class InteractionDto
    {
        public int Id { get; set; }
        public int User1Id { get; set; }
        public int User2Id { get; set; }
        public Status Status { get; set; }
    }
}
