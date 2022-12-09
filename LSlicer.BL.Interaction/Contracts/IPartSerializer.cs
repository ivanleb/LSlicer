using LSlicer.Data.Interaction;
using LSlicer.Data.Model;

namespace LSlicer.BL.Interaction
{
    public interface IPartSerializer
    {
        string Serialize(PartDataForSave[] dataForSave);
        PartDataForSave[] Deserialize(string data);
    }
}
