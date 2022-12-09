using LSlicer.BL.Interaction;

namespace UtilModule
{
    public class UserIdentityModel : IUserIdentity
    {
        public string Name { get; set; }
        public string PasswordHash { get; set; }
    }
}
