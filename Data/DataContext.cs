using BlueBirds.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlueBirds.Data
{
    public class DataContext : IdentityDbContext<AppUser>
    {

        public DataContext(DbContextOptions options) : base(options) 
        {
            
        }

        public DbSet<Bird> Birds { get; set; }
        public DbSet<Photo> Photos { get; set; }

    }
}
