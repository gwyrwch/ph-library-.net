using System.ComponentModel.DataAnnotations;

namespace ph.Models
{
    public class Post
    {
        [Required]
        public string ImagePath { get; set; }
        
        [Required]
        [StringLength(2000, ErrorMessage = "{0} length must be less than {1}.")]
        public string Description { get; set; }
        
        [Required]
        public User Author { get; set; }
        
        [Required]
        public PostSections Section { get; set; }
    }
    
}