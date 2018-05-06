using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using antvk_api.Data;
using antvk_api.Dtos;
using antvk_api.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;
using System.Security.Cryptography;
using System.Text;

namespace antvk_api.Controllers
{
    [Route("api/[controller]")]
    public class PlaceController : Controller
    {
        private readonly DataContext _context;
        CloudStorageAccount storageAccount = new CloudStorageAccount(
                            new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
                            "tuymove",
                            "66HSLgR9RMAAJTfdRoE5iwIYzYVVinjIJCEU1pGWxSxWgpnknXK4b7GB57dtpBr/PzN7SVDcFPk0ccM320p0+Q=="), true);
        private CloudBlobClient blobClient;
        private CloudBlobContainer container;
        private CloudBlockBlob blockBlob;
        private string storageURL;
        public PlaceController(DataContext context)
        {
            _context = context;
            blobClient = storageAccount.CreateCloudBlobClient();
            container = blobClient.GetContainerReference("antvk-storage");
            storageURL = "https://tuymove.blob.core.windows.net/antvk-storage/";
        }

        [HttpGet]
        public async Task<IActionResult> getAllPlace()
        {
            var queryData = await _context.place
            .Include(events => events.events)
            .ToListAsync();
            if (queryData.Count == 0)
            {
                dynamic tempdata = new JObject();
                tempdata.placeName = "test";
                tempdata.placeDescription = "TestPlace";
                // resultData.placeEventDate = "2018-03-10";
                tempdata.placeImage = null;
                tempdata.placeType = 0;
                tempdata.location = new JObject();
                tempdata.location.latitude = 42.37396F;
                tempdata.location.longitude = -71.13412F;
                return Ok(tempdata);
            }

            List<JObject> resultData = new List<JObject>();
            foreach (var item in queryData)
            {
                // JObject tempdata = new JObject();
                dynamic tempdata = new JObject();
                tempdata.Add("placeID", item.placeID.ToString());
                tempdata.Add("placeName", item.placeName);
                tempdata.Add("placeDescription", item.placeDescription);
                tempdata.Add("placeImage", item.placeImage);
                tempdata.Add("placeType", item.placeType);
                tempdata.Add("location", new JObject());
                tempdata.location.Add("latitude", item.placeType);
                tempdata.location.Add("longitude", item.longitude);

                List<JObject> Eitems = new List<JObject>();
                foreach (var eventItem in item.events)
                {
                    dynamic Eitem = new JObject();
                    Eitem.eventName = eventItem.eventName;
                    Eitem.eventDesc = eventItem.eventDesc;
                    Eitem.eventDate = eventItem.eventDate;
                    Eitem.eventImage = eventItem.eventImage;
                    Eitems.Add(Eitem);
                }
                tempdata.Add("events", JToken.FromObject(Eitems));
                resultData.Add(tempdata);
            }
            return Ok(resultData);
        }

        [HttpPut]
        public async Task<IActionResult> addPlace([FromBody]placeForAdd data)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            // named "myfile".  
            var name = "";
            var hash = "";
            string[] ext  = data.placeImageEXT.Split('/');
            using (var sha256 = SHA256.Create())
            {
                // Send a sample text to hash.  
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(data.placeName));
                // Get the hashed string.  
                hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                // Print the string.   
                Console.WriteLine(hash);
                name = this.storageURL + hash + ext[1];
            }
            blockBlob = container.GetBlockBlobReference(hash + ext[1]);
            blockBlob.Properties.ContentType = data.placeImageEXT;
            await blockBlob.UploadFromByteArrayAsync(data.placeImage, 0, data.placeImage.Length);

            var addData = new place();

            addData.placeName = data.placeName;
            addData.placeDescription = data.placeDescription;
            addData.placeImage = name;
            addData.placeType = data.placeType;
            addData.latitude = data.latitude;
            addData.longitude = data.longitude;
            await _context.place.AddAsync(addData);
            await _context.SaveChangesAsync();
            return StatusCode(201);
        }

        [HttpPut("event")]
        public async Task<IActionResult> addEvent([FromBody]eventForAdd data)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
                var name = "";
            var hash = "";
            string[] ext  = data.eventImageEXT.Split('/');
            using (var sha256 = SHA256.Create())
            {
                // Send a sample text to hash.  
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(data.eventName));
                // Get the hashed string.  
                hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                // Print the string.   
                Console.WriteLine(hash);
                name = this.storageURL + hash + ext[1];
            }
            blockBlob = container.GetBlockBlobReference(hash + ext[1]);
            blockBlob.Properties.ContentType = data.eventImageEXT;
            await blockBlob.UploadFromByteArrayAsync(data.eventImage, 0, data.eventImage.Length);

            var addData = new events();
            place place = await _context.place.FindAsync(data.placeID);

            addData.eventName = data.eventName;
            addData.eventDesc = data.eventDesc;
            addData.eventImage = name;
            addData.place = place;
            addData.eventDate = data.eventDate;
            await _context.events.AddAsync(addData);
            await _context.SaveChangesAsync();
            return StatusCode(201);
        }
    }
}