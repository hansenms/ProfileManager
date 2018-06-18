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
        public ActionResult<Profile> GetById(long id)
        {
            var profile = _context.Profiles.Find(id);
            if (profile == null)
            {
                return NotFound();
            }
            return profile;
        }

        [HttpPost]
        public IActionResult Create(Profile profile)
        {
            _context.Profiles.Add(profile);
            _context.SaveChanges();

            return CreatedAtRoute("GetProfile", new { id = profile.Id }, profile);
        }

        [HttpPut("{id}")]
        public IActionResult Update(long id, Profile updatedProfile)
        {
            var profile = _context.Profiles.Find(id);
            if (profile == null)
            {
                return NotFound();
            }

            profile.FirstName = updatedProfile.FirstName;
            profile.LastName = updatedProfile.LastName;
            profile.Department = updatedProfile.Department;
            profile.Photo = updatedProfile.Photo;

            _context.Profiles.Update(profile);
            _context.SaveChanges();
            return NoContent(); 
        }
    }
}