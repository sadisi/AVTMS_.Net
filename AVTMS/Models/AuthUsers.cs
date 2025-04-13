namespace AVTMS.Models
{
    public class AuthUsers :UserActivity
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string NIC { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {MiddleName} {LastName}";
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime RegisteredDate { get; set; }

        public string UserType { get; set; } = "Authorized User";
        public string UserBranch { get; set; }
       


    }
}
