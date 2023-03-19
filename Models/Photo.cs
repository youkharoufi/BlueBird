using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlueBirds.Models
{
    public class Photo
    {
        [Key]
        public string? Id { get; set; }

        public string? Url { get; set; }

        [NotMapped]
        public IFormFile? photoFile { get; set; }

        public string? UserId { get; set; }

        public AppUser? AppUser { get; set; }
    }
}
