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

namespace Task4.Middleware
{
    public class AuthenticationMiddleware
    {
        private RequestDelegate next;
        public AuthenticationMiddleware(RequestDelegate next) => this.next = next;

        

        public async Task InvokeAsync(HttpContext context, UserContext userContext)
        {                
            UserModel user = userContext.Users.FirstOrDefault(u => u.Email.Equals(context.User.Identity.Name));

            if (user is null)
            {
                await next(context);
                return;
            }   

            if (user.isBlocked)
            {
                await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                await context.Response.WriteAsync("You were blocked");
            }
            else
                await next(context);

        }

    }
}
