using System.ComponentModel.DataAnnotations;

namespace AVTMS.Models
{
    public class DetectedVehicle
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Vehicle Number Plate")]
        public string NumberPlate { get; set; }

        [Required]
        [Display(Name = "Detected Time")]
        public DateTime DetectedAt { get; set; }

        [Display(Name = "Uploaded Video File Name")]
        public string VideoFileName { get; set; }

        // Optional: Save the path of the uploaded video
        public string VideoPath { get; set; }
    }
}
