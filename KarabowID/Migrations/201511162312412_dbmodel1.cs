namespace KarabowID.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dbmodel1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Address", c => c.String());
            DropColumn("dbo.AspNetUsers", "Address");
        }

        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Address");
        }
    }
}
