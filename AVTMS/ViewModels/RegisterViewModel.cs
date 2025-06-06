﻿using System.ComponentModel.DataAnnotations;

namespace AVTMS.ViewModels
{
    public class RegisterViewModel
    {

        [Required(ErrorMessage = "User Type is required.")]
        public string UserType { get; set; }

        [Required(ErrorMessage ="Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(40, MinimumLength = 8, ErrorMessage = "The {0} must be at {2} and at max {1} characters long.")]
        [DataType(DataType.Password)]
       
        public string Password { get; set; }

        [Required(ErrorMessage = "ConfirmPassword is required.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password does not match.")]
        public string ConfirmPassword { get; set; }
       
    }
}
