using System;

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

}