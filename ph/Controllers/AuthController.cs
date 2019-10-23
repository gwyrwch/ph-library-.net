using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
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
                    // проверяем, принадлежит ли URL приложению
                    if (!string.IsNullOrEmpty(user.ReturnUrl) && Url.IsLocalUrl(user.ReturnUrl))
                    {
                        return Redirect(user.ReturnUrl);
                    }

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Invalid username or password");
            }
            return View(user);
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
                return RedirectToAction("CreatePet", new { username = newUser.User.UserName });
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
                //todo: this is how delete of the user is working now            
//            var id = "a44c78b4-4477-4a84-badd-43900512487d";
//
//            var users = _userManager.Users;
//            var result = await _userManager.DeleteAsync(users.First(user => user.Id == id));
//
//            if (result.Succeeded)
//            {
//                for (int i = 0; i < 10; i++)
//                    Console.WriteLine("ok");
//                return RedirectToAction("CreatePet", new { username = "gwyrwch" });
//            }
            var vm = new SignUpPetViewModel {Username = username};
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