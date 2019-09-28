using System.Collections.Generic;

namespace ph.Models
{
    public class ProfileViewModel
    {
        public IEnumerable<Post> Posts { get; set; }
        public IEnumerable<Pet> Pets { get; set; }
        public User User { get; set; }
    }
}