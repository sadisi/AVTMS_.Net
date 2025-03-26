using AVTMS.Models;
using Microsoft.EntityFrameworkCore;

namespace AVTMS.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
        
        {
             
        }

        public DbSet<Vehicle> Vehicles {  get; set; }
    }
}
