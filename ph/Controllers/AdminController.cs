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
            Console.WriteLine(_roleManager.Roles.ToList()[0]);
            return View();
        }
    }
}