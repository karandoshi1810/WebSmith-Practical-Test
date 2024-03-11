using System.ComponentModel.DataAnnotations;

namespace JobListing.Models
{
    public class Login
    {
        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$", ErrorMessage = "Please enter valid email address")]
        [DataType(DataType.EmailAddress)] 
        public required string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public required string Password { get; set; }
    }
}
