using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ph.Data;
using ph.Models;

namespace ph.Controllers
{
    public class AuthController : Controller
    {
        private ApplicationDbContext db;
        private UserManager<User> _userManager = null;
//        private SignInManager<User> _signInManager = null;

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
            
//            _signInManager = signInManager;
        }

        // GET
        public IActionResult Index()
        {
            return Redirect("Auth/Login");
        }

        public async Task<IActionResult> Registration()
        {
            return View();
        }

        [HttpPost]
        // todo: add checking password
        public async Task<IActionResult> Registration([Bind("UserName, Email")]User user)
        {
            var alreadyExists = TmpRAMDB.Users().Count(u => u.Email == user.Email || u.UserName == user.UserName);
            if (alreadyExists != 0)
            {
                return Redirect("/Home/Feed");
            }
            

            if (user.Email != string.Empty && user.UserName != String.Empty)
            {
//                user.Id = (user.UserName + user.Email).GetHashCode().ToString();
                TmpRAMDB.Users().Add(user);
                return Redirect("/Home/Feed");
            }

            return Redirect("/Home/Feed");
        }

        public async Task<IActionResult> Login()
        {
            return View();
        }
        
        [HttpPost]
        // todo: add checking password
        public IActionResult Login([Bind("Password, UserName")] LoginViewModel viewModel)
        {
            return Redirect("/Home/Feed");
        }
        
        public async Task<IActionResult> CreateUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(SignUpViewModel newUser)
        {
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
                for (int i = 0; i < 10; i++)
                    Console.WriteLine("ok");
                return RedirectToAction("CreatePet", "Auth", new { usename = newUser.User.UserName.ToLower() });
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
        
        public async Task<IActionResult> CreatePet(string username)
        {
            var vm = new SignUpPetViewModel();
            vm.Username = username;
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePet(SignUpPetViewModel newPet)
        {
            var path = "";
            if (newPet.ProfileImage != null)
            {
                for (int i = 0; i < 30; i++)
                {
                    Console.WriteLine(newPet.ProfileImage.FileName);
                }
                
                var ext = newPet.ProfileImage.FileName.Split('.').Last();
                path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot") 
                       + "/images/pets/" + "main_" + newPet.Pet.Name + "." + ext;
                using (var fs = new FileStream(path, FileMode.Create))
                {
                    await newPet.ProfileImage.CopyToAsync(fs);
                }
                
            }

            var user = _userManager.Users.First(u => u.UserName == newPet.Username);
//            newPet.Pet.ProfileImagePath = path;

            newPet.Pet.User = user;
//            db.Employees.Get(FocusedShift.AssignedEmployee.Id).Shifts.Add(FocusedShift);
            var a = db.Pets.Add(newPet.Pet);
            

            Console.WriteLine("ok");
            if (a.State == EntityState.Added)
            {
                Console.WriteLine("pet added");
            }
            else Redirect("Login");
            
            db.SaveChanges();

            return Redirect("Home/Feed");
        }
    }
}