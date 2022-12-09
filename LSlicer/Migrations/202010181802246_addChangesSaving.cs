namespace LSlicer.Implementations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addChangesSaving : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DbAppSettings", "CurrentChangesPath", c => c.String(maxLength: 2147483647));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DbAppSettings", "CurrentChangesPath");
        }
    }
}
