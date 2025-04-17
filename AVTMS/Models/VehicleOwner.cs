using System.ComponentModel.DataAnnotations;

namespace AVTMS.Models
{
    public class VehicleOwner : UserActivity
    {
        [Key]
        public string NIC { get; set; }

        public string OwnerName { get; set; }
        public string OwnerMobileNumber { get; set; }
        public string OwnerEmail { get; set; }
        public string OwnerAddress { get; set; }






        public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    }
}
