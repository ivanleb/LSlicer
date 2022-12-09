namespace LSlicer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addUserDefinedTimeouts : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DbAppSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SlicingEnginePath = c.String(maxLength: 2147483647),
                        SupportEnginePath = c.String(maxLength: 2147483647),
                        WorkingDirectory = c.String(maxLength: 2147483647),
                        SlicingParametersRepoPath = c.String(maxLength: 2147483647),
                        SupportParametersRepoPath = c.String(maxLength: 2147483647),
                        DefaultSlicingParameters = c.String(maxLength: 2147483647),
                        SlicingResultDirectory = c.String(maxLength: 2147483647),
                        DefaultSupportParameters = c.String(maxLength: 2147483647),
                        SupportEngineList = c.String(maxLength: 2147483647),
                        SelectedSupportEngine = c.String(maxLength: 2147483647),
                        SliceEngineList = c.String(maxLength: 2147483647),
                        SelectedSliceEngine = c.String(maxLength: 2147483647),
                        CurrentChangesPath = c.String(maxLength: 2147483647),
                        WaitingUserActionTimeoutTicks = c.Long(nullable: false),
                        AutoSaveIntervalTicks = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Name = c.String(maxLength: 2147483647),
                        PasswordHash = c.String(maxLength: 2147483647),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DbAppSettings", t => t.Id)
                .Index(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Users", "Id", "dbo.DbAppSettings");
            DropIndex("dbo.Users", new[] { "Id" });
            DropTable("dbo.Users");
            DropTable("dbo.DbAppSettings");
        }
    }
}
