using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace BlueBirds.Models
{
    public class AppUser : IdentityUser
    {

        //public string? PhotoUrl { get; set; }

        public string Role { get; set; }

        public string? Token { get; set; }

        [NotMapped]
        public IFormFile? PhotoFile { get; set; }



    }
}
