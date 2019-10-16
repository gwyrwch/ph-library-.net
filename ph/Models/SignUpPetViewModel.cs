using Microsoft.AspNetCore.Http;

namespace ph.Models
{
    public class SignUpPetViewModel
    {
        public Pet Pet { get; set; }
        public IFormFile ProfileImage { get; set; }
    }
}