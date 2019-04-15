using System;
using System.ComponentModel.DataAnnotations;

namespace BillsPaymentSystem.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ExpirationDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime expirationDateTime = (DateTime)value;
            DateTime currentDateTime = DateTime.Now;

            if (currentDateTime > expirationDateTime)
            {
                return new ValidationResult("The credit card is expired!");
            }
            return ValidationResult.Success;
        }

    }
}
