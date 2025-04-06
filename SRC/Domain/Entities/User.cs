namespace Domain
{
    public class User
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public required string Info { get; set; }
        public required string Email { get; set; }
    }
}
