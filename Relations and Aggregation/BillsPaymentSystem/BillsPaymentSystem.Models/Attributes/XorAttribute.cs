using System;
using System.ComponentModel.DataAnnotations;

namespace BillsPaymentSystem.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class XorAttribute : ValidationAttribute
    {
        private string targetPropertyName;

        public XorAttribute(string targetPropertyName)
        {
            this.targetPropertyName = targetPropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var targetValue = validationContext.ObjectType
                .GetProperty(this.targetPropertyName)
                .GetValue(validationContext.ObjectInstance);

            if((value == null) ^ (targetValue == null))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("The two properties must have opposite values!");
        }
    }
}
