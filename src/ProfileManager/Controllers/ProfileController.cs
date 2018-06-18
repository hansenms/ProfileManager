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
    [ApiController]
    public class ProfileController : Controller
    {
        private readonly ProfileContext _context;

        public ProfileController(ProfileContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<List<Profile>> GetAll()
        {
            return _context.Profiles.ToList();
        }

        [HttpGet("{id}", Name = "GetProfile")]
        public ActionResult<Profile> GetById(int id)
        {
            var profile = _context.Profiles.Find(id);
            if (profile == null)
            {
                return NotFound();
            }
            return profile;
        }
    }
}