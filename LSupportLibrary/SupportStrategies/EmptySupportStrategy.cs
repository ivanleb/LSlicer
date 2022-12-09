using LSlicer.Data.Interaction;

namespace LSupportLibrary
{
    public class EmptySupportStrategy : IMakeSupportStrategy
    {
        public void ApplyTransform(IPartTransform partTransform)
        {
            //do nothing
        }

        public IPart[] GenerateSupports(IPart part, string  supportPath)
        {
            return new IPart[0];
        }

        public void Load(IPart part)
        {
            //do nothing
        }

        public void Prepare(IPart part)
        {
            //do nothing
        }

        public void WriteSlicedSupports(string filePath)
        {
            //do nothing
        }

        public void WriteToFile(string filePath)
        {
            //do nothing
        }
    }
}
