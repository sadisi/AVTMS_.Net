using System.ComponentModel.DataAnnotations;

namespace AVTMS.Models
{
    public class VehicleNotes : UserActivity
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Note is required.")]
        [Display(Name = "Vehicle Note", Prompt = "Add a note...")]
        public string? NoteContent { get; set; }

        [Display(Name = "Vehicle Number plate ")]
        [Required(ErrorMessage = "Vehicle Numberplate is required.")]
        public int VehicleId { get; set; }
        public Vehicle? Vehicle { get; set; }




    }
}
