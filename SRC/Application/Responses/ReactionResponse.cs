namespace Application.Responses
{
    public class ReactionResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PostId { get; set; }
        public string Type { get; set; }
    }
}
