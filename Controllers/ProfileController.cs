using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProfileManager.Data;
using ProfileManager.Models;

namespace ProfileManager.Controllers
{
    [Route("api/[controller]")]
    public class ProfileController : Controller
    {
        private readonly ProfileContext _context;

        public ProfileController(ProfileContext context)
        {
            _context = context;
        }

        [HttpGet]
        public List<Profile> GetAll()
        {
            return _context.Profiles.ToList();
        }
    }
}