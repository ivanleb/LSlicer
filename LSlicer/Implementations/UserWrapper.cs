using LSlicer.BL.Interaction;
using LSlicer.BL.Interaction.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSlicer.Implementations
{
    public class UserWrapper : IUserIdentity
    {
        private User _user;

        public UserWrapper(User user)
        {
            _user = user;
        }

        public string Name { get => _user.Name; set => _user.Name = value; }
        public string PasswordHash { get => _user.PasswordHash; set => _user.PasswordHash = value; }
    }
}
