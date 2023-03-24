using System;
using System.Data.Entity;
using System.Linq;

namespace LSlicer.Implementations
{
    public class UserIdentityController
    {
        private readonly AppSettingsSQLiteContext _context;

        public UserIdentityController(AppSettingsSQLiteContext context)
        {
            _context = context;
        }

        public User Get(int? id) 
        {
            return id == null ? _context.Users.First() : _context.Users.FirstOrDefault(x => x.Id == id);
        }

        public User GetByName(string name)
        {
            return _context.Users.FirstOrDefault(x => x.Name == name);
        }

        public Boolean Update(User user) 
        {
            var entry = _context.Entry(user);
            if (entry.Entity == null) return false;
            entry.CurrentValues.SetValues(user);
            entry.State = EntityState.Modified;
            _context.SaveChangesAsync();
            return true;
        }

        public Boolean Create(string name, string pswHash) 
        {
            var defaultSettings = _context.Settings.FirstOrDefault();
            if (_context.Users.Count() == 0)
            {
                User firstUser = new User { Name = name, PasswordHash = pswHash, Settings = defaultSettings };
                _context.Users.Add(firstUser);
                _context.SaveChanges();
                return true;
            }

            var theSame = _context.Users.FirstOrDefault(x => x.Name == name);
            if (theSame != null) return false;

            DbAppSettings settings = (DbAppSettings)defaultSettings.Clone();
            var settingsEntry = _context.Settings.Add(settings);
            User user = new User {Name = name, PasswordHash = pswHash, Settings = settingsEntry };
            _context.Users.Add(user);
            _context.SaveChanges();
            return true;
        }

        public Boolean Delete(int? id) 
        {
            if (id == null) return false;
            var user = _context.Users.FirstOrDefault(x => x.Id == id);
            if (user == null) return false;
            _context.Users.Remove(user);
            _context.SaveChanges();
            return true;
        }
    }
}
