using LSlicer.Data.Model;
using LSlicer.Data.Interaction.Contracts;
using System.IO;

namespace LSupportLibrary
{
    public static class CustomSupportParametersReader
    {
        public static ISupportParameters Read(FileInfo file) 
        {
            //TODO: вставить чтение из файла сюда
            return GetDefault();
        }

        private static ISupportParameters GetDefault() 
        {
            return new SupportParameters();
        }
    }
}
