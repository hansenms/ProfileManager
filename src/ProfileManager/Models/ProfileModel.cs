using System;
using Microsoft.EntityFrameworkCore;

namespace ProfileManager.Models
{
    public class Profile
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Department { get; set; }
        public string Photo { get; set; }
    }
}