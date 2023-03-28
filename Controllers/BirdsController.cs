using System.IO;
using BlueBirds.Data;
using BlueBirds.Models;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace BlueBirds.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BirdsController : Controller
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public BirdsController(DataContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;


        }

        [HttpGet]
        public async Task<ActionResult<List<Bird>>> getAllBirds()
        {
            return Ok(await _context.Birds.ToListAsync());
        }

        [HttpPost]
        public async Task<ActionResult<Bird>> createBird([FromForm]BirdDto newBird)
        {

            string wwwRootPath = _hostEnvironment.WebRootPath;

            if (newBird.PhotoFile != null)
            {
                MemoryStream memoryStream = new MemoryStream();
                newBird.PhotoFile.OpenReadStream().CopyTo(memoryStream);
                string photoUrl = Convert.ToBase64String(memoryStream.ToArray());

                string filename = Guid.NewGuid().ToString();
                var uploads = Path.Combine(wwwRootPath, @"images\birds");
                var extension = Path.GetExtension(newBird.PhotoFile.FileName);


                Uri domain = new Uri(Request.GetDisplayUrl());



                using (var fileStreams = new FileStream(Path.Combine(uploads, filename + extension), FileMode.Create))
                {
                    newBird.PhotoFile.CopyTo(fileStreams);
                }

                photoUrl = domain.Scheme + "://" + domain.Host + (domain.IsDefaultPort ? "" : ":" + domain.Port) + "/images/birds/" + filename + extension;

                var oldPhoto = await _context.Photos.FirstOrDefaultAsync(p => p.Id == newBird.Id);

                if (oldPhoto != null)
                {
                    _context.Photos.Remove(oldPhoto);
                }

                var photo = new Photo
                {
                    Id = newBird.Id,
                    Url = photoUrl,

                };

                _context.Photos.Add(photo);

                var bird = new Bird
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = newBird.Name,
                    Geolocalization = newBird.Geolocalization,
                    PhotoURL = photoUrl,
                    PhotoFile = newBird.PhotoFile
                };
                
                _context.Birds.Add(bird);
                await _context.SaveChangesAsync();

                return Ok(bird);


            }

            return Ok(newBird);

        }


        [HttpPut]
        public async Task<ActionResult<Bird>> updateBird([FromForm]BirdDto updatedBird) { 
            
            
            string wwwRootPath = _hostEnvironment.WebRootPath;

            if (updatedBird.PhotoFile != null)
            {
                MemoryStream memoryStream = new MemoryStream();
                updatedBird.PhotoFile.OpenReadStream().CopyTo(memoryStream);
        string photoUrl = Convert.ToBase64String(memoryStream.ToArray());

        string filename = Guid.NewGuid().ToString();
        var uploads = Path.Combine(wwwRootPath, @"images\birds");
        var extension = Path.GetExtension(updatedBird.PhotoFile.FileName);


        Uri domain = new Uri(Request.GetDisplayUrl());



         using (var fileStreams = new FileStream(Path.Combine(uploads, filename + extension), FileMode.Create))
                {
                    updatedBird.PhotoFile.CopyTo(fileStreams);
                }

                photoUrl = domain.Scheme + "://" + domain.Host + (domain.IsDefaultPort ? "" : ":" + domain.Port) + "/images/birds/" + filename + extension;

                var oldPhoto = await _context.Photos.FirstOrDefaultAsync(p => p.Id == updatedBird.Id);

                if (oldPhoto != null)
                {
                    _context.Photos.Remove(oldPhoto);
                }

                var photo = new Photo
                {
                    Id = updatedBird.Id,
                    Url = photoUrl,

                };

                _context.Photos.Add(photo);

                var bird = await _context.Birds.FirstOrDefaultAsync(o=>o.Id == updatedBird.Id);

                bird.Id = updatedBird.Id;
                bird.Name = updatedBird.Name;
                bird.Geolocalization = updatedBird.Geolocalization;
                bird.PhotoURL = photoUrl;
                bird.PhotoFile = updatedBird.PhotoFile;
                
                await _context.SaveChangesAsync();

                return Ok(bird);


            }
            else
            {

                var bird = await _context.Birds.FirstOrDefaultAsync(o=>o.Id == updatedBird.Id);

                bird.Id = updatedBird.Id;
                bird.Name = updatedBird.Name;
                bird.Geolocalization = updatedBird.Geolocalization;
                bird.PhotoURL = bird.PhotoURL;
                bird.PhotoFile = updatedBird.PhotoFile;
                
                await _context.SaveChangesAsync();

                return Ok(bird);
            }


        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Bird>> deleteBird(string id)
        {

            var bird = await _context.Birds.FirstOrDefaultAsync(o=>o.Id == id);

            _context.Birds.Remove(bird);

            await _context.SaveChangesAsync();

            return Ok(bird);
        }

    }
}
