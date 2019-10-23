using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Antiforgery.Internal;

namespace ph.Models
{
    public class Post
    {
        public Post()
        {
            PetsToPosts = new List<PetToPost>();
        }

        [Required] 
        public string Id { get; set; }
        
        [Required]
        public string ImagePath { get; set; }
        
        [Required]
        [StringLength(2000, ErrorMessage = "{0} length must be less than {1}.")]
        public string Description { get; set; }
        
        [Required]
        public string UserId { get; set; }
        
        [Required]
        [ForeignKey("UserId")]
        public User User { get; set; }
        
        [Required]
        public PostType Type { get; set; }
        
        public DateTime PublicationTime { get; set; }
        
        // todo: make class Pet2Post
//        [Required]
//        public string PetId { get; set; }
//        
//        [Required]
//        [ForeignKey("PetId")]
//        public User Pet { get; set; }

        public ICollection<PetToPost> PetsToPosts { get; set; }
        public ICollection<Like> Likes { get; set; }
    }
        
}