using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AVTMS.Models
{
    public class AuthUsers :UserActivity
    {
        public int Id { get; set; }

        [Display(Name = "Employee ID", Prompt = "Enter Employee ID")]
        [Required(ErrorMessage = "Employee ID is required.")]
        public int EmployeeId { get; set; }

        [Display(Name = "NIC Number", Prompt = "Enter NIC Number")]
        [Required(ErrorMessage = "NIC Number is required.")]
        public string NIC { get; set; }

        [Display(Name = "First Name", Prompt = "Enter First Name")]
        [Required(ErrorMessage = "First Name is required.")]
        public string FirstName { get; set; }

        [Display(Name = "Middle Name", Prompt = "Enter Middle Name")]
        [Required(ErrorMessage = "Middle Name is required.")]
        public string MiddleName { get; set; }

        [Display(Name = "Last Name ", Prompt = "Enter Last Name")]
        [Required(ErrorMessage = "Last Name is required.")]
        public string LastName { get; set; }
        [Display(Name = "Full Name")]
        public string FullName => $"{FirstName} {MiddleName} {LastName}";
        [Display(Name = "Email", Prompt = "Enter Email")]
        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Display(Name = "Password", Prompt = "Enter Password")]
        [StringLength(40, MinimumLength = 8, ErrorMessage = "The {0} must be at {2} and at max {1} characters long.")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; 
        }
        [Display(Name = "Phone Number", Prompt = "Enter Phone Number")]
        [Phone]
        [Required(ErrorMessage = "Phone Number is required")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Register Date")]
        public DateTime RegisteredDate { get; set; }

        [Display(Name = "User Type")]
        public string UserType { get; set; } = "Authorized User";

        [Display(Name = "User Branch")]
        [Required(ErrorMessage = "User Branch is required.")]
        public string UserBranch { get; set; }


        [NotMapped] 
        public bool IsRegistered { get; set; }
    }
}
