using LSlicer.BL.Interaction.Contracts;
using System.IO;

namespace LSlicer.BL.Domain
{
    public class FileIsNotEmpty : IValidationRule<string>
    {
        private long _minSizeBytes;

        public FileIsNotEmpty(long minSizeBytes = 1024)
        {
            _minSizeBytes = minSizeBytes;
        }

        public string Description => "File is empty";

        public bool Apply(string entity)
        {
            return new FileInfo(entity).Length > _minSizeBytes;
        }
    }
}
