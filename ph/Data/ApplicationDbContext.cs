using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ph.Models;

namespace ph.Data
{
    public sealed class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<Pet> Pets { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PetToPost> PetsToPosts { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
//            Database.EnsureCreated();
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Pet>()    
                .HasOne<User>(p => p.User)
                .WithMany(u => u.Pets)
                .HasForeignKey(p => p.UserId);
            
            // configures one-to-many relationship
            modelBuilder.Entity<Post>()    
                .HasOne<User>(p => p.User)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.UserId);

            modelBuilder.Entity<PetToPost>().HasKey(pp => new {pp.PetId, pp.PostId});

            modelBuilder.Entity<PetToPost>()
                .HasOne<Pet>(sc => sc.Pet)
                .WithMany(s => s.PetsToPosts)
                .HasForeignKey(sc => sc.PetId);


            modelBuilder.Entity<PetToPost>()
                .HasOne<Post>(sc => sc.Post)
                .WithMany(s => s.PetsToPosts)
                .HasForeignKey(sc => sc.PostId);

        }

       
    }
}