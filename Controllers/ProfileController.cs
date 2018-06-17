using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProfileManager.Models;

namespace ProfileManager.Controllers
{
    [Route("api/[controller]")]
    public class ProfileController : Controller
    {

        [HttpGet]
        public List<Profile> GetAll()
        {
            var retList = new List<Profile>();
            retList.Add( new Profile{ 
                FirstName = "Michael",
                LastName = "Hansen",
                Department = "In Charge",
                Photo = "myphoto.png"
                } );

            return retList;    
        }
    }
}

