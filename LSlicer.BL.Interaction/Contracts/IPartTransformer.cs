using LSlicer.BL.Interaction.Types;

namespace LSlicer.BL.Interaction
{
    public interface IPartTransformer 
    {
        void Transform(ModelOnViewTransformSpec spec);
    }
}
