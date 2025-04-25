using System.ComponentModel.DataAnnotations;

namespace AVTMS.Models
{
    public class VehicleOwner : UserActivity
    {

        [Required(ErrorMessage = "Owner NIC is required.")]
        [Display(Name = "Owner NIC", Prompt = "Enter Owner NIC number")]
        [Key]

        public string NIC { get; set; }

        [Required(ErrorMessage = "Owner Name is required.")]
        [Display(Name = "Owner Name", Prompt = "Enter Owner Name ")]
        public string OwnerName { get; set; }

        [Required(ErrorMessage = "Owner Mobile Number is required.")]
        [Display(Name = "Owner Mobile Number", Prompt = "Enter Owner Mobile Number ")]
        [Phone]
        public string OwnerMobileNumber { get; set; }

        [Required(ErrorMessage = "Owner Email is required.")]
        [Display(Name = "Owner Email", Prompt = "Enter Owner Email ")]
        [EmailAddress]
        public string OwnerEmail { get; set; }

        [Required(ErrorMessage = "Owner Address is required.")]
        [Display(Name = "Owner Address", Prompt = "Enter Vehicle Owner Address ")]
        public string OwnerAddress { get; set; }






        public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    }
}
