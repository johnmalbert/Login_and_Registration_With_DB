using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using LoginRegWDb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace LoginRegWDb.Controllers
{
    public class HomeController : Controller
    {
        private MyContext _context;
     
        // here we can "inject" our context service into the constructor
        public HomeController(MyContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("NewUser")]
        public IActionResult NewUser(RegUser newUser)
        {
            if(ModelState.IsValid)
            {
                if(_context.Users.Any(u => u.Email == newUser.Email))
                {
                    // Manually add a ModelState error to the Email field, with provided
                    // error message
                    ModelState.AddModelError("Email", "Email already in use!");
                    return View("Index");
                }
                // hash password
                PasswordHasher<RegUser> Hasher = new PasswordHasher<RegUser>();
                newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                //add user to DB
                _context.Add(newUser);
                _context.SaveChanges();
                var UserId = _context.Users.FirstOrDefault(u => u.Email == newUser.Email).NewUserLogin;
                Console.WriteLine(UserId);                
                HttpContext.Session.SetInt32("UserId", UserId);
                return RedirectToAction("Success");
            }
            else
            {
                return View("Index");
            }
        }

        [HttpGet("success")]
        public IActionResult Success()
        {
            //only logged in users can see this page:
            var UserId = _context.Users.
                FirstOrDefault(u => u.NewUserLogin == HttpContext.Session.
                GetInt32("UserId"));
            if(UserId != null)
            {
                return View();
            }
            else
            {
                Console.WriteLine("User isn't logged in.");                
                return RedirectToAction("Index");
            }
        }

        [HttpPost("login-attempt")]
        public IActionResult AttemptLogin(LoginUser user)
        {
            if(ModelState.IsValid)
            {
                //see if user is in db
                var userInDb = _context.Users.FirstOrDefault(u => u.Email == user.Email);

                if(userInDb == null)
                {
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("Login");
                }

                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(user, userInDb.Password, user.Password);

                if(result == 0)
                {
                    //password doesn't match...
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("Login");
                }
                //everything is all good, user exists and password matches
                var UserId = _context.Users.FirstOrDefault(u => u.Email == user.Email).NewUserLogin;
                Console.WriteLine(UserId);                
                HttpContext.Session.SetInt32("UserId", UserId);
                return RedirectToAction("Success");
            }
            return View("Login");
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
