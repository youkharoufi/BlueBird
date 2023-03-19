using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlueBirds.Models
{
    public class Bird
    {
        [Key]
        public string? Id { get; set; }

        public string Name { get; set; }

        public string Geolocalization { get; set; }

        public string? PhotoURL { get; set; }

        [NotMapped]
        public IFormFile? PhotoFile { get; set; }

    }
}
