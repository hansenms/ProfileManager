using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using ProfileManager.Data;
using ProfileManager.Extensions;
using ProfileManager.Models;

namespace ProfileManager.Controllers
{
    [Route("api/[controller]")]
    public class ProfileController : Controller
    {
        private readonly ProfileContext _context;
        private readonly IConfiguration _configuration;
        
        private CloudBlobContainer GetStorageContainer()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_configuration.GetConnectionString("StorageConnectionString"));
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Get a reference to a container named "images".
            CloudBlobContainer container = blobClient.GetContainerReference("images");

            return container;            
        }

        public ProfileController(ProfileContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
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
        public IActionResult Create([FromBody]Profile profile)
        {
            _context.Profiles.Add(profile);
            _context.SaveChanges();

            return CreatedAtRoute("GetProfile", new { id = profile.Id }, profile);
        }

        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody]Profile updatedProfile)
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

        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            var profile = _context.Profiles.Find(id);
            if (profile == null)
            {
                return NotFound();
            }
            _context.Profiles.Remove(profile);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPost("Image/{id}")]
        public async Task<IActionResult> Create(long id, IFormFile file)
        {
            string[] supportedMimeTypes = { "image/png", "image/jpeg", "image/jpg" };

            if (!supportedMimeTypes.Contains(file.ContentType))
            {
                return BadRequest("Only JPEG and PNG Supported.");
            }

            if (!(file.Length > 0))
            {
                return BadRequest("Empty file recieved");
            }

            CloudBlobContainer container = GetStorageContainer();

            var blobName = file.GetFilename();

            var fileStream = await file.GetFileStream();
            fileStream.Position = 0;

            blobName = string.Format(@"{0}/{1}", id, blobName);

            CloudBlockBlob blob = container.GetBlockBlobReference(blobName);

            await blob.UploadFromStreamAsync(fileStream);

            //TODO changed to Created (201)
            return Ok(new { fileName = blobName });
        }


        //Get list of files for profile id
        [HttpGet("Image/{id}")]
        public async Task<IActionResult> Get(long id)
        {
            // Get a reference to a container named "images".
            CloudBlobContainer container = GetStorageContainer();

            List<string> blobNames = new List<string>();

            BlobContinuationToken token = null;
            do
            {
                //TODO: Use flat listing with prefix instead
                BlobResultSegment resultSegment = await container.ListBlobsSegmentedAsync(token);
                token = resultSegment.ContinuationToken;

                foreach (IListBlobItem item in resultSegment.Results)
                {
                    if (item.GetType() == typeof(CloudBlobDirectory))
                    {
                        CloudBlobDirectory directory = (CloudBlobDirectory)item;
                        Console.WriteLine(@"Prefix {0}", directory.Prefix);
                        if (directory.Prefix == id.ToString() + "/")
                        {
                            BlobResultSegment resultSegmentDir = await directory.ListBlobsSegmentedAsync(null);
                            foreach (IListBlobItem item2 in resultSegmentDir.Results)
                            {
                                if (item2.GetType() == typeof(CloudBlockBlob))
                                {
                                    CloudBlockBlob blob = (CloudBlockBlob)item2;
                                    blobNames.Add(blob.Name);
                                    Console.WriteLine("Block blob of length {0}: {1}", blob.Properties.Length, blob.Uri);
                                }
                            }
                            break;
                        }
                    }
                }
            }
            while (token != null);

            return Ok(new { fileName = blobNames });
        }

        [HttpGet("Image/{*imagepath}")]
        public async Task<IActionResult> Get(string imagepath)
        {

            var container = GetStorageContainer();

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(imagepath);

            if (!await blockBlob.ExistsAsync())
            {
                return NotFound();
            }

            MemoryStream filestream = new MemoryStream();
            await blockBlob.DownloadToStreamAsync(filestream);

            string contentType = MimeTypes.GetMimeType(imagepath);

            return File(filestream.ToArray(), contentType);
        }

        [HttpPost("Image/face/{apiname}")]
        public async Task<IActionResult> ProxyDetect(string apiname)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(_configuration["CogSvcEndpoint"]);
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _configuration["FaceApiKey"]);

            var content = new StreamContent(Request.Body);
            content.Headers.Add("Content-type", Request.ContentType);
            var response = await client.PostAsync($"face/v1.0/{apiname}/{Request.QueryString}", content);
           
            return new ContentResult() {
                Content = await response.Content.ReadAsStringAsync(),
                StatusCode = (int)response.StatusCode,
                ContentType = "application/json"
            };
        }
    }
}