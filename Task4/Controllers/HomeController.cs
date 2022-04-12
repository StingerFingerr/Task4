using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Task4.Data;
using Task4.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Task4.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserContext userContext;
        public HomeController(UserContext userContext) => this.userContext = userContext;

        [Authorize]
        public async Task<IActionResult> Index()
        {
            if (CheckUserStatus() is false)
                return await Logout();

            List<UserModel> users = userContext.Users.ToList<UserModel>();

            return View(users);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Block(IFormCollection form)
        {
            if (CheckUserStatus() is false)
                return await Logout();

            string[] ids = form["id"];
            bool goToLogin = false;

            foreach (var id in ids)
            {
                UserModel user = userContext.Users.First<UserModel>(u => u.ID == int.Parse(id));
                if (user is null)
                    continue;

                if (user.Email.Equals(User.Identity.Name))
                    goToLogin = true;
                
                user.isBlocked = true;
            }

            userContext.SaveChanges();

            if (goToLogin)

                return await Logout();

            else
                return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Unblock(IFormCollection form)
        {
            if (CheckUserStatus() is false)
                return await Logout();

            string[] ids = form["id"];

            foreach (var id in ids)
            {
                UserModel user = userContext.Users.First<UserModel>(u => u.ID == int.Parse(id));
                if (user is null)
                    continue;

                user.isBlocked = false;
            }

            userContext.SaveChanges();

            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Delete(IFormCollection form)
        {
            if (CheckUserStatus() is false)
                return await Logout();

            string[] ids = form["id"];
            bool goToLogin = false;
            
            foreach (var id in ids)
            {
                UserModel user = userContext.Users.First<UserModel>(u => u.ID == int.Parse(id));
                if (user is null)
                    continue;

                userContext.Remove(user);

                if (user.Email.Equals(User.Identity.Name))
                    goToLogin = true;          
            }
            userContext.SaveChanges();

            if (goToLogin)
                return await Logout();
            else
                return RedirectToAction("Index");
        }

        private bool CheckUserStatus()
        {
            UserModel user = userContext.Users.FirstOrDefault(u => u.Email.Equals(User.Identity.Name));

            if (user is null)
                return true;

            if (user.isBlocked)
                return false;

            return true;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}
