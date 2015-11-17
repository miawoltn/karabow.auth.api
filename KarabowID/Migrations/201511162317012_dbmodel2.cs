namespace KarabowID.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dbmodel2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Address", c => c.String());
        }
        
        public override void Down()
        {
        }
    }
}
