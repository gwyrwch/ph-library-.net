using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static System.Net.Mime.MediaTypeNames;

namespace ph.Models
{
    public class Pet
    {
        public Pet()
        {
            PetsToPosts = new List<PetToPost>();
        }
        
        [Key]
        public string Id { get; set; }
        
        [Required]
        [StringLength(30, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 2)]
        public string Name { get; set; }
        
        [Required]
        public string UserId { get; set; }
        
        [Required]
        [ForeignKey("UserId")]
        public User User { get; set; }
        

        [Required]
        public PetType PetType { get; set; }
        
        public string Breed { get; set; }
        
        public string ProfileImagePath { get; set; }
        
        public ICollection<PetToPost> PetsToPosts { get; set; }
        
    }
}