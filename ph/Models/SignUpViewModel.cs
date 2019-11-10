using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ph.Models
{
    public class SignUpViewModel
    {
        public User User { get; set; }
        [Required(ErrorMessage = "Password may not be empty.")]
        public string Password { get; set; }
        
        
        [Required(ErrorMessage = "Confirm your password.")]
        public IFormFile ProfileImage { get; set; }
    }

    internal class ErrorMessage
    {
    }
}