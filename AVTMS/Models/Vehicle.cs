using System.ComponentModel.DataAnnotations;

namespace AVTMS.Models
{
    public class Vehicle: UserActivity
    {
        public int Id { get; set; }

        [Required]
        public string VehicleNumberPlate { get; set; }

        [Required]
        public string VehicleModel { get; set; }

        [Required]
        public string VehicleColor { get; set; }

        [Required]
        public string? VehicleNote { get; set; }

        [Required]
        public string VehicleOwnerNIC { get; set; }
        public VehicleOwner? VehicleOwner { get; set; }





    }
}
