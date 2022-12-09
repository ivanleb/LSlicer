using LSlicer.BL.Interaction;

namespace LSlicer.BL
{
    public class TaskSpec : ITaskSpec
    {
        public TaskSpec(string filePath, string resultFilePath, int partId)
        {
            FilePath = filePath;
            ResultFilePath = resultFilePath;
            PartId = partId;
        }

        public string FilePath { get; set; }
        public string ResultFilePath { get; set; }
        public int PartId { get; set; }
    }
}
