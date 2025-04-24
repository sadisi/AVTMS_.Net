using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AVTMS.Models
{
    public class Admin: UserActivity
    {
        public int Id { get; set; }

        [Display(Name = "Employee ID")]
        [Required(ErrorMessage = "User ID is required.")]
        public int EmployeeId { get; set; }

        [Display(Name = "Employee NIC")]
        [Required(ErrorMessage = "NIC is required.")]
        public string NIC { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "First Name is required.")]
        public string FirstName { get; set; }

        [Display(Name = "Middle Name")]
        [Required(ErrorMessage = "Middle Name is required.")]
        public string MiddleName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Last Name is required.")]
        public string LastName { get; set; }

        [Display(Name = "FirstName")]
        public string FullName => $"{FirstName} {MiddleName} {LastName}";

        [Display(Name = "Email")]
        [EmailAddress]
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }

        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(40, MinimumLength = 8, ErrorMessage = "The {0} must be at {2} and at max {1} characters long.")]
        public string Password { get; set; }

        [Display(Name = "Phone Number")]
        [Phone]
        [Required(ErrorMessage = "Phone Number is required.")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Registered Date")]
        public DateTime RegisteredDate { get; set; }

        [Display(Name = "User Type")]
        [Required(ErrorMessage = "Usertype is required.")]
        public string UserType { get; set; } = "Admin";

        [Display(Name = "User Branch")]
        public string UserBranch { get; set; }

       

        [NotMapped]
        public bool IsRegistered { get; set; }

    }
}
