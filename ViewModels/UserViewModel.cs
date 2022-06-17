using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagementSystem.ViewModels
{
    public class UserViewModel
    {
        public string UserId { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        //[Required(ErrorMessage = "Please enter password")]
        [MinLength(5, ErrorMessage = "The password must be at least 5 characters long")]
        [MaxLength(20, ErrorMessage = "The password cannot be longer than 20 characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public Role Role { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of birth")]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; }

        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Invalid phone number")]

        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        [NotMapped]
        public string FullName
        {
            get { return FirstName + " " + LastName; }
        }
    }
}
