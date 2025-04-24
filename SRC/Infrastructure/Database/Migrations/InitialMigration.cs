using FluentMigrator;

namespace Infrastructure.Database.Migrations
{
    [Migration(202503151112)]
    public class InitialMigration : Migration
    {
        public override void Up()
        {
            Create.Table("users")
                .WithColumn("id").AsInt32().PrimaryKey().Identity()
                .WithColumn("name").AsString(ValidationConstants.MaxNameLength).NotNullable()
                .WithColumn("last_name").AsString(ValidationConstants.MaxLastNameLength).NotNullable()
                .WithColumn("date_of_birth").AsDateTime().NotNullable()
                .WithColumn("info").AsString(ValidationConstants.MaxUserInfoLength)
                .WithColumn("email").AsString(ValidationConstants.MaxEmailLength).NotNullable();

            Create.Table("posts")
                .WithColumn("id").AsInt32().PrimaryKey().Identity()
                .WithColumn("user_id").AsInt32().NotNullable().ForeignKey("users", "id")
                .WithColumn("text").AsString(ValidationConstants.MaxTextContentLength).NotNullable()
                .WithColumn("created_at").AsDateTime().NotNullable();

            Create.Table("comments")
                .WithColumn("id").AsInt32().PrimaryKey().Identity()
                .WithColumn("post_id").AsInt32().NotNullable().ForeignKey("posts", "id")
                .WithColumn("user_id").AsInt32().NotNullable().ForeignKey("users", "id")
                .WithColumn("content").AsString(ValidationConstants.MaxTextContentLength).NotNullable()
                .WithColumn("created_at").AsDateTime().NotNullable();

            Create.Table("reactions")
                .WithColumn("id").AsInt32().PrimaryKey().Identity()
                .WithColumn("user_id").AsInt32().NotNullable().ForeignKey("users", "id")
                .WithColumn("postd").AsInt32().NotNullable().ForeignKey("posts", "id")
                .WithColumn("type").AsString(ValidationConstants.MaxReactionTypeLength).NotNullable();

            Create.Table("messages")
                .WithColumn("id").AsInt32().PrimaryKey().Identity()
                .WithColumn("sender_id").AsInt32().NotNullable().ForeignKey("users", "id")
                .WithColumn("receiver_id").AsInt32().NotNullable().ForeignKey("users", "id")
                .WithColumn("text").AsString(ValidationConstants.MaxTextContentLength).NotNullable()
                .WithColumn("created_at").AsDateTime().NotNullable();

            Create.Table("interactions")
               .WithColumn("id").AsInt32().PrimaryKey().Identity()
               .WithColumn("user1_id").AsInt32().NotNullable().ForeignKey("users", "id")
               .WithColumn("user2Id").AsInt32().NotNullable().ForeignKey("users", "id")
               .WithColumn("status").AsString().NotNullable();

            Insert.IntoTable("users")
                .Row(new { name = "John", last_name = "Doe", date_of_birth = "2000-01-01", info = "Software Engineer", email = "john.doe@example.com" })
                .Row(new { name = "Jane", last_name = "Smith", date_of_birth = "2003-10-01", info = "Data Scientist", email = "jane.smith@example.com" });

            Insert.IntoTable("posts")
                .Row(new { userId = 1, text = "First post by John", created_at = DateTime.UtcNow })
                .Row(new { userId = 2, text = "First post by Jane", created_at = DateTime.UtcNow });

            Insert.IntoTable("comments")
                .Row(new { postId = 1, userId = 2, content = "Nice post, John!", created_at = DateTime.UtcNow })
                .Row(new { postId = 2, userId = 1, content = "Great job, Jane!", created_at = DateTime.UtcNow });

            Insert.IntoTable("reactions")
                .Row(new { userId = 1, postId = 2, type = "Like" })
                .Row(new { userId = 2, postId = 1, type = "Dislike" });

            Insert.IntoTable("messages")
                .Row(new { senderId = 1, receiverId = 2, text = "Hi Jane!", created_at = DateTime.UtcNow })
                .Row(new { senderId = 2, receiverId = 1, text = "Hello John!", created_at = DateTime.UtcNow });

            Insert.IntoTable("interactions")
                .Row(new { user1Id = 1, user2Id = 2, status = "Friend" });
        }

        public override void Down()
        {
            Delete.Table("interactions");
            Delete.Table("messages");
            Delete.Table("reactions");
            Delete.Table("comments");
            Delete.Table("posts");
            Delete.Table("users");
        }
    }
}