using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static System.Net.Mime.MediaTypeNames;

namespace ph.Models
{
    public class Pet
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(30, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 2)]
        public string Name { get; set; }
        
        [Required]
        public User Master { get; set; }

        public string Breed { get; set; } // todo: make table with breeds?

        public List<string> PhotoPaths { get; set; } // todo: list of images
    }
}