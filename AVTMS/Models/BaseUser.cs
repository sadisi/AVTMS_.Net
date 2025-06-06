﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AVTMS.Models
{
    public class BaseUser : UserActivity
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Employee ID is required.")]
        [Display(Name = "Employee ID", Prompt = "Enter Employee ID")]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "NIC is required.")]
        [Display(Name = "NIC", Prompt = "Enter NIC")]
        public string NIC { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        [Display(Name = "First Name", Prompt = "Enter First Name")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Middle Name is required.")]
        [Display(Name = "Middle Name", Prompt = "Enter Middle Name")]
        public string MiddleName { get; set; }
        [Required(ErrorMessage = "Last Name  is required.")]
        [Display(Name = "Last Name", Prompt = "Enter Last Name")]
        public string LastName { get; set; }

        public string FullName => $"{FirstName} {MiddleName} {LastName}";

        [Required(ErrorMessage = "Email is required.")]
        [Display(Name = "Email", Prompt = "Enter Email")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        [Display(Name = "Password", Prompt = "Enter Password")]
        [StringLength(40, MinimumLength = 8, ErrorMessage = "The {0} must be at {2} and at max {1} characters long.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Phone]
        [Required(ErrorMessage = "Phone Number is required.")]
        [Display(Name = "Phone Number", Prompt = "Enter Phone Number")]
        public string PhoneNumber { get; set; }
        public DateTime RegisteredDate { get; set; }

        public string UserType { get; set; } = "Base User";

        [Display(Name = "User Branch")]
        [Required(ErrorMessage = "User branch is required.")]
        public string UserBranch { get; set; }



        [NotMapped]
        public bool IsRegistered { get; set; }
    }
}
