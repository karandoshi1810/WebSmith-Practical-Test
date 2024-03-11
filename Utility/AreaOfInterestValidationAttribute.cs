using JobListing.ViewModels;
using System.ComponentModel.DataAnnotations;
namespace JobListing.CustomValidation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class AreaOfInterestValidationAttribute : ValidationAttribute
    {
#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
        protected override ValidationResult? IsValid(object value, ValidationContext validationContext)
#pragma warning restore CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
        {
            var areaOfInterests = (List<AreaOfInterestViewModel>)validationContext.ObjectInstance;

            if (areaOfInterests == null|| areaOfInterests.Count() == 0)
            {
                return new ValidationResult("Please select atleast one area of interest.");
            }
            return ValidationResult.Success;
        }
    }
}
