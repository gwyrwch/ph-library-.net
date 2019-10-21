using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ph.Models;

namespace ph.Controllers
{
    [Authorize(Roles="Admin")]
    public class AdminController : Controller
    {
        RoleManager<IdentityRole> _roleManager;
        UserManager<User> _userManager;
        
        public AdminController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        
        public IActionResult Index()
        {
            var users = _userManager.Users.Where(user => user.Id != "df205732-21c6-490c-89e4-ecf2a309fb1a");
            return View(users);
        }


        public IActionResult DeleteUser(string userId)
        {
            var userToDelete = _userManager.Users.First(user => user.Id == userId);
            var result = _userManager.DeleteAsync(userToDelete);
            
            // todo redirect to this method with parametr from index view;
            
            return Redirect("Index");
        }
    }
}