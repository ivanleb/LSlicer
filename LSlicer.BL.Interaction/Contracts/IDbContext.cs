using System.Collections.Generic;

namespace LSlicer.BL.Interaction.Contracts
{
    public interface IDbContext
    {
        List<IAppSettings> AppSettings { get; }
        void SaveChanges();
    }
}
