using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using E_LearningProject.Models;

namespace E_LearningProject.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDBContext _context;

        public UserController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
           

                // Hash the password before saving to the database
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

                // Add the user to the database
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Redirect to login page after successful registration
                return RedirectToAction("Login");
            }

            // If the model state is not valid, return the view with validation errors
            return View(user);
        }



        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);

            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email)
                };

                var identity = new ClaimsIdentity(claims, "CookieAuthentication");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("CookieAuthentication", principal);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid login attempt");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuthentication");
            return RedirectToAction("Login");
        }
    }
}
