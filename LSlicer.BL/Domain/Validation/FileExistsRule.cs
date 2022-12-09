using LSlicer.BL.Interaction.Contracts;
using System.IO;

namespace LSlicer.BL.Domain
{
    public class FileExistsRule : IValidationRule<string>
    {
        public string Description => "File does no exist";

        public bool Apply(string entity)
        {
            return new FileInfo(entity).Exists;
        }
    }
}
