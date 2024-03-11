using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using JobListing.ViewModels;
using JobListing.CustomValidation;

namespace JobListing.Models
{
    public class JobApplication
    {
        public int ApplicationId { get; set; }

        [Required(ErrorMessage = "Please enter first name")]
        [MaxLength(50,ErrorMessage = "Maximum 50 characters can be entered")]
        [DisplayName("First Name")]
        [DataType(DataType.Text)]
        public string? FirstName {  get; set; }

        [Required(ErrorMessage = "Please enter last name")]
        [MaxLength(50, ErrorMessage = "Maximum 50 characters can be entered")]
        [DisplayName("Last Name")]
        [DataType(DataType.Text)]
        public string? LastName { get; set;}

        [Required(ErrorMessage = "Please enter contact number")]
        //[MaxLength(10, ErrorMessage = "Maximum 10 characters can be entered")]
        [StringLength(10,ErrorMessage = "Contact numbers should be of 10 digits")]
        [DisplayName("Contact Number")]
        [RegularExpression(@"^\d+$",ErrorMessage = "Please enter only numeric values")]
        [DataType(DataType.Text)]
        public string? ContactNumber { get; set; }


        //[EmailAddress]
        [DataType(DataType.Text)]
        [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$", ErrorMessage = "Please enter valid email address")]
        [Required(ErrorMessage = "Please enter email address")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Please enter date fof birth")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString ="dd:MM:yyyy",ApplyFormatInEditMode = false)]
        [DisplayName("Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Please select gender")]
        public string? Gender { get; set; }

        [Required(ErrorMessage = "Please select a state")]
        [DataType(DataType.Text)]
        //[BindProperty]
        public string? State { get; set; }

        public List<State>? States { get; set; }

        [Required(ErrorMessage = "Please select a city")]
        [DataType(DataType.Text)]
        //[BindProperty]
        public string? City { get; set; }

        public List<City>? Cities { get; set; }

        [Required(ErrorMessage = "Please enter your address.")]
        [DataType (DataType.Text)]
        [MaxLength(1000,ErrorMessage = "Maximum character limit is 1000 characters")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Please select atleast one area of interest")]
        [DataType(DataType.Text)]
        [BindProperty]
        public string[]? AreaOfInterest { get; set; }
        [AreaOfInterestValidation(ErrorMessage = "Please select atleast one area of interest")]
        public List<AreaOfInterestViewModel>? AreaOfInterests { get; set; }

        [MaxFileSizeValidation(2000000,ErrorMessage = "File size should not be greatr than 2 mb")]
        [AllowedFileExtensionValidation([".pdf"],ErrorMessage = "Please upload .pdf file.")]
        [Required(ErrorMessage = "Please select a file.")]
        [DataType(DataType.Upload)]
        public byte[]? Resume { get; set; }

        [DataType(DataType.Upload)]
        [MaxFileSizeValidation(2 * 1024 * 1024, ErrorMessage = "File size should not be greatr than 2 mb")]
        [AllowedFileExtensionValidation([".pdf"], ErrorMessage = "Please upload .pdf file.")]
        [DisplayName("Updated Resume")]
        public byte[]? UpdatedResume { get; set; }
        public string? FileName { get; set; }
        public string? ContentType { get; set; }
        public FileContentResult? Content { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "dd:MM:yyyy", ApplyFormatInEditMode = false)]
        public DateTime AppliedOn { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "dd:MM:yyyy", ApplyFormatInEditMode = false)]
        public DateTime Updatedon { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

    }
}
