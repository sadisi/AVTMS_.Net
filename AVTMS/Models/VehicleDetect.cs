using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AVTMS.Models
{
    public class VehicleDetect
    {

        [Key]
        public int DetectId { get; set; }
        public string start_time { get; set; }
        public string end_time { get; set; }
        public string license_plate { get; set; }

        [NotMapped]
        public Vehicle MatchedVehicle { get; set; }


    }
}
