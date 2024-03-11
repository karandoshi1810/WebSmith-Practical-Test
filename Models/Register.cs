using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace JobListing.Models
{
    public class Register
    {
        public int Id { get; set; }

        [DataType(DataType.Text)]
        [MaxLength(50, ErrorMessage = "Maximum number of characters that can be entered is 50")]
        [DisplayName("First Name")]
        [Required(ErrorMessage = "Please enter first name")]
        public string FirstName {  get; set; }

        [DataType(DataType.Text)]
        [MaxLength(50, ErrorMessage = "Maximum number of characters that can be entered is 50")]
        [DisplayName("Last Name")]
        [Required(ErrorMessage = "Please enter last name")]
        public string LastName { get; set;}

        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$", ErrorMessage = "Please enter valid email address")]
        [DisplayName("Email")]
        [Required(ErrorMessage = "Please enter email")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [DisplayName("Password")]
        [Required(ErrorMessage = "Please enter password")]
        [MaxLength(15, ErrorMessage = "Maximum number of characters that can be entered is 15")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,15}$",ErrorMessage = "Please enter valid password.\n Password must be of 8-15 characters long.\n It should contain:\n\t atleast one capital letter\n one digit\n and one special character")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [DisplayName("Confirm Password")]
        [MaxLength(15, ErrorMessage = "Maximum number of characters that can be entered is 8")]
        [Required(ErrorMessage = "Please enter confirm password")]
        [Compare("Password",ErrorMessage ="Confirm password value does not match password")]
        public string ConfirmPassword { get; set;}

    }
}
