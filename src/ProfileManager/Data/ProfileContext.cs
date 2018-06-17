using Microsoft.EntityFrameworkCore;
using ProfileManager.Models;

namespace ProfileManager.Data
{
    public class ProfileContext : DbContext
    {
        public ProfileContext(DbContextOptions<ProfileContext> options) : base(options) 
        {

        }

        public DbSet<Profile> Profiles { get; set; }

    } 
}