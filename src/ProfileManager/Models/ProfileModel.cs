using System;
using Microsoft.EntityFrameworkCore;

namespace ProfileManager.Models
{
    public class Profile
    {
        public int ProfileId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Department { get; set; }
        public string Photo { get; set; }
    }

    public class ProfileDbContext : DbContext
    {
        public ProfileDbContext(DbContextOptions<ProfileDbContext> options) : base(options) 
        {

        }

        public DbSet<Profile> Profiles { get; set; }


    } 

}