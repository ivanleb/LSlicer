namespace LSlicer.Migrations
{
    using LSlicer.BL.Interaction;
    using LSlicer.Implementations;
    using System.Data.Entity.Migrations;
    //using System.Data.SQLite.EF6.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<AppSettingsSQLiteContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            //SetSqlGenerator("System.Data.SQLite", new SQLiteMigrationSqlGenerator());
        }
        protected override void Seed(AppSettingsSQLiteContext context)
        {
            IAppSettings settings = AppSettingsResourceFile.DefaultValue;
            DbAppSettings defaultSettings = new DbAppSettings();
            defaultSettings.CopyFrom(settings);

            context.SaveChanges();
        }
    }
}
