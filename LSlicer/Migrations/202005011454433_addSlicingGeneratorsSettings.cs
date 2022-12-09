namespace LSlicer.Implementations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addSlicingGeneratorsSettings : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DbAppSettings", "SliceEngineList", c => c.String(maxLength: 2147483647));
            AddColumn("dbo.DbAppSettings", "SelectedSliceEngine", c => c.String(maxLength: 2147483647));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DbAppSettings", "SelectedSliceEngine");
            DropColumn("dbo.DbAppSettings", "SliceEngineList");
        }
    }
}
