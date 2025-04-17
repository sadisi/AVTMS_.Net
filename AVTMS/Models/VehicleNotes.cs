namespace AVTMS.Models
{
    public class VehicleNotes : UserActivity
    {
        public int Id { get; set; }

        public string? NoteContent { get; set; }

        public int VehicleId { get; set; }
        public Vehicle? Vehicle { get; set; }




    }
}
