using System.ComponentModel.DataAnnotations;

namespace AVTMS.Models
{
    public class ViolationEmail
    {
        public Guid Id { get; set; }

        [Required]
        public string LicensePlate { get; set; }

        [Required]
        public string VehicleModel { get; set; }

        public string NoteContent { get; set; }

        [Required]
        public DateTime CapturedTime { get; set; }

        [Required]
        public string OwnerName { get; set; }

        [Required]
        public string OwnerNIC { get; set; }

        [Required]
        [EmailAddress]
        public string OwnerEmail { get; set; }

        public DateTime SentAt { get; set; }
    }
}
