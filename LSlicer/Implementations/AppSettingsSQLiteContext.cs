using LSlicer.BL.Interaction;
using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.SQLite;
using System.Data.SQLite.EF6;
//using System.Data.SQLite.EF6.Migrations;

namespace LSlicer.Implementations
{
    public class AppSettingsSQLiteContext : DbContext
    {
        public DbSet<DbAppSettings> Settings { get; set; }
        public DbSet<User> Users { get; set; }

        public AppSettingsSQLiteContext() : base("name=DefaultConnection")
        {
            DbConfiguration.SetConfiguration(new SQLiteLoggedConfiguration());
        }

        public AppSettingsSQLiteContext(ILoggerService logger) : base("name=DefaultConnection")
        {
            DbConfiguration.SetConfiguration(new SQLiteLoggedConfiguration(logger));
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<DbAppSettings>().HasKey(i => i.Id)
                .HasRequired(i => i.User)
                .WithRequiredPrincipal(i => i.Settings);
            modelBuilder.Entity<User>().HasKey(i => i.Id);
        }
    }

    public class SQLiteLoggedConfiguration : DbConfiguration
    {
        public SQLiteLoggedConfiguration()
        {
            SetProviderFactory("System.Data.SQLite", SQLiteFactory.Instance);
            SetProviderFactory("System.Data.SQLite.EF6", SQLiteProviderFactory.Instance);
            SetProviderServices("System.Data.SQLite", (DbProviderServices)SQLiteProviderFactory.Instance.GetService(typeof(DbProviderServices)));
        }
        public SQLiteLoggedConfiguration(ILoggerService logger)
        {
            logger.Info($"[{nameof(SQLiteLoggedConfiguration)}] \"System.Data.SQLite\" -> {SQLiteFactory.Instance.ToString()}");
            SetProviderFactory("System.Data.SQLite", SQLiteFactory.Instance);
            logger.Info($"[{nameof(SQLiteLoggedConfiguration)}] \"System.Data.SQLite.EF6\" -> {SQLiteProviderFactory.Instance.ToString()}");
            SetProviderFactory("System.Data.SQLite.EF6", SQLiteProviderFactory.Instance);
            logger.Info($"[{nameof(SQLiteLoggedConfiguration)}] \"System.Data.SQLite\" -> {nameof(DbProviderServices)}");
            SetProviderServices("System.Data.SQLite", (DbProviderServices)SQLiteProviderFactory.Instance.GetService(typeof(DbProviderServices)));
        }
    }

    public class AppSettingsContextFactory : IDbContextFactory<AppSettingsSQLiteContext>
    {
        public AppSettingsSQLiteContext Create()
        {
            return new AppSettingsSQLiteContext();
        }
    }


    //Moved to Migrations/Configuration.cs, saved here for backup
    //internal sealed class ContextMigrationConfiguration : DbMigrationsConfiguration<AppSettingsContext>
    //{
    //    public ContextMigrationConfiguration()
    //    {
    //        AutomaticMigrationsEnabled = true;
    //        AutomaticMigrationDataLossAllowed = true;
    //        SetSqlGenerator("System.Data.SQLite", new SQLiteMigrationSqlGenerator());
    //    }
    //    protected override void Seed(AppSettingsContext context)
    //    {
    //        IAppSettings settings = AppSettings.DefaultValue;
    //        DbAppSettings defaultSettings = new DbAppSettings();
    //        defaultSettings.CopyFrom(settings);

    //        var entrySettings = context.Settings.Add(defaultSettings);

    //        context.Users.Add(new User { Name = "default", PasswordHash = "000000".GetHashCode().ToString(), Settings = entrySettings });

    //        context.SaveChanges();
    //    }
    //}
}
