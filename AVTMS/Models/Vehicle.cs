using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AVTMS.Models
{
    public class Vehicle: UserActivity
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vehicle Number Plate is required.")]
        [Display(Name = "Vehicle Number Plate")]
        public string VehicleNumberPlate { get; set; }

        [Required(ErrorMessage = "Vehicle Model is required.")]
        [Display(Name = "Vehicle Model (BMW/Benz/Toyota etc.)")]
        public string VehicleModel { get; set; }

        [Required(ErrorMessage = "Vehicle Color is required.")]
        [Display(Name = "Vehicle Color")]
        public string VehicleColor { get; set; }


        [Required(ErrorMessage = "Vehicle Note is required.")]
        [Display(Name = "Vehicle Note ")]
        public string? VehicleNote { get; set; }


        [Required(ErrorMessage = "Vehicle Owner NIC is required.")]
        [Display(Name = "Vehicle Owner NIC")]
        public string VehicleOwnerNIC { get; set; }
        public VehicleOwner? VehicleOwner { get; set; }


        [Display(Name = "Vehicle Image")]
        public string? ImagePath { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }




    }
}
