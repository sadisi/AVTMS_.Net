
using AVTMS.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AVTMS.Data
{
    public class AppDbContext : IdentityDbContext<Users>
    {
        public AppDbContext(DbContextOptions options) : base(options)

        {

        }

        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<AVTMS.Models.BaseUser> BaseUser { get; set; } = default!;
        public DbSet<AVTMS.Models.AuthUsers> AuthUsers { get; set; } = default!;
        public DbSet<AVTMS.Models.VehicleOwner> VehicleOwner { get; set; } = default!;
        public DbSet<AVTMS.Models.VehicleNotes> VehicleNotes { get; set; } = default!;
        public DbSet<AVTMS.Models.DetectedVehicle> DetectedVehicle { get; set; } = default!;
    }
}
