namespace Application.Responses
{
    public class PostResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public required string Text { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
