using LSlicer.BL.Interaction.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LSlicer.BL.Interaction.Abstracts
{
    public abstract class BaseValidator<T> : IValidator<T>
    {
        private readonly List<IValidationRule<T>> _rules = new List<IValidationRule<T>>();
        public IValidator<T> AddRule(IValidationRule<T> validationRule)
        {
            _rules.Add(validationRule);
            return this;
        }

        public IValidator<T> AddRule(Func<T, Boolean> validationFunc)
        {
            BaseValidationRule validationRule = validationFunc;
            validationRule.Description = validationFunc.Method.Name;
            _rules.Add(validationRule);
            return this;
        }

        public Reason Validate(T entity)
        {
            foreach (var rule in _rules)
            {
                if (!rule.Apply(entity))
                {
                    return new Reason(rule.Description);
                }
            }
            return new Reason();
        }

        private class BaseValidationRule : IValidationRule<T>
        {
            private readonly Func<T, Boolean> _validationFunc;

            public BaseValidationRule(Func<T, Boolean> validationFunc)
            {
                _validationFunc = validationFunc;
            }

            public string Description { get; set; }

            public bool Apply(T entity)
            {
                return _validationFunc != null ? _validationFunc(entity) : false;
            }

            public static implicit operator BaseValidationRule(Func<T, Boolean> validationFunc) 
                => new BaseValidationRule(validationFunc);
        }
    }
}
