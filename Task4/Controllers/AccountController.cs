using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Task4.Data;
using Task4.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Task4.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserContext userContext;

        public AccountController(UserContext userContext) =>
            (this.userContext) =
            (userContext);
        

        public ActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                UserModel user = null;
                user = userContext.Users.FirstOrDefault(u => u.Email == model.Email);
                if(user is null)
                {
                    user = new UserModel();
                    user.Email = model.Email;
                    user.Password = model.Password;
                    user.RegisterDate = DateTime.Now;
                    user.LastLoginDate = DateTime.Now;
                    user.isBlocked = false;

                    userContext.Users.Add(user);
                    userContext.SaveChanges();


                    user = userContext.Users.FirstOrDefault(u => u.Email == model.Email);
                    await Authenticate(user.Email);

                    if (user is null)
                        ModelState.AddModelError(string.Empty, "Error");
                    else
                        return RedirectToAction("Index", "Home");

                }
                else
                {
                    ModelState.AddModelError("", "User with this login already exists!");
                }

            }

            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                UserModel user = userContext.Users.FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password);

                if (user is null)
                    ModelState.AddModelError("", "There is no user with this username and password!");
                else if (user.isBlocked)
                    ModelState.AddModelError("", "You were blocked");
                else
                {
                    user.LastLoginDate = DateTime.Now;
                    userContext.SaveChanges();

                    await Authenticate(user.Email);
                    return RedirectToAction("Index", "Home");
                }
                
            }
            return View();
        }

        private async Task Authenticate(string email)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, email)
            };
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

    }
}
