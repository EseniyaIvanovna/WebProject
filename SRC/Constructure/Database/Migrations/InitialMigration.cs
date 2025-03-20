using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                .WithColumn("email").AsString(255).NotNullable()
                .WithColumn("created_at").AsDateTime().NotNullable();

            Create.Table("posts")
                .WithColumn("id").AsInt32().PrimaryKey().Identity()
                .WithColumn("user_id").AsInt32().NotNullable().ForeignKey("users", "id")
                .WithColumn("text").AsString().NotNullable()
                .WithColumn("created_at").AsDateTime().NotNullable();

            Create.Table("comments")
                .WithColumn("id").AsInt32().PrimaryKey().Identity()
                .WithColumn("user_id").AsInt32().NotNullable().ForeignKey("users", "id")
                .WithColumn("post_id").AsInt32().NotNullable().ForeignKey("posts", "id")
                .WithColumn("text").AsString().NotNullable()
                .WithColumn("created_at").AsDateTime().NotNullable();

            Create.Table("reactions")
                .WithColumn("id").AsInt32().PrimaryKey().Identity()
                .WithColumn("user_id").AsInt32().NotNullable().ForeignKey("users", "id")
                .WithColumn("post_id").AsInt32().NotNullable().ForeignKey("posts", "id")
                .WithColumn("type").AsString(50).NotNullable()
                .WithColumn("created_at").AsDateTime().NotNullable();

            Create.Table("messages")
                .WithColumn("id").AsInt32().PrimaryKey().Identity()
                .WithColumn("sender_id").AsInt32().NotNullable().ForeignKey("users", "id")
                .WithColumn("receiver_id").AsInt32().NotNullable().ForeignKey("users", "id")
                .WithColumn("text").AsString().NotNullable()
                .WithColumn("created_at").AsDateTime().NotNullable();
        }

        public override void Down()
        {
            Delete.Table("messages");
            Delete.Table("reactions");
            Delete.Table("comments");
            Delete.Table("posts");
            Delete.Table("users");
        }
    }
}