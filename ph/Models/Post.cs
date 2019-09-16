using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Antiforgery.Internal;

namespace ph.Models
{
    public class Post
    {
        [Required] 
        public uint Id { get; set; }
        
        [Required]
        public string ImagePath { get; set; }
        
        [Required]
        [StringLength(2000, ErrorMessage = "{0} length must be less than {1}.")]
        public string Description { get; set; }
        
        [Required]
        public uint AuthorId { get; set; }
        
        [Required]
        public PostType Type { get; set; }
        
        public DateTime PublicationTime { get; set; }
        
        // todo: make class Pet2Post
        public uint? IncludedPetId { get; set; }
    }
    
}