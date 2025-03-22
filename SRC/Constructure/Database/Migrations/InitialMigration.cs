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
                .WithColumn("name").AsString(100).NotNullable()
                .WithColumn("last_name").AsString(100).NotNullable()
                .WithColumn("age").AsString(100).NotNullable()
                .WithColumn("info").AsString(255).NotNullable()
                .WithColumn("email").AsString(255).NotNullable();

            Create.Table("posts")
                .WithColumn("id").AsInt32().PrimaryKey().Identity()
                .WithColumn("user_id").AsInt32().NotNullable().ForeignKey("users", "id")
                .WithColumn("text").AsString().NotNullable()
                .WithColumn("created_at").AsDateTime().NotNullable();

            Create.Table("comments")
                .WithColumn("id").AsInt32().PrimaryKey().Identity()
                .WithColumn("post_id").AsInt32().NotNullable().ForeignKey("users", "id")
                .WithColumn("user_id").AsInt32().NotNullable().ForeignKey("posts", "id")
                .WithColumn("content").AsString().NotNullable()
                .WithColumn("created_at").AsDateTime().NotNullable();

            Create.Table("reactions")
                .WithColumn("id").AsInt32().PrimaryKey().Identity()
                .WithColumn("user_id").AsInt32().NotNullable().ForeignKey("users", "id")
                .WithColumn("post_id").AsInt32().NotNullable().ForeignKey("posts", "id")
                .WithColumn("type").AsString(50).NotNullable();

            Create.Table("messages")
                .WithColumn("id").AsInt32().PrimaryKey().Identity()
                .WithColumn("sender_id").AsInt32().NotNullable().ForeignKey("users", "id")
                .WithColumn("receiver_id").AsInt32().NotNullable().ForeignKey("users", "id")
                .WithColumn("text").AsString().NotNullable()
                .WithColumn("created_at").AsDateTime().NotNullable();

            Create.Table("interactions")
               .WithColumn("id").AsInt32().PrimaryKey().Identity()
               .WithColumn("user1_id").AsInt32().NotNullable().ForeignKey("users", "id")
               .WithColumn("user2_id").AsInt32().NotNullable().ForeignKey("users", "id")
               .WithColumn("status").AsString().NotNullable();

            Insert.IntoTable("users")
                .Row(new { name = "John", last_name = "Doe", age = "30", info = "Software Engineer", email = "john.doe@example.com" })
                .Row(new { name = "Jane", last_name = "Smith", age = "25", info = "Data Scientist", email = "jane.smith@example.com" });

            Insert.IntoTable("posts")
                .Row(new { user_id = 1, text = "First post by John", created_at = DateTime.UtcNow })
                .Row(new { user_id = 2, text = "First post by Jane", created_at = DateTime.UtcNow });

            Insert.IntoTable("comments")
                .Row(new { post_id = 1, user_id = 2, content = "Nice post, John!", created_at = DateTime.UtcNow })
                .Row(new { post_id = 2, user_id = 1, content = "Great job, Jane!", created_at = DateTime.UtcNow });

            Insert.IntoTable("reactions")
                .Row(new { user_id = 1, post_id = 2, type = "like" })
                .Row(new { user_id = 2, post_id = 1, type = "love" });

            Insert.IntoTable("messages")
                .Row(new { sender_id = 1, receiver_id = 2, text = "Hi Jane!", created_at = DateTime.UtcNow })
                .Row(new { sender_id = 2, receiver_id = 1, text = "Hello John!", created_at = DateTime.UtcNow });

            Insert.IntoTable("interactions")
                .Row(new { user1_id = 1, user2_id = 2, status = "friends" });
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