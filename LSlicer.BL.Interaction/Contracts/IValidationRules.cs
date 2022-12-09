using System;

namespace LSlicer.BL.Interaction.Contracts
{
    public interface IValidationRule<in T>
    {
        Boolean Apply(T entity);
        String Description { get; }
    }
}