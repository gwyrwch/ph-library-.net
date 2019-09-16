using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static System.Net.Mime.MediaTypeNames;

namespace ph.Models
{
    public class Pet
    {
        public uint Id { get; set; }
        
        [Required]
        [StringLength(30, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 2)]
        public string Name { get; set; }
        
        [Required]
        public uint OwnerId { get; set; }

        public PetType PetType { get; set; }
        
        public string Breed { get; set; }
    }
}