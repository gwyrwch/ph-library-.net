using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace ph.Models
{
    public class CreatePostViewModel
    {
        public CreatePostViewModel()
        {
            Pets = new List<Pet>();
        }
        
        public Post Post { get; set; }
        public IFormFile PostImage { get; set; }
        
         public string Username { get; set; }
         
         public ICollection<Pet> Pets { get; set; }
         
         public string SelectedPets { get; set; }

    }
}