using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace JobListing.CustomValidation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]

    sealed class AllowedFileExtensionValidationAttribute : ValidationAttribute
    {
        private readonly string[] _extension;
        public AllowedFileExtensionValidationAttribute(string[] extension)
        {
            _extension = extension;
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            if (file != null)
            {
                var uploadedFileExtension = Path.GetExtension(file.FileName);
                if (!_extension.Contains(uploadedFileExtension.ToLower()))
                {
                    return new ValidationResult("Please select a .pdf file");
                }
            }
            if(file == null)
            {
                return new ValidationResult("Please upload a file for resume");
            }
            return ValidationResult.Success;
        }
    }
}
