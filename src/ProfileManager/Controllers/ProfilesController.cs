using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProfileManager.Data;
using ProfileManager.Models;

namespace ProfileManager.Controllers
{
    public class ProfilesController : Controller
    {
        private HttpClient GetHttpClient(string accessToken)
        {
            var client = new HttpClient();
            string protocol = Request.IsHttps ? "https://" : "http://";
            client.BaseAddress = new Uri(protocol + Request.Host.ToUriComponent() + "/");
            client.DefaultRequestHeaders.Clear();  
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));  

            if (!String.IsNullOrEmpty(accessToken))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

            return client;
        }

        // GET: Profiles
        public async Task<IActionResult> Index()
        {

            List<Profile> profiles = new List<Profile>();
            using (var client = GetHttpClient(Request.Headers["x-ms-token-aad-access-token"]))
            { 
                HttpResponseMessage response = await client.GetAsync("api/profile");
                if (response.IsSuccessStatusCode)
                {
                    profiles = await response.Content.ReadAsAsync<List<Profile>>();
                }
                else
                {
                    return Ok(Request.Headers["x-ms-token-aad-access-token"]);
                    //return NotFound();
                }

            }
            return View(profiles);
        }

        // GET: Profiles/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Profile profile;

            using (var client = GetHttpClient(Request.Headers["x-ms-token-aad-access-token"]))
            { 
                HttpResponseMessage response = await client.GetAsync("api/profile/" + id.ToString());
                if (response.IsSuccessStatusCode)
                {
                    profile = await response.Content.ReadAsAsync<Profile>();
                }
                else
                {
                    return NotFound();
                }
            }

            return View(profile);
        }

        // GET: Profiles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Profiles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Department,Photo")] Profile profile)
        {
            using (var client = GetHttpClient(Request.Headers["x-ms-token-aad-access-token"]))
            { 
                if (ModelState.IsValid)
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync("api/profile", profile);
                    if (response.IsSuccessStatusCode)
                    {
                        profile = await response.Content.ReadAsAsync<Profile>();
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            return View(profile);
        }

        // GET: Profiles/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Profile profile;

            using (var client = GetHttpClient(Request.Headers["x-ms-token-aad-access-token"]))
            { 
                HttpResponseMessage response = await client.GetAsync("api/profile/" + id.ToString());
                if (response.IsSuccessStatusCode)
                {
                    profile = await response.Content.ReadAsAsync<Profile>();
                }
                else
                {
                    return NotFound();
                }
            }
            return View(profile);
        }

        // POST: Profiles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,FirstName,LastName,Department,Photo")] Profile profile)
        {
            if (id != profile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                using (var client = GetHttpClient(Request.Headers["x-ms-token-aad-access-token"]))
                { 
                    HttpResponseMessage response = await client.PutAsJsonAsync("api/profile/" + id.ToString(), profile);
                    if (!response.IsSuccessStatusCode)
                    {
                        return NotFound();
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(profile);
        }

        // GET: Profiles/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Profile profile;

            using (var client = GetHttpClient(Request.Headers["x-ms-token-aad-access-token"]))
            { 
                HttpResponseMessage response = await client.GetAsync("api/profile/" + id.ToString());
                if (response.IsSuccessStatusCode)
                {
                    profile = await response.Content.ReadAsAsync<Profile>();
                }
                else
                {
                    return NotFound();
                }
            }

            return View(profile);
        }

        // POST: Profiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            using (var client = GetHttpClient(Request.Headers["x-ms-token-aad-access-token"]))
            { 
                HttpResponseMessage response = await client.DeleteAsync("api/profile/" + id.ToString());
                if (!response.IsSuccessStatusCode)
                {
                    return NotFound();
                }
            }    
            
            return RedirectToAction(nameof(Index));
        }
    }
}
