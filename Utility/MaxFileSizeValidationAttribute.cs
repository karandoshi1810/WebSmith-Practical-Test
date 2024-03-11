using System.ComponentModel.DataAnnotations;
using System.Drawing.Text;

namespace JobListing.CustomValidation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    sealed class MaxFileSizeValidationAttribute : ValidationAttribute
    {
        private readonly int _maxFileSize;
        public MaxFileSizeValidationAttribute(int maxFileSize)
        {
            _maxFileSize = maxFileSize;
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            if (file != null)
            {
                if (file.Length > _maxFileSize)
                {
                    return  new ValidationResult("File size should not be greate than " + _maxFileSize.ToString());
                }
            }
            if (file == null)
            {
                return new ValidationResult("Please upload a file for resume");
            }
            return ValidationResult.Success;
        }
    }
}
