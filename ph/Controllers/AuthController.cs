using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ph.Data;
using ph.Models;

namespace ph.Controllers
{
    public class AuthController : Controller
    {
        private ApplicationDbContext db;
        private UserManager<User> _userManager = null;
        private SignInManager<User> _signInManager = null;

        public AuthController(ApplicationDbContext _context,
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder
                .UseSqlite("Data Source=app.db");
            db = _context;
            db.Pets.Load();

            _userManager = userManager;

            _signInManager = signInManager;
        }

        public IActionResult Login()
        {
            return View(new LoginViewModel { ReturnUrl = null });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel user)
        {
            if (ModelState.IsValid)
            {
                var result = 
                    await _signInManager.PasswordSignInAsync(user.UserName, user.Password, user.RememberMe, false);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(user.ReturnUrl) && Url.IsLocalUrl(user.ReturnUrl))
                    {
                        return Redirect(user.ReturnUrl);
                    }

                    return RedirectToAction("Feed", "Home", new {type=-1});
                }
                ModelState.AddModelError("", "Invalid username or password");
            }
            return View(user);
        }
        
        public  IActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(SignUpViewModel newUser)
        {
            if(!ModelState.IsValid)
                return View(newUser);
            
            var path = "";
            if (newUser.ProfileImage != null)
            {
                var ext = newUser.ProfileImage.FileName.Split('.').Last();
                path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot") + "/images/users/" + newUser.User.UserName + "." + ext;
                using (var fs = new FileStream(path, FileMode.Create))
                {
                    await newUser.ProfileImage.CopyToAsync(fs);
                }
            }

            newUser.User.ProfileImagePath = path;

            var result = await _userManager.CreateAsync(newUser.User, newUser.Password);
         
            if (result.Succeeded)
            {
                await _signInManager.PasswordSignInAsync(newUser.User.UserName, newUser.Password, false, false);
                return RedirectToAction("CreatePet");
            }

            for (int i = 0; i < 10; i++)
            {
                foreach (var error in result.Errors)
                {
//                ModelState.AddModelError(string.Empty, error.Description);
                    Console.WriteLine(error.Description);
                }
            }
            
            return View(newUser);
        }
        
        [Authorize]
        public IActionResult CreatePet()
        {
            var vm = new SignUpPetViewModel {Username = _userManager.GetUserAsync(HttpContext.User).Result.UserName};
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePet(SignUpPetViewModel newPet)
        {
            var path = "";
            if (newPet.ProfileImage != null)
            {
                var ext = newPet.ProfileImage.FileName.Split('.').Last();
                path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot") 
                       + "/images/pets/" + "main_" + newPet.Pet.Name + "." + ext;
                using (var fs = new FileStream(path, FileMode.Create))
                {
                    await newPet.ProfileImage.CopyToAsync(fs);
                }
                
            }
            
            var user = _userManager.Users.First(u => u.UserName == newPet.Username);
            newPet.Pet.ProfileImagePath = path;

            newPet.Pet.User = user;
            var a = db.Pets.Add(newPet.Pet);
            

            Console.WriteLine("ok");
            if (a.State == EntityState.Added)
            {
                Console.WriteLine("pet added");
            }
            else Redirect("CreatePet");
            
            db.SaveChanges();

            return Redirect("Login");
        }
    }
}