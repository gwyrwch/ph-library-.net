using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ph.Models;

namespace ph.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<Pet> Pets;
        public DbSet<Post> Posts;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}