using System;

namespace LSlicer.BL.Interaction.Contracts
{
    public interface IValidator<T>
    {
        IValidator<T> AddRule(IValidationRule<T> validationRule);
        Reason Validate(T entity);
    }
}
