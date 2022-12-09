using LSlicer.BL.Interaction.Abstracts;

namespace LSlicer.BL.Domain
{
    public class StlFileValidator : BaseValidator<string>
    {
        public StlFileValidator():base()
        {
            AddRule(new FileExistsRule());
            AddRule(new FileIsNotEmpty(100));
        }
    }
}
