using Microsoft.AspNetCore.Http;

namespace ph.Models
{
    public class CreatePostViewModel
    {
        public Post Post { get; set; }
        public IFormFile PostImage { get; set; }

    }
}