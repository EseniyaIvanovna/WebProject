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
                .WithColumn("lastname").AsString(ValidationConstants.MaxLastNameLength).NotNullable()
                .WithColumn("dateofbirth").AsInt32().NotNullable()
                .WithColumn("info").AsString(ValidationConstants.MaxUserInfoLength)
                .WithColumn("email").AsString(ValidationConstants.MaxEmailLength).NotNullable();

            Create.Table("posts")
                .WithColumn("id").AsInt32().PrimaryKey().Identity()
                .WithColumn("userid").AsInt32().NotNullable().ForeignKey("users", "id")
                .WithColumn("text").AsString(ValidationConstants.MaxTextContentLength).NotNullable()
                .WithColumn("createdat").AsDateTime().NotNullable();

            Create.Table("comments")
                .WithColumn("id").AsInt32().PrimaryKey().Identity()
                .WithColumn("postid").AsInt32().NotNullable().ForeignKey("posts", "id")
                .WithColumn("userid").AsInt32().NotNullable().ForeignKey("users", "id")
                .WithColumn("content").AsString(ValidationConstants.MaxTextContentLength).NotNullable()
                .WithColumn("createdat").AsDateTime().NotNullable();

            Create.Table("reactions")
                .WithColumn("id").AsInt32().PrimaryKey().Identity()
                .WithColumn("userid").AsInt32().NotNullable().ForeignKey("users", "id")
                .WithColumn("postid").AsInt32().NotNullable().ForeignKey("posts", "id")
                .WithColumn("type").AsString(ValidationConstants.MaxReactionTypeLength).NotNullable();

            Create.Table("messages")
                .WithColumn("id").AsInt32().PrimaryKey().Identity()
                .WithColumn("senderid").AsInt32().NotNullable().ForeignKey("users", "id")
                .WithColumn("receiverid").AsInt32().NotNullable().ForeignKey("users", "id")
                .WithColumn("text").AsString(ValidationConstants.MaxTextContentLength).NotNullable()
                .WithColumn("createdat").AsDateTime().NotNullable();

            Create.Table("interactions")
               .WithColumn("id").AsInt32().PrimaryKey().Identity()
               .WithColumn("user1id").AsInt32().NotNullable().ForeignKey("users", "id")
               .WithColumn("user2id").AsInt32().NotNullable().ForeignKey("users", "id")
               .WithColumn("status").AsString().NotNullable();

            Insert.IntoTable("users")
                .Row(new { name = "John", lastname = "Doe", dateofbirth = "2000-01-01", info = "Software Engineer", email = "john.doe@example.com" })
                .Row(new { name = "Jane", lastname = "Smith", dateofbirth = "2003-10-01", info = "Data Scientist", email = "jane.smith@example.com" });

            Insert.IntoTable("posts")
                .Row(new { userid = 1, text = "First post by John", createdat = DateTime.UtcNow })
                .Row(new { userid = 2, text = "First post by Jane", createdat = DateTime.UtcNow });

            Insert.IntoTable("comments")
                .Row(new { postid = 1, userid = 2, content = "Nice post, John!", createdat = DateTime.UtcNow })
                .Row(new { postid = 2, userid = 1, content = "Great job, Jane!", createdat = DateTime.UtcNow });

            Insert.IntoTable("reactions")
                .Row(new { userid = 1, postid = 2, type = "Like" })
                .Row(new { userid = 2, postid = 1, type = "Dislike" });

            Insert.IntoTable("messages")
                .Row(new { senderid = 1, receiverid = 2, text = "Hi Jane!", createdat = DateTime.UtcNow })
                .Row(new { senderid = 2, receiverid = 1, text = "Hello John!", createdat = DateTime.UtcNow });

            Insert.IntoTable("interactions")
                .Row(new { user1id = 1, user2id = 2, status = "Friend" });
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