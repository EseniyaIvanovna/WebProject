using Domain.Enums;

namespace Application.Responses
{
    public class InteractionResponse
    {
        public int Id { get; set; }
        public int User1Id { get; set; }
        public int User2Id { get; set; }
        public Status Status { get; set; }
    }
}
