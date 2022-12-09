namespace LSlicer.BL.Interaction
{
    public interface IUserIdentity
    {
        string Name { get; set; }
        string PasswordHash { get; set; }
    }
}
