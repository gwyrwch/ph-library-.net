using Microsoft.AspNetCore.Http;

namespace ph.Models
{
    public class SignUpViewModel
    {
        public User User { get; set; }
        public string Password { get; set; }
        
        public IFormFile ProfileImage { get; set; }
    }

    internal class ErrorMessage
    {
    }
}