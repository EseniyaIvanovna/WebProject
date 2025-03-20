namespace Application.Dto
{
    public class PostDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
