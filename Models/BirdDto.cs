using System.ComponentModel.DataAnnotations.Schema;

namespace BlueBirds.Models
{
    public class BirdDto
    {
        public string? Id { get; set; }
        public string? Name { get; set; }

        public string Geolocalization { get; set; }

        [NotMapped]
        public IFormFile PhotoFile { get; set; }
    }
}
