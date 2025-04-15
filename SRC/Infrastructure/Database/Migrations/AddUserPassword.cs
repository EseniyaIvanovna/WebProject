using FluentMigrator;

namespace Infrastructure.Database.Migrations
{
    [Migration(202504121100)]
    public class  AddUserPassword
    {
        public override void Up()
        {
            Execute.Sql("Create type userRole as ENUM ('User', 'Admon')");
            Alter.Table("users")
                .AddColumn("passwordHash"). //+роль поле для юзера
        }

        public override void Down()
        {
            Delete.Column("passworHash")
        }
    }
}
