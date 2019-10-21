using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using ph.Models;


namespace ph.Data
{
    public class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly IServiceProvider _serviceProvider;
       
        public DatabaseInitializer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;          
        }

        public async void Initialize()
        {            
            using (var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                context.Database.EnsureCreated();

                var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();
                if (!(await roleManager.RoleExistsAsync("Admin")))  
                {
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                }

                var userManager = serviceScope.ServiceProvider.GetService<UserManager<User>>();
                if (userManager.Users.First(user => user.UserName == "gwyrwch@gmail.com") == null)
                {
                    const string user = "gwyrwch@gmail.com";
                    const string password = "AbC!12345";
                    var success = await userManager.CreateAsync(new User { UserName = user, Email = user, EmailConfirmed = true }, password);
                    if (success.Succeeded)
                    {
                        await userManager.AddToRoleAsync(await userManager.FindByNameAsync(user), "Admin");
                    }
                }
                
            }            
        }
    }
}