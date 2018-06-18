using System.Linq;
using ProfileManager.Models;

namespace ProfileManager.Data
{
    public class ProfileDbInitializer
    {
        public static void Initialize(ProfileContext context)
        {
            context.Database.EnsureCreated();

            if (context.Profiles.Any()) 
            {
                //Database is initialized.
                return;
            }

            var profiles = new Profile[] {
                new Profile{ FirstName = "Abby", LastName = "Addision", Department = "Accounting", Photo = "abby.jpg"},
                new Profile{ FirstName = "Bob", LastName = "Beumont", Department = "Business Desk", Photo = "bob.jpg"},
                new Profile{ FirstName = "Chris", LastName = "Christianson", Department = "Communications", Photo = "chris.jpg"}
            };

            foreach (Profile p in profiles)
            {
                context.Add(p);
            }

            context.SaveChanges();
        }
    }
}