using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ph.Models;

namespace ph.Data
{
    public sealed class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<Pet> Pets { get; set; }
//        public DbSet<Post> Posts { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Pet>()    
                .HasOne<User>(p => p.User)
                .WithMany(u => u.Pets)
                .HasForeignKey(p => p.UserId);
            
//            base.OnModelCreating(modelBuilder);
//            // configures one-to-many relationship
//            modelBuilder.Entity<Post>()    
//                .HasOne<User>(p => p.User)
//                .WithMany(u => u.Posts)
//                .HasForeignKey(p => p.UserId);  
        }

       
    }
}