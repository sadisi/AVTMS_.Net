using System.ComponentModel.DataAnnotations;

namespace AVTMS.ViewModels
{
    public class VeryfyEmailViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string Email { get; set; }
    }
}
