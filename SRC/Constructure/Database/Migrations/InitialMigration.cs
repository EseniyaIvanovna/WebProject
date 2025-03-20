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